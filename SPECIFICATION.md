> **Implementation update (October 2026):** Manager approval dialogs and single-use approvals described in this original specification have been retired. The current application enforces explicitly granted Employee permissions instead; unauthorized operations are denied rather than requesting a manager's password. Historical approval schema is retained for database compatibility. See README / FEATURES for current behavior.

# MASTER DEVELOPMENT PROMPT — PROFESSIONAL WPF CASHIER & POS MANAGEMENT SYSTEM

## ROLE

Act as a Senior C#/.NET Desktop Software Architect, WPF UI/UX Designer, Database Engineer, and Security Engineer.

Build a complete, professional, production-oriented Windows desktop Point of Sale (POS) and Cashier Management System.

This must be a REAL, FUNCTIONAL application with fully connected screens, working database operations, complete business logic, security, authentication, authorization, reporting, and receipt printing.

Do not build a UI-only demonstration. Do not create non-functional buttons, fake data services, or placeholder business logic.

The application should be suitable for a small or medium retail store, supermarket, grocery shop, clothing store, or other retail business.

## 1. TECHNOLOGY STACK

Use the following technologies:

- Language: C#
- Desktop framework: WPF
- Runtime: .NET 10 LTS
- UI markup: XAML
- Architecture: MVVM + Clean Architecture principles
- MVVM library: CommunityToolkit.Mvvm
- Database: SQLite
- ORM: Entity Framework Core
- Dependency injection: Microsoft.Extensions.DependencyInjection
- Logging: Microsoft.Extensions.Logging
- Excel import/export: ClosedXML
- Charts: A maintained WPF-compatible charting library
- Password security: A modern salted password hashing algorithm
- Asynchronous operations where appropriate
- Dependency injection throughout the application

Use well-maintained, compatible NuGet packages.

The application must work completely offline on a single Windows computer.

Design for future expansion to SQL Server and a multi-terminal architecture, but do not require a server or internet connection in the initial version.

Keep the application lightweight and efficient, including on computers with 4 GB RAM.

## 2. PROJECT ARCHITECTURE

Use a modular solution structure:

CashierPOS.sln

- CashierPOS.WPF — Windows, Views, ViewModels, Themes, Resources
- CashierPOS.Core — Entities, Enums, Domain Models, Business Rules
- CashierPOS.Application — Services, DTOs, Interfaces, Validation
- CashierPOS.Infrastructure — EF Core, SQLite, Repositories, Printing, Excel, Backups
- CashierPOS.Tests — Unit and Integration Tests

Apply:

- SOLID principles
- MVVM pattern
- Dependency injection
- Separation of concerns
- Centralized exception handling
- Centralized permission validation
- Reusable components
- Testable business services
- Reliable database transactions

Do not place critical business logic inside XAML code-behind.

## 3. AUTHENTICATION AND USER MANAGEMENT

Implement a complete local authentication system.

The application has TWO primary roles:

1. MANAGER
2. EMPLOYEE

On first launch, display an initial setup wizard requiring the creation of the first Manager account.

Do not create a publicly known default password.

User properties:

- UserId
- Username
- FullName
- PasswordHash
- RoleId
- Email (optional)
- Phone (optional)
- IsActive
- IsLocked
- FailedLoginAttempts
- CreatedAt
- LastLoginAt
- CreatedBy
- UpdatedAt

Authentication features:

- Secure username/password login
- Password visibility toggle
- Remember username
- Secure password hashing and verification
- Password change
- Manager-controlled password reset
- Logout
- Session lock
- Auto-lock after configurable inactivity
- Failed login tracking and temporary account lockout
- Authentication activity logging
- Disable/enable user accounts
- Prevent inactive users from logging in
- Prevent removing or deactivating the last active Manager
- Optional employee PIN for fast shift authentication

Never store plaintext passwords or sensitive authentication secrets.

## 4. ROLE-BASED ACCESS CONTROL (RBAC)

Implement a complete PERMISSION-BASED authorization engine, not only a basic role check.

Database entities:

- Roles
- Permissions
- RolePermissions
- Users
- UserPermissionOverrides
- AuditLogs

Create a central AuthorizationService.

Every protected business operation must check its required permission.

Hiding a UI button must NOT be considered sufficient authorization.

