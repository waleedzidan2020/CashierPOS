# Implemented scope and remaining work

This is release 0.1. There are no navigation buttons for modules with empty business implementations. The table compares this release with the supplied development prompt.

| Area | Working in this release | Still outside this release |
|---|---|---|
| Architecture | Five-project solution, MVVM commands, DI, logging, EF Core SQLite, generated migrations, real persistence | SQL Server adapter, networked terminals |
| Accounts | First Manager setup, username/password login, visibility toggle, remember username, temporary lockout, enable/disable, reset/change password, logout, idle/manual lock, last-Manager safeguard | Employee PIN, email/phone profile and configurable password/lockout policy |
| Authorization | Seeded permissions, Manager-only operations, Employee overrides, fresh service checks, protected DTOs, single-use approvals bound to operation and requester | Separate pending-request inbox and asynchronous review workflow |
| UI | Sidebar/top bar, Arabic RTL navigation and bilingual forms, dark/light resources, keyboard POS actions, status/error feedback, virtualized tables | Comprehensive localization of every technical column, custom notifications/toasts, icon set, configurable shortcut editor, Windows visual/DPI acceptance |
| Dashboard | Database-driven sales/refunds/revenue/tax/profit/low-stock cards and last-seven-day revenue bars; date ranges, own/all report scope | Other requested graphs/widgets; maintained third-party chart component |
| POS | Name/SKU/barcode search, keyboard-emulation scanner, quantities, fixed/percentage order discount, explicit price override, cash/card/split tender, change, walk-in/customer ID, idempotent atomic checkout, hold/resume, invoice number | Per-item discounts, customer picker, other payment types, sales attribution reassignment, notes editor, configurable shortcut collisions |
| Products | Create/edit/archive, SKU/barcode uniqueness, prices/costs, minimum stock, service-supported category/supplier relationships, cost access control | Image gallery, brands, variant and multi-unit editors, barcode generation/label printing, bulk price change, reactivation UI |
| Categories | List and add unique category | Edit/archive, product-category selection UI |
| Inventory | Balances, immutable-by-service movements, adjustments with reason/approval, low-stock count, nonnegative stock constraints | Expiry/batch lots, reservation subsystem, dedicated stocktaking/damaged/expiry screens, reorder forecasts |
| Suppliers | Create/edit/list name, phone, notes | Tax/contact address fields and supplier balances |
| Purchases | Orders, supplier reference, line costs, partial stock receipt, over-receiving prevention, weighted average costs | UI creates one-line orders; service supports multiple lines. Purchase returns, payments/balances and detailed purchase reports |
| Customers | Create/edit/list, walk-in checkout and optional customer ID | Credit, balances, loyalty, customer discounts and dedicated history/export views |
| Refunds | Item-level partial/full-by-items returns, quantity/payment caps, original tender records, optional restock, reason, approvals, audit | Dedicated refund receipt, exchange screen, completed-sale void workflow, payment gateway interaction |
| Shifts | Single drawer, opening/closing/count/variance, signed cash in/out, reason, shift listing/printing | Multiple drawers, delegated manager closure and review states |
| Expenses | Create/list category, description, amount, actor and date; profit reporting | Editing, category management, payment method and explicit review/approval state |
| Reports | Date-filtered sales and operational totals, gross/net profit estimate, refunds/tax/expenses included, own/all scope, Excel and Windows print/PDF | Full set of requested grouped reports, dedicated inventory valuation, static PDF export independent of Windows print |
| Excel | Worksheet selection, required/optional column mapping, preview, skip/update policy, whole-file validation/transaction, additive stock movements, template/export, text error list | Error-row workbook export, category/supplier auto-creation, per-row success import after invalid rows |
| Printing | Preview and Windows printer chooser, 58/80 mm/A4 sale receipt, store/contact/footer, sales/shift report print, PDF through Windows printer | Persisted printer/format defaults, logo/QR, automatic silent printing, physical printer validation |
| Backup | SQLite backup API, exact migration/integrity/FK validation, manual backup, restore safety snapshot/session invalidation, configurable scheduled backup while app runs in a Manager session | Backups while only Employees are signed in or app is closed, retention/encryption, configuration-only exports |
| Settings | Store/contact/currency/language/theme/tax/discount threshold/idle timeout/footer/backup directory and interval | Credit/negative-stock toggles, tax-category rules, invoice numbering editor, maintenance and security-policy editor |
| Audit | Login events, account/permission changes, product changes, stock adjustments, sale/refund/approval/shift/expense/import/export/settings/backup events | Separate old/new structured JSON on every log, cryptographic tamper evidence and retention policy |
| Tests | Real SQLite integration, security/forgery/overrides/approval, checkout/rollback/idempotency, tax rounding, refunds, receiving, shifts, Excel, restore | Interactive WPF automation and hardware acceptance |

Useful next increments: Windows acceptance and visual refinement; multi-line purchase editor; product categorization/variants; dedicated return receipts; complete report grouping; stocktake/expiry; customer loyalty/credit and the associated accounting rules. Retain the tested financial and authorization invariants when extending them.
