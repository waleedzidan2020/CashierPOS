using CashierPOS.Core;
namespace CashierPOS.Application;
public interface ITransaction : IDisposable { void Commit(); }
public interface IPosDb : IDisposable {
 IQueryable<T> Set<T>() where T:Entity; void Add<T>(T value) where T:Entity; void Remove<T>(T value) where T:Entity;
 void Save(); ITransaction Begin();
}
public interface IDbFactory { IPosDb Open(); }
public interface IPasswordHasher { string Hash(string password); bool Verify(string password,string hash); }
public interface IClock { DateTime UtcNow {get;} }
public sealed class SystemClock : IClock { public DateTime UtcNow=>DateTime.UtcNow; }
public sealed record Session(long UserId,string Username,string FullName,bool IsManager,int Version,DateTime AuthenticatedAtUtc,Guid Token);
public sealed class DomainException(string message) : Exception(message);
public sealed class ApprovalRequiredException(string permission,string payload) : Exception("Manager approval required: "+permission) { public string Permission {get;}=permission; public string Payload {get;}=payload; }
public static class Permissions {
 public static readonly string[] ManagerOnly=["Users.Manage","Permissions.Manage","AuditLogs.View","Backup.Create","Backup.Restore","Settings.Manage"];
 public static readonly string[] EmployeeDefault=["Sales.Create","Sales.ViewOwn","Sales.Suspend","Products.View","Inventory.View","Shifts.Own","Reports.ViewOwn","Receipts.Print"];
 public static readonly string[] All=[..ManagerOnly,..EmployeeDefault,"Products.Create","Products.Edit","Products.Archive","Products.Cost","Products.ChangePrice","Inventory.Adjust","Sales.ViewAll","Sales.ApplyDiscount","Sales.OverridePrice","Sales.Refund","Sales.Void","Customers.View","Customers.Create","Customers.Edit","Suppliers.Manage","Purchases.Manage","Expenses.View","Expenses.Create","Reports.ViewAll","Excel.Import","Excel.Export","Shifts.All"];
}
public sealed record ProductView(long Id,string SKU,string Barcode,string Name,long PriceCents,long? CostCents,int Stock,int MinimumStock,bool IsActive);
public sealed record UserView(long Id,string Username,string FullName,string Role,bool IsActive,int FailedAttempts);
public sealed record ProductInput(long Id,string SKU,string Barcode,string Name,decimal Cost,decimal Price,int MinimumStock,long? CategoryId=null,long? SupplierId=null);
public sealed record CartLine(long ProductId,int Quantity,long? OverridePriceCents=null,long? ExpectedPriceCents=null);
public sealed record Tender(string Method,long AmountCents);
public sealed record Checkout(Guid RequestId,List<CartLine> Lines,long DiscountCents,List<Tender> Payments,long? CustomerId=null,string Notes="",Guid? Approval=null);
public sealed record SaleReceipt(Sale Sale,List<SaleItem> Items,List<Payment> Payments,string Cashier);
public sealed record Report(DateTime FromUtc,DateTime ToUtc,long SalesCents,long RefundCents,long NetRevenueCents,long TaxCents,long? GrossProfitCents,long? ExpensesCents,long? NetProfitCents,int Transactions,int LowStock,List<Sale> Sales);
public sealed record RefundInput(Guid RequestId,long SaleItemId,int Quantity,bool Restock,string Reason,Guid? Approval=null);
public sealed record PurchaseLine(long ProductId,int Quantity,long CostCents);
public sealed record ImportRow(int Row,string SKU,string Barcode,string Name,decimal Cost,decimal Price,int Quantity,int MinimumStock);
public sealed record ImportResult(int Created,int Updated,int Skipped,List<string> Errors);