### Manager role

By default, a Manager can:

- Access the complete dashboard
- Manage employee accounts
- Activate/deactivate employees
- Change employee permissions
- Add/edit/archive products
- Manage categories and brands
- Manage suppliers and customers
- Create and manage purchases
- Manage stock
- Update product prices
- Access product purchase costs and profit margins
- Create and refund sales
- Approve restricted discounts
- Approve employee refund requests
- Access all reports
- Access company profit data
- Manage expenses
- Export business data
- Import Excel data
- Configure receipt and tax settings
- Configure application settings
- Manage backups and restores
- View audit logs
- Manage shifts and cash reconciliation

### Employee role

By default, an Employee can:

- Login to their own account
- Open the POS sales screen
- Search products
- Scan barcodes
- Add/remove products from an active cart
- Change permitted quantities
- Accept payments
- Complete sales
- Print receipts
- View their own transaction history
- Open and close assigned cash shifts
- View basic product information
- View available stock quantities
- Register customers when authorized

By default, an Employee CANNOT:

- Create employee accounts
- Manage roles
- Modify permissions
- Modify purchase costs
- View company-wide profits
- Delete or archive products
- Change product selling prices
- Change inventory stock directly
- Delete completed transactions
- Issue unauthorized refunds
- Apply restricted discounts
- Restore databases
- Access sensitive system configuration
- Access other employees' private reports

### Dynamic Employee Permission Management

Create a Manager-only "Roles & Permissions" screen.

The Manager can select an Employee and enable/disable individual grantable permissions.

Examples:

- Sales.Create
- Sales.ViewOwn
- Sales.ViewAll
- Sales.ApplyDiscount
- Sales.OverridePrice
- Sales.Suspend
- Sales.Refund
- Sales.Void
- Products.View
- Products.Create
- Products.Edit
- Products.ChangePrice
- Inventory.View
- Inventory.Adjust
- Customers.Create
- Customers.Edit
- Reports.ViewOwn
- Reports.ViewAll
- Expenses.Create
- Excel.Import
- Excel.Export

High-risk permissions such as Users.Manage, Permissions.Manage, AuditLogs.View, Backup.Restore, and critical application security settings must remain Manager-only.

### Manager Approval Workflow

If an Employee attempts a restricted operation such as:

- High-value discount
- Price override
- Refund
- Void completed sale
- Sensitive stock adjustment

Display an approval dialog.

Require authorization from an active Manager account.

Record:

- Employee who requested the operation
- Manager who approved it
- Operation type
- Affected record
- Previous and new values
- Reason
- Timestamp

Manager approval should be recorded as a one-time authorization, not permanently increase the Employee's permissions.

## 5. MODERN DESKTOP UI/UX

Design a beautiful, professional POS desktop interface.

Visual style:

- Modern Windows desktop appearance
- Clean and minimal design
- Rounded corners
- Professional typography
- Consistent iconography
- Light and dark themes
- Smooth interactions
- Responsive layouts
- High DPI support
- Keyboard accessibility
- Reusable dialog components
- Loading indicators
- Helpful toast notifications
- Clear error and success messages
- Consistent validation feedback

Main layout:

Left sidebar navigation:

- Dashboard
- Point of Sale
- Products
- Categories
- Inventory
- Purchases
- Suppliers
- Customers
- Sales History
- Returns & Refunds
- Expenses
- Reports
- Shifts
- Employees
- Roles & Permissions
- Excel Import/Export
- Backup & Restore
- Settings

Top bar:

- Company name
- Current user
- User role
- Current date/time
- Search
- Notifications
- Theme toggle
- Profile menu
- Logout

Only display sections permitted for the logged-in user.

Support English and Arabic localization with Right-to-Left layout for Arabic.

Default currency: Egyptian Pound (EGP), configurable by Manager.

## 6. DASHBOARD

Implement a live operational dashboard.

Dashboard cards:

- Today's total sales
- Today's revenue
- Total number of transactions
- Gross profit
- Net profit (based on configured accounting assumptions)
- Total expenses
- Number of products
- Inventory value
- Low-stock product count
- Out-of-stock products
- Total registered customers
- Active employees
- Open cashier shifts
- Pending approvals

Charts:

