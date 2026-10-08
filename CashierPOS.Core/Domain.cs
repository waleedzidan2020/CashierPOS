namespace CashierPOS.Core;
public abstract class Entity { public long Id { get; set; } }
public sealed class Role : Entity { public string Name { get; set; } = ""; }
public sealed class Permission : Entity { public string Code { get; set; } = ""; public bool ManagerOnly { get; set; } }
public sealed class RolePermission : Entity { public long RoleId { get; set; } public long PermissionId { get; set; } }
public sealed class UserPermissionOverride : Entity { public long UserId { get; set; } public long PermissionId { get; set; } public bool Allowed { get; set; } }
public sealed class User : Entity {
 public string Username { get; set; }=""; public string NormalizedUsername { get; set; }=""; public string FullName { get; set; }="";
 public string PasswordHash { get; set; }=""; public long RoleId { get; set; } public bool IsActive { get; set; }=true;
 public int FailedLoginAttempts { get; set; } public DateTime? LockedUntilUtc { get; set; }
 public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow; public DateTime? LastLoginUtc { get; set; }
 public long? CreatedBy { get; set; } public int SessionVersion { get; set; }
}
public sealed class Category : Entity { public string Name { get; set; }=""; }
public sealed class Supplier : Entity { public string Name { get; set; }=""; public string Phone { get; set; }=""; public string Notes { get; set; }=""; }
public sealed class Customer : Entity { public string Name { get; set; }=""; public string Phone { get; set; }=""; public string Notes { get; set; }=""; }
public sealed class Product : Entity {
 public string SKU { get; set; }=""; public string Barcode { get; set; }=""; public string Name { get; set; }="";
 public long? CategoryId { get; set; } public long? SupplierId { get; set; }
 public long CostCents { get; set; } public long PriceCents { get; set; } public int Stock { get; set; }
 public int MinimumStock { get; set; } public bool IsActive { get; set; }=true; public int Version { get; set; }
}
public sealed class Sale : Entity {
 public string InvoiceNumber { get; set; }=""; public string RequestHash { get; set; }=""; public Guid RequestId { get; set; }
 public long UserId { get; set; } public long ShiftId { get; set; } public long? CustomerId { get; set; }
 public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow;
 public long SubtotalCents { get; set; } public long DiscountCents { get; set; } public long TaxCents { get; set; }
 public long TotalCents { get; set; } public long CostCents { get; set; } public long ChangeCents { get; set; }
 public string Notes { get; set; }="";
}
public sealed class SaleItem : Entity {
 public long SaleId { get; set; } public long ProductId { get; set; } public string ProductName { get; set; }="";
 public string SKU { get; set; }=""; public int Quantity { get; set; } public long UnitPriceCents { get; set; }
 public long CostCents { get; set; } public long DiscountCents { get; set; } public long TaxCents { get; set; } public long TotalCents { get; set; }
}
public sealed class Payment : Entity { public long SaleId { get; set; } public string Method { get; set; }="Cash"; public long AmountCents { get; set; } }
public sealed class InventoryMovement : Entity {
 public long ProductId { get; set; } public string Type { get; set; }=""; public int QuantityChange { get; set; }
 public int BeforeQuantity { get; set; } public int AfterQuantity { get; set; }
 public string ReferenceType { get; set; }=""; public long? ReferenceId { get; set; } public long UserId { get; set; }
 public long? ApprovedBy { get; set; } public string Reason { get; set; }=""; public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow;
}
public sealed class CashierShift : Entity {
 public long UserId { get; set; } public DateTime OpenedAtUtc { get; set; }=DateTime.UtcNow; public DateTime? ClosedAtUtc { get; set; }
 public long OpeningCents { get; set; } public long? ExpectedCents { get; set; } public long? CountedCents { get; set; } public string Notes { get; set; }="";
}
public sealed class CashMovement : Entity { public long ShiftId { get; set; } public long AmountCents { get; set; } public string Reason { get; set; }=""; public long UserId { get; set; } public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow; }
public sealed class Refund : Entity {
 public long SaleId { get; set; } public long SaleItemId { get; set; } public long UserId { get; set; } public long ShiftId { get; set; }
 public int Quantity { get; set; } public long AmountCents { get; set; } public long TaxCents { get; set; } public long CostCents { get; set; }
 public bool Restock { get; set; } public string Reason { get; set; }=""; public Guid RequestId { get; set; }
 public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow;
}
public sealed class RefundPayment : Entity { public long RefundId { get; set; } public long PaymentId { get; set; } public string Method { get; set; }=""; public long AmountCents { get; set; } }
public sealed class Purchase : Entity { public long SupplierId { get; set; } public long UserId { get; set; } public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow; public string SupplierInvoice { get; set; }=""; public long TotalCents { get; set; } }
public sealed class PurchaseItem : Entity { public long PurchaseId { get; set; } public long ProductId { get; set; } public int Ordered { get; set; } public int Received { get; set; } public long UnitCostCents { get; set; } }
public sealed class Expense : Entity { public string Category { get; set; }=""; public string Description { get; set; }=""; public long AmountCents { get; set; } public long UserId { get; set; } public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow; }
public sealed class ApprovalDecision : Entity {
 public Guid Token { get; set; } public long EmployeeId { get; set; } public long ManagerId { get; set; } public string Permission { get; set; }="";
 public string PayloadHash { get; set; }=""; public string Reason { get; set; }=""; public DateTime ExpiresAtUtc { get; set; } public DateTime? UsedAtUtc { get; set; }
}
public sealed class AuditLog : Entity { public long? UserId { get; set; } public string Action { get; set; }=""; public string Entity { get; set; }=""; public long? EntityId { get; set; } public string Details { get; set; }=""; public long? ApprovedBy { get; set; } public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow; }
public sealed class ApplicationSetting : Entity { public string Key { get; set; }=""; public string Value { get; set; }=""; }
public sealed class BackupHistory : Entity { public string Path { get; set; }=""; public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow; public long UserId { get; set; } }
public sealed class SuspendedSale : Entity { public long UserId { get; set; } public string Name { get; set; }=""; public string CartJson { get; set; }=""; public DateTime CreatedAtUtc { get; set; }=DateTime.UtcNow; }
public static class Money {
 public static long Cents(decimal amount) => checked((long)decimal.Round(amount*100m,0,MidpointRounding.AwayFromZero));
 public static decimal Amount(long cents) => cents/100m;
 public static string Text(long cents) => Amount(cents).ToString("N2");
}
