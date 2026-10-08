using CashierPOS.Application;
using CashierPOS.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Cryptography;
namespace CashierPOS.Infrastructure;
public sealed class PosDbContext(DbContextOptions<PosDbContext> options) : DbContext(options), IPosDb {
 IQueryable<T> IPosDb.Set<T>()=>Set<T>();void IPosDb.Add<T>(T value)=>Add(value);void IPosDb.Remove<T>(T value)=>Remove(value);void IPosDb.Save()=>SaveChanges();
 public ITransaction Begin()=>new DbTransaction(Database.BeginTransaction());
 protected override void OnModelCreating(ModelBuilder m){
  foreach(var t in typeof(User).Assembly.GetTypes().Where(t=>!t.IsAbstract&&typeof(Entity).IsAssignableFrom(t))){m.Entity(t).HasBaseType((Type?)null);m.Entity(t).HasKey("Id");}
  m.Entity<User>().HasIndex(x=>x.NormalizedUsername).IsUnique();m.Entity<Role>().HasIndex(x=>x.Name).IsUnique();m.Entity<Permission>().HasIndex(x=>x.Code).IsUnique();
  m.Entity<RolePermission>().HasIndex(x=>new{x.RoleId,x.PermissionId}).IsUnique();m.Entity<UserPermissionOverride>().HasIndex(x=>new{x.UserId,x.PermissionId}).IsUnique();
  m.Entity<Product>().HasIndex(x=>x.SKU).IsUnique();m.Entity<Product>().HasIndex(x=>x.Barcode).IsUnique().HasFilter("\"Barcode\" <> ''");m.Entity<Product>().Property(x=>x.Version).IsConcurrencyToken();
  m.Entity<Product>().ToTable(t=>{t.HasCheckConstraint("CK_Product_Stock","Stock >= 0");t.HasCheckConstraint("CK_Product_Prices","CostCents >= 0 AND PriceCents >= 0");});
  m.Entity<Sale>().HasIndex(x=>x.RequestId).IsUnique();m.Entity<Sale>().HasIndex(x=>x.InvoiceNumber).IsUnique();m.Entity<Sale>().HasIndex(x=>new{x.UserId,x.CreatedAtUtc});
  m.Entity<Refund>().HasIndex(x=>x.RequestId).IsUnique();m.Entity<ApprovalDecision>().HasIndex(x=>x.Token).IsUnique();m.Entity<ApplicationSetting>().HasIndex(x=>x.Key).IsUnique();m.Entity<Category>().HasIndex(x=>x.Name).IsUnique();
  m.Entity<CashierShift>().HasIndex(x=>x.UserId).IsUnique().HasFilter("\"ClosedAtUtc\" IS NULL");
  Foreign<User,Role>(m,"RoleId");Foreign<RolePermission,Role>(m,"RoleId");Foreign<RolePermission,Permission>(m,"PermissionId");Foreign<UserPermissionOverride,User>(m,"UserId");Foreign<UserPermissionOverride,Permission>(m,"PermissionId");
  Foreign<Product,Category>(m,"CategoryId");Foreign<Product,Supplier>(m,"SupplierId");Foreign<Sale,User>(m,"UserId");Foreign<Sale,CashierShift>(m,"ShiftId");Foreign<Sale,Customer>(m,"CustomerId");Foreign<SaleItem,Sale>(m,"SaleId");Foreign<SaleItem,Product>(m,"ProductId");Foreign<Payment,Sale>(m,"SaleId");
  Foreign<InventoryMovement,Product>(m,"ProductId");Foreign<InventoryMovement,User>(m,"UserId");Foreign<CashierShift,User>(m,"UserId");Foreign<CashMovement,CashierShift>(m,"ShiftId");Foreign<CashMovement,User>(m,"UserId");
  Foreign<Refund,Sale>(m,"SaleId");Foreign<Refund,SaleItem>(m,"SaleItemId");Foreign<Refund,User>(m,"UserId");Foreign<Refund,CashierShift>(m,"ShiftId");Foreign<RefundPayment,Refund>(m,"RefundId");Foreign<RefundPayment,Payment>(m,"PaymentId");
  Foreign<Purchase,Supplier>(m,"SupplierId");Foreign<Purchase,User>(m,"UserId");Foreign<PurchaseItem,Purchase>(m,"PurchaseId");Foreign<PurchaseItem,Product>(m,"ProductId");Foreign<Expense,User>(m,"UserId");Foreign<SuspendedSale,User>(m,"UserId");Foreign<ApprovalDecision,User>(m,"EmployeeId");Foreign<ApprovalDecision,User>(m,"ManagerId");
 }
 private static void Foreign<T,U>(ModelBuilder m,string key) where T:Entity where U:Entity=>m.Entity<T>().HasOne<U>().WithMany().HasForeignKey(key).OnDelete(DeleteBehavior.Restrict);
 private sealed class DbTransaction(IDbContextTransaction tx) : ITransaction { public void Commit()=>tx.Commit();public void Dispose()=>tx.Dispose(); }
}
public sealed class SqliteFactory(string path) : IDbFactory {
 public string Path {get;}=System.IO.Path.GetFullPath(path);
 public PosDbContext Create(){Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);return new PosDbContext(new DbContextOptionsBuilder<PosDbContext>().UseSqlite(new SqliteConnectionStringBuilder{DataSource=Path,ForeignKeys=true,Pooling=false,DefaultTimeout=15}.ToString()).Options);}
 public IPosDb Open()=>Create();
 public void Initialize(){using var db=Create();db.Database.Migrate();using var tx=db.Database.BeginTransaction();
  if(!db.Set<Role>().Any()){
   var manager=new Role{Name="Manager"};var employee=new Role{Name="Employee"};db.AddRange(manager,employee);db.SaveChanges();
   foreach(var code in Permissions.All.Distinct()){var p=new Permission{Code=code,ManagerOnly=Permissions.ManagerOnly.Contains(code)};db.Add(p);db.SaveChanges();db.Add(new RolePermission{RoleId=manager.Id,PermissionId=p.Id});if(Permissions.EmployeeDefault.Contains(code))db.Add(new RolePermission{RoleId=employee.Id,PermissionId=p.Id});}db.SaveChanges();
  }
  foreach(var (key,value) in new Dictionary<string,string>{{"StoreName","Cashier POS"},{"Currency","EGP"},{"Language","en"},{"Theme","light"},{"TaxPercent","0"},{"TaxInclusive","false"},{"EmployeeDiscountPercent","0"},{"AutoLockMinutes","10"},{"ReceiptFooter","Thank you / شكراً لزيارتكم"},{"StoreAddress",""},{"StorePhone",""},{"AutoBackupMinutes","0"},{"BackupDirectory",System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path)!,"Backups")}})if(!db.Set<ApplicationSetting>().Any(x=>x.Key==key))db.Add(new ApplicationSetting{Key=key,Value=value});
  db.SaveChanges();tx.Commit();
 }
}
public sealed class DesignFactory : IDesignTimeDbContextFactory<PosDbContext> {public PosDbContext CreateDbContext(string[] args)=>new SqliteFactory(System.IO.Path.Combine(System.IO.Path.GetTempPath(),"cashierpos-design.db")).Create();}
public sealed class PasswordHasher : IPasswordHasher {
 private const int Iterations=600000;
 public string Hash(string password){var salt=RandomNumberGenerator.GetBytes(16);var hash=Rfc2898DeriveBytes.Pbkdf2(password,salt,Iterations,HashAlgorithmName.SHA256,32);return $"PBKDF2-SHA256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";}
 public bool Verify(string password,string hash){try{var parts=hash.Split('$');if(parts.Length!=4||parts[0]!="PBKDF2-SHA256"||!int.TryParse(parts[1],out var iterations)||iterations<100000||iterations>2000000)return false;var salt=Convert.FromBase64String(parts[2]);var expected=Convert.FromBase64String(parts[3]);if(salt.Length!=16||expected.Length!=32)return false;var actual=Rfc2898DeriveBytes.Pbkdf2(password,salt,iterations,HashAlgorithmName.SHA256,32);return CryptographicOperations.FixedTimeEquals(actual,expected);}catch(FormatException){return false;}}
}