- Daily sales chart
- Weekly sales chart
- Monthly revenue
- Monthly profit
- Best-selling products
- Sales by category
- Payment method distribution
- Employee performance

Filters:

- Today
- Yesterday
- Last 7 days
- This month
- Custom date range

Employees must see only the dashboard widgets they are authorized to access.

## 7. POINT OF SALE (POS)

This is the MAIN and most important screen.

Build a fast, keyboard-friendly cashier interface.

Layout:

Left side:

- Product search
- Barcode input
- Category filtering
- Product search results
- Product cards or compact product table

Right side:

- Current shopping cart
- Product name
- Quantity
- Unit price
- Discount
- Tax
- Line total
- Remove item
- Order subtotal
- Total discount
- Total tax
- Grand total
- Payment section

Features:

- Search by name, SKU, or barcode
- Barcode scanner support (keyboard-emulation scanners)
- Add products quickly
- Auto-focus barcode search
- Adjustable quantities
- Prevent invalid quantities
- Live total calculation
- Check available stock
- Hold/suspend a sale
- Resume suspended sales
- Clear active cart with confirmation
- Multiple discounts (subject to business rules)
- Fixed amount discount
- Percentage discount
- Per-item discounts
- Order-level discounts
- Manager approval for restricted discounts
- Automatic configurable tax calculations
- Cash payment
- Card payment recording
- Mixed/split tender payments
- Other configurable payment methods
- Amount received
- Change due
- Insufficient payment validation
- Customer selection
- Walk-in customer option
- Salesperson assignment
- Complete sale
- Print receipt
- Reprint receipt with permission
- Sales notes
- Unique invoice number

Keyboard shortcuts:

- F2: Product search
- F3: Customer search
- F4: Discount
- F5: Refresh products
- F6: Suspend sale
- F7: Resume sale
- F8: Payment
- F9: Complete sale
- F10: Print receipt
- Esc: Close dialog

Make keyboard shortcuts configurable and prevent collisions.

A completed sale must atomically:

1. Validate permissions and input.
2. Validate active shift when required.
3. Validate stock and prices.
4. Save sale header.
5. Save sale items.
6. Save payment records.
7. Update stock using stock movement records.
8. Update applicable loyalty data.
9. Save audit information.
10. Commit the database transaction.

If an operation fails, roll back the entire transaction and display a useful error.

Prevent accidental double submission and duplicate sales.

## 8. PRODUCT MANAGEMENT

Create complete CRUD operations for products.

Product fields:

- ProductId
- SKU
- Barcode
- ProductName
- Description
- CategoryId
- BrandId
- SupplierId
- CostPrice
- SellingPrice
- TaxCategory
- StockQuantity
- MinimumStock
- ReorderLevel
- UnitOfMeasure
- ProductImage
- ExpiryDate (optional)
- IsActive
- CreatedAt
- UpdatedAt

Features:

- Add product
- Edit product
- Archive product
- Search products
- Filter by category
- Filter by brand
- Filter by stock status
- Product images
- Barcode generation
- Barcode printing
- Automatic profit margin calculation
- Bulk price update (Manager only)
- Multiple units when properly configured
- Product variants such as size/color
- Product validation
- Duplicate SKU/barcode detection

Never permanently delete products linked to existing sales.

## 9. INVENTORY MANAGEMENT

Build a complete stock management module.

Features:

- Current stock
- Available stock
- Reserved stock when applicable
- Minimum stock alerts
- Low-stock notifications
- Out-of-stock warnings
- Stock adjustment
- Stock receiving
- Damaged item tracking
- Expired item tracking
- Manual stock counting
- Stock reconciliation
- Stock movement history
- Reorder suggestions
- Inventory valuation
- Stock adjustment approval
- Stock movement reason tracking

All stock changes must create an immutable stock movement record.

Record:

- ProductId
- MovementType
- QuantityChange
- BeforeQuantity
- AfterQuantity
- ReferenceType
- ReferenceId
- PerformedBy
- ApprovedBy
- Reason
- Timestamp

Do not allow inventory changes that bypass the stock movement system.

## 10. PURCHASES AND SUPPLIERS

Supplier management:

- Supplier name
- Phone
- Email
- Address
- Tax registration information (optional)
- Notes
- Status

Purchase features:

