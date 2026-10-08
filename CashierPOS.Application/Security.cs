using CashierPOS.Core;
using System.Security.Cryptography;
using System.Text;
namespace CashierPOS.Application;
public sealed class SecurityService(IDbFactory factory,IPasswordHasher hasher,IClock clock) {
 private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid,(long UserId,int Version)> sessions=new();
 public void RevokeAllSessions()=>sessions.Clear();
 public bool NeedsSetup() {using var db=factory.Open();return !db.Set<User>().Any();}
 public Session Setup(string username,string fullName,string password) {
  using var db=factory.Open();using var tx=db.Begin();
  if(db.Set<User>().Any())throw new DomainException("Initial setup has already been completed.");
  ValidateUser(username,fullName,password);
  var user=new User{Username=username.Trim(),NormalizedUsername=Normalize(username),FullName=fullName.Trim(),PasswordHash=hasher.Hash(password),RoleId=db.Set<Role>().Single(r=>r.Name=="Manager").Id,CreatedAtUtc=clock.UtcNow};
  db.Add(user);db.Save();Log(db,user.Id,"Manager.Setup","User",user.Id);db.Save();tx.Commit();return ToSession(db,user);
 }
 public Session Login(string username,string password) {
  using var db=factory.Open();using var tx=db.Begin();var normalized=Normalize(username);var u=db.Set<User>().SingleOrDefault(x=>x.NormalizedUsername==normalized);
  var allowed=u!=null&&u.IsActive&&!(u.LockedUntilUtc>clock.UtcNow)&&hasher.Verify(password,u.PasswordHash);
  if(!allowed){if(u!=null&&u.IsActive&&!(u.LockedUntilUtc>clock.UtcNow)){u.FailedLoginAttempts++;if(u.FailedLoginAttempts>=5)u.LockedUntilUtc=clock.UtcNow.AddMinutes(5);}
   Log(db,u?.Id,"Login.Failed","User",u?.Id);db.Save();tx.Commit();throw new DomainException("Invalid credentials, inactive account, or account temporarily locked.");}
  u!.FailedLoginAttempts=0;u.LockedUntilUtc=null;u.LastLoginUtc=clock.UtcNow;Log(db,u.Id,"Login.Success","User",u.Id);db.Save();tx.Commit();return ToSession(db,u);
 }
 public User ValidateSession(IPosDb db,Session session) {
  if(!sessions.TryGetValue(session.Token,out var identity)||identity.UserId!=session.UserId||identity.Version!=session.Version)throw new DomainException("Invalid session identity.");
  var u=db.Set<User>().SingleOrDefault(x=>x.Id==session.UserId);
  if(u==null||!u.IsActive||u.SessionVersion!=session.Version||u.LockedUntilUtc>clock.UtcNow)throw new DomainException("Session expired. Please sign in again.");return u;
 }
 public bool Can(IPosDb db,Session session,string code) {
  var u=ValidateSession(db,session);var p=db.Set<Permission>().SingleOrDefault(x=>x.Code==code);if(p==null)return false;
  var manager=db.Set<Role>().Any(x=>x.Id==u.RoleId&&x.Name=="Manager");if(p.ManagerOnly&&!manager)return false;
  var ov=db.Set<UserPermissionOverride>().SingleOrDefault(x=>x.UserId==u.Id&&x.PermissionId==p.Id);
  return ov?.Allowed??db.Set<RolePermission>().Any(x=>x.RoleId==u.RoleId&&x.PermissionId==p.Id);
 }
 public bool Can(Session s,string code) {using var db=factory.Open();return Can(db,s,code);}
 public void Require(IPosDb db,Session s,string code) {if(!Can(db,s,code))throw new DomainException("Permission denied: "+code);}
 public long? RequireOrApproval(IPosDb db,Session s,string permission,string payload,Guid? token,bool force=false) {
  ValidateSession(db,s);if(!force&&Can(db,s,permission))return null;
  if(Permissions.ManagerOnly.Contains(permission))throw new DomainException("Manager-only operation.");
  if(token is null)throw new ApprovalRequiredException(permission,payload);
  var a=db.Set<ApprovalDecision>().SingleOrDefault(x=>x.Token==token.Value);
  if(a==null||a.EmployeeId!=s.UserId||a.Permission!=permission||a.PayloadHash!=Hash(payload)||a.UsedAtUtc!=null||a.ExpiresAtUtc<=clock.UtcNow)throw new DomainException("Approval is invalid, used, expired or belongs to another operation.");
  var manager=db.Set<User>().Single(x=>x.Id==a.ManagerId);
  if(!manager.IsActive||manager.LockedUntilUtc>clock.UtcNow||!db.Set<Role>().Any(x=>x.Id==manager.RoleId&&x.Name=="Manager"))throw new DomainException("Approving manager is no longer authorized.");
  a.UsedAtUtc=clock.UtcNow;return manager.Id;
 }
 public Guid Approve(Session employee,string username,string password,string permission,string payload,string reason) {
  if(string.IsNullOrWhiteSpace(reason))throw new DomainException("An approval reason is required.");
  var manager=Login(username,password);if(!manager.IsManager)throw new DomainException("Approval requires an active Manager.");
  using var db=factory.Open();using var tx=db.Begin();ValidateSession(db,employee);Require(db,manager,permission);
  if(Permissions.ManagerOnly.Contains(permission))throw new DomainException("This permission cannot be delegated.");
  var a=new ApprovalDecision{Token=Guid.NewGuid(),EmployeeId=employee.UserId,ManagerId=manager.UserId,Permission=permission,PayloadHash=Hash(payload),Reason=reason.Trim(),ExpiresAtUtc=clock.UtcNow.AddMinutes(2)};
  db.Add(a);Log(db,employee.UserId,"Manager.Approval",permission,null,"Payload hash: "+a.PayloadHash+"; reason: "+reason,manager.UserId);db.Save();tx.Commit();return a.Token;
 }
 public List<UserView> Users(Session s){using var db=factory.Open();Require(db,s,"Users.Manage");return (from u in db.Set<User>() join r in db.Set<Role>() on u.RoleId equals r.Id select new UserView(u.Id,u.Username,u.FullName,r.Name,u.IsActive,u.FailedLoginAttempts)).ToList();}
 public long CreateUser(Session s,string username,string name,string password,bool manager=false){ValidateUser(username,name,password);using var db=factory.Open();using var tx=db.Begin();Require(db,s,"Users.Manage");var norm=Normalize(username);if(db.Set<User>().Any(x=>x.NormalizedUsername==norm))throw new DomainException("Username already exists.");var u=new User{Username=username.Trim(),NormalizedUsername=norm,FullName=name.Trim(),PasswordHash=hasher.Hash(password),RoleId=db.Set<Role>().Single(r=>r.Name==(manager?"Manager":"Employee")).Id,CreatedBy=s.UserId,CreatedAtUtc=clock.UtcNow};db.Add(u);db.Save();Log(db,s.UserId,"User.Create","User",u.Id);db.Save();tx.Commit();return u.Id;}
 public void SetActive(Session s,long id,bool active){using var db=factory.Open();using var tx=db.Begin();Require(db,s,"Users.Manage");var u=db.Set<User>().Single(x=>x.Id==id);var managerId=db.Set<Role>().Single(x=>x.Name=="Manager").Id;if(!active&&u.RoleId==managerId&&u.IsActive&&db.Set<User>().Count(x=>x.IsActive&&x.RoleId==managerId)<=1)throw new DomainException("Cannot deactivate the last active Manager.");u.IsActive=active;u.SessionVersion++;Log(db,s.UserId,"User.Active","User",id,"Active="+active);db.Save();tx.Commit();}
 public void ResetPassword(Session s,long id,string password){ValidatePassword(password);using var db=factory.Open();using var tx=db.Begin();Require(db,s,"Users.Manage");var u=db.Set<User>().Single(x=>x.Id==id);u.PasswordHash=hasher.Hash(password);u.SessionVersion++;u.FailedLoginAttempts=0;u.LockedUntilUtc=null;Log(db,s.UserId,"Password.Reset","User",id);db.Save();tx.Commit();}
 public void ChangePassword(Session s,string oldPassword,string newPassword){ValidatePassword(newPassword);using var db=factory.Open();using var tx=db.Begin();var u=ValidateSession(db,s);if(!hasher.Verify(oldPassword,u.PasswordHash))throw new DomainException("Current password is incorrect.");u.PasswordHash=hasher.Hash(newPassword);u.SessionVersion++;Log(db,u.Id,"Password.Change","User",u.Id);db.Save();tx.Commit();}
 public Dictionary<string,bool> GetPermissions(Session s,long userId){using var db=factory.Open();Require(db,s,"Permissions.Manage");var u=db.Set<User>().Single(x=>x.Id==userId);return db.Set<Permission>().AsEnumerable().ToDictionary(p=>p.Code,p=>!p.ManagerOnly&&(db.Set<UserPermissionOverride>().SingleOrDefault(x=>x.UserId==u.Id&&x.PermissionId==p.Id)?.Allowed??db.Set<RolePermission>().Any(x=>x.RoleId==u.RoleId&&x.PermissionId==p.Id)));}
 public void Override(Session s,long userId,string code,bool? allowed){using var db=factory.Open();using var tx=db.Begin();Require(db,s,"Permissions.Manage");var user=db.Set<User>().Single(x=>x.Id==userId);if(db.Set<Role>().Any(r=>r.Id==user.RoleId&&r.Name=="Manager"))throw new DomainException("This screen configures Employee permissions only.");var p=db.Set<Permission>().Single(x=>x.Code==code);if(p.ManagerOnly)throw new DomainException("Manager-only permission cannot be changed for Employees.");var old=db.Set<UserPermissionOverride>().SingleOrDefault(x=>x.UserId==userId&&x.PermissionId==p.Id);if(old!=null){if(allowed==null)db.Remove(old);else old.Allowed=allowed.Value;}else if(allowed!=null)db.Add(new UserPermissionOverride{UserId=userId,PermissionId=p.Id,Allowed=allowed.Value});Log(db,s.UserId,"Permission.Change","User",userId,$"{code}={allowed?.ToString()??"role default"}");db.Save();tx.Commit();}
 public void Logout(Session s){if(!sessions.TryRemove(s.Token,out var identity)||identity.UserId!=s.UserId)return;using var db=factory.Open();Log(db,s.UserId,"Logout","User",s.UserId);db.Save();}
 private Session ToSession(IPosDb db,User u){var token=Guid.NewGuid();sessions[token]=(u.Id,u.SessionVersion);return new Session(u.Id,u.Username,u.FullName,db.Set<Role>().Any(r=>r.Id==u.RoleId&&r.Name=="Manager"),u.SessionVersion,clock.UtcNow,token);}
 public bool IsManager(IPosDb db,Session s){var u=ValidateSession(db,s);return db.Set<Role>().Any(r=>r.Id==u.RoleId&&r.Name=="Manager");}
 public static string Normalize(string value)=>value.Trim().ToUpperInvariant();
 public static string Hash(string value)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
 private static void ValidateUser(string username,string name,string password){if(string.IsNullOrWhiteSpace(username)||username.Length>64||string.IsNullOrWhiteSpace(name)||name.Length>120)throw new DomainException("Username and full name are required (maximum 64 / 120 characters).");ValidatePassword(password);}
 private static void ValidatePassword(string p){if(p.Length<10||p.Length>128||!p.Any(char.IsLetter)||!p.Any(char.IsDigit))throw new DomainException("Password must have 10–128 characters, including a letter and a number.");}
 internal void Log(IPosDb db,long? actor,string action,string entity,long? id,string details="",long? approvedBy=null)=>db.Add(new AuditLog{UserId=actor,Action=action,Entity=entity,EntityId=id,Details=details,ApprovedBy=approvedBy,CreatedAtUtc=clock.UtcNow});
}