- Create purchase order
- Add purchase items
- Receive stock
- Partial receiving
- Record supplier invoices
- Purchase cost tracking
- Purchase returns
- Outstanding supplier balances
- Purchase history
- Purchase reporting
- Supplier search
- Stock update on receiving

Ensure receiving a purchase updates inventory correctly and does not count received goods twice.

## 11. CUSTOMER MANAGEMENT

Customer fields:

- CustomerId
- FullName
- Phone
- Email
- Address
- CustomerType
- LoyaltyPoints
- CreatedAt
- Notes

Features:

- Add customer
- Edit customer
- Search customer
- View purchase history
- Track total spending
- Customer-specific discounts
- Loyalty points (configurable)
- Customer balances only when credit sales are enabled
- Export customer records with permission

A walk-in customer must be supported without requiring registration.

## 12. RETURNS, REFUNDS AND VOID OPERATIONS

Support:

- Full refunds
- Partial refunds
- Item-level returns
- Exchanges
- Refund reason
- Refund to original payment type where applicable
- Manager approval
- Refund receipt
- Inventory restocking decision
- Damaged return handling
- Refund audit trail

Prevent refunding more units or money than originally sold.

Completed invoices and refunds must remain historically traceable.

Never silently delete completed sales.

Use reversal or adjustment records rather than destroying accounting history.

## 13. CASHIER SHIFTS AND CASH DRAWER

Build a cashier shift management system.

Each shift stores:

- ShiftId
- EmployeeId
- OpeningTime
- ClosingTime
- OpeningCash
- ExpectedCash
- ActualCash
- CashDifference
- ShiftStatus
- ClosingNotes

Features:

- Open shift
- Close shift
- Opening cash balance
- Cash in
- Cash out
- Paid-out reason
- Sales during shift
- Refunds during shift
- Expected cash calculation
- Counted cash
- Cash discrepancy tracking
- Manager review
- Shift report
- Shift receipt printing

Prevent conflicting open shifts according to configured business rules.

## 14. EXPENSE MANAGEMENT

Features:

- Create expense
- Expense categories
- Expense description
- Expense amount
- Expense date
- Payment method
- Employee attribution
- Manager review
- Expense search
- Expense reports

Examples:

- Electricity
- Rent
- Transportation
- Maintenance
- Supplies
- Other operating expenses

Only authorized users may create, edit, approve, or view expenses.

## 15. REPORTING AND ANALYTICS

Implement real database-driven reports.

Reports:

- Daily sales
- Weekly sales
- Monthly sales
- Custom date sales
- Sales by product
- Sales by category
- Sales by employee
- Sales by payment method
- Gross profit
- Net profit estimate
- Product profitability
- Inventory valuation
- Low-stock report
- Out-of-stock report
- Purchase report
- Supplier report
- Expense report
- Refund report
- Shift reconciliation report
- Tax summary
- Customer purchase report
- Discount report

Features:

- Date filtering
- Sorting
- Searching
- Grouping
- Totals
- Charts
- Export to Excel
- Export to PDF
- Print reports

Define accounting assumptions clearly, including inventory costing, cost of goods sold, tax treatment, and refund effects.

Employee reports must be restricted according to permissions.

## 16. EXCEL IMPORT AND EXPORT

Implement a complete Microsoft Excel integration module.

The Manager can import product data from existing Excel files.

Supported format:

- .xlsx

Example columns:

- SKU
- Barcode
- ProductName
- Category
- CostPrice
- SellingPrice
- Quantity
- MinimumStock
- Supplier
- Description

Import wizard:

Step 1: Select Excel file.

Step 2: Choose Excel worksheet.

Step 3: Preview rows.

Step 4: Map Excel columns to application fields.

Step 5: Validate imported data.

Step 6: Detect duplicate SKUs and barcodes.

Step 7: Choose how to handle duplicates.

Step 8: Import valid rows using transactions.

Step 9: Show import results.

Step 10: Export error rows for correction.

Support:

- Column mapping
- Optional columns
- Import preview
- Product creation
- Product update
- Skip duplicates
- Import validation
- Error reports
- Export inventory to Excel
- Export sales to Excel
- Export Excel import template

Treat imported quantity as an opening stock adjustment or approved inventory movement, not an unexplained overwrite.

Never silently overwrite prices or stock when importing.

## 17. RECEIPT AND INVOICE PRINTING

Support common Windows receipt printers:

- 58 mm thermal receipts
- 80 mm thermal receipts
- A4 invoices

Features:

- Select installed printer
- Save default printer
- Print preview
- Print receipt
- Reprint receipt
- Receipt logo
- Store information
- Address
- Phone
- Invoice number
- Cashier name
- Date and time
- Itemized purchased products
- Discounts
- Tax breakdown
- Total
- Amount paid
- Change
- Payment method
- Optional QR code
- Custom footer message

Support printer errors gracefully without losing completed sales.

Receipt printing must not be required for successful transaction completion.

Do not claim government tax or e-invoice compliance unless a separate verified integration is implemented.

## 18. BACKUP AND RESTORE

Manager-only backup module.

Features:

- Manual database backup
- Automatic scheduled backups while the application is running
- Configurable backup directory
- Backup history
- Backup verification
- Restore database
- Restore confirmation
- Pre-restore safety backup
- Import/export configuration
- Corrupt database handling
- Clear backup error reporting

Use a SQLite-safe backup process.

Never copy a live database in a way that risks inconsistent data.

Store backups outside the installation directory.

## 19. SYSTEM SETTINGS

Manager can configure:

General:

- Store name
- Store logo
- Address
- Phone
- Currency
- Date format
- Language
- Light/dark mode

Sales:

- Tax percentage
- Tax-inclusive/exclusive pricing
- Default discount
- Maximum employee discount
- Allow/disallow selling without stock
- Allow/disallow credit sales
- Invoice numbering rules

Printing:

- Default printer
- Receipt size
- Receipt footer
- Auto-print after sale

Security:

- Session timeout
- Password policy
- Account lockout
- Manager approval requirements
- Audit logging

Data:

- Backup directory
- Auto-backup configuration
- Database maintenance

Keep configuration persistent and validate changes.

## 20. NOTIFICATIONS

Create in-app notifications for:

- Low stock
- Out-of-stock products
- Pending Manager approvals
- Cash register discrepancies
- Database backup errors
- Import completion/errors
- Products nearing expiration
- Important application errors

Do not require internet access.

## 21. AUDIT LOGGING AND SECURITY

Audit sensitive activity:

- Login success/failure
- Logout
- User creation
- User permission modification
- Product price modification
- Inventory adjustment
- Refund
- Void
- Discount override
- Manager approval
- Database restore
- Configuration changes
- Sensitive export activity

Each audit log should contain:

- AuditId
- ActorUserId
- ActionType
- EntityType
- EntityId
- OldValues (safely redacted)
- NewValues (safely redacted)
- Timestamp
- Reason
- ApprovedBy (optional)

Security rules:

- Enforce permissions at service level.
- Validate all inputs.
- Never trust UI visibility as authorization.
- Use parameterized database queries.
- Do not store plaintext passwords.
- Do not expose password hashes in exports or logs.
- Apply principle of least privilege.
- Use transactions for critical financial operations.
- Avoid hard deletion of financial records.
- Prevent unauthorized privilege escalation.
- Require authentication again for high-risk approvals.
- Ensure audit logs are protected from ordinary editing.

## 22. DATABASE DESIGN

Use EF Core Code First with migrations.

Create normalized, properly indexed tables including:

- Users
- Roles
- Permissions
- RolePermissions
- UserPermissionOverrides
- Products
- ProductVariants
- Categories
- Brands
- Suppliers
- Customers
- Sales
- SaleItems
- Payments
- Refunds
- RefundItems
- Purchases
- PurchaseItems
- InventoryMovements
- StockAdjustments
- Expenses
- ExpenseCategories
- CashierShifts
- CashMovements
- ApprovalRequests
- ApprovalDecisions
- AuditLogs
- ApplicationSettings
- BackupHistory

Database requirements:

- Proper primary and foreign keys
- Unique indexes for usernames/SKUs where appropriate
- Decimal precision suitable for currency
- Store monetary values accurately
- Foreign key constraints
- Transactional integrity
- Consistent timestamps
- Concurrency protection
- Database migration support
- Referential integrity
- Soft deletion for appropriate business records
- Immutable financial history

Do not store money using floating-point types.

Use decimal in business calculations and appropriate exact storage representations for SQLite.

## 23. RELIABILITY AND ERROR HANDLING

The system must handle:

- Invalid user credentials
- Database connection errors
- Database locked errors
- Printer unavailable
- Excel import errors
- Duplicate barcodes
- Invalid product prices
- Out-of-stock purchases
- Insufficient payments
- Unauthorized actions
- Application shutdown during checkout
- Unexpected application crashes
- Failed backup operations
- Concurrent database update conflicts

Show understandable messages instead of crashing.

Do not accidentally complete the same sale twice.

Use appropriate logging, validation, retries, and database transaction boundaries.

## 24. TESTING

Create automated tests covering:

- Authentication
- Authorization
- Manager versus Employee permissions
- Permission overrides
- Manager approval
- Sales calculations
- Percentage/fixed discounts
- Tax calculation
- Cash change calculation
- Stock deduction
- Stock receiving
- Refund quantities
- Refund amounts
- Duplicate sale prevention
- Shift reconciliation
- Excel import validation
- Database transactions
- Unauthorized operation rejection

Include integration tests using a temporary SQLite database.

Verify that Employees cannot bypass restrictions by invoking services directly.

## 25. INSTALLATION AND DEPLOYMENT

Provide:

- Complete Visual Studio solution
- All C# source files
- All XAML files
- Project references
- NuGet dependency configuration
- Database migrations
- Database initialization
- First-run Manager setup wizard
- Sample/demo data generation option
- README
- Build commands
- Run instructions
- Publish instructions

Publish as a self-contained Windows executable when requested.

Store user-writable application data in an appropriate Windows application data folder, not inside a protected Program Files installation directory.

The production build must not depend on a pre-existing developer database.

## 26. DEVELOPMENT EXECUTION PLAN

Build the application incrementally while keeping it compilable.

Phase 1:
- Solution structure
- Dependency injection
- EF Core/SQLite configuration
- Migrations
- Login
- Manager setup
- User management
- RBAC
- Main dashboard/navigation shell

Phase 2:
- Products
- Categories
- Suppliers
- Inventory movements
- Product search
- Barcode support

Phase 3:
- Complete POS
- Checkout
- Payments
- Receipt printing
- Sales history
- Transaction management

Phase 4:
- Customers
- Purchases
- Refunds
- Expenses
- Cashier shifts

Phase 5:
- Reports
- Excel import/export
- Backup/restore
- Settings
- Notifications

Phase 6:
- UI refinement
- Validation
- Automated testing
- Error handling
- Performance improvements
- Deployment

## 27. IMPORTANT DEVELOPMENT INSTRUCTIONS

You must:

1. Generate actual working C# and XAML source code.
2. Create files within the correct project structure.
3. Implement real database persistence.
4. Implement every feature with functional services.
5. Wire all buttons, commands, and navigation.
6. Use MVVM correctly.
7. Use reusable controls and styles.
8. Enforce RBAC for every protected operation.
9. Avoid hardcoded credentials.
10. Keep critical transactions atomic.
11. Prevent inconsistent inventory and payment records.
12. Build after each completed phase.
13. Fix compiler errors before moving forward.
14. Run relevant automated tests.
15. Report tests that fail or cannot be executed.
16. Do not falsely claim features are complete if they contain placeholder implementations.
17. Preserve already working features between phases.
18. Document any known limitations.

If the scope is too large for one response, implement complete working vertical slices and continue systematically rather than generating incomplete skeletons for every module.

## FINAL EXPECTED RESULT

Deliver a complete Windows WPF Cashier/POS Management System with:

- Professional modern interface
- Manager and Employee login
- Configurable Employee permissions
- Manager approval workflows
- Functional POS sales operations
- Real SQLite database
- Product management
- Stock management
- Supplier/customer management
- Receipt printing
- Purchase and refund workflows
- Cashier shifts
- Expenses
- Reports and charts
- Excel integration
- Backup and restore
- Audit logs
- Reliable financial calculations
- Responsive and secure user experience

Begin by creating the complete solution architecture, the required projects, database entities, first migration, authentication system, authorization service, and fully functional Manager/Employee login workflow.

Then implement the remaining features in the specified phases.