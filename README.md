# CashierPOS 0.1

Functional offline Windows WPF POS with a real SQLite database, Manager/Employee authentication, permission checks inside application services, one-time Manager approval, stock movements, transactional checkout, refunds and cash shifts. This release implements working vertical slices of the supplied specification; it is not the complete production system described in that specification. See FEATURES.md for the exact boundaries.

## Run

Extract the entire ZIP, then run `Start-CashierPOS.cmd` or `Portable/CashierPOS.WPF.exe`. The portable release targets Windows x64 and contains .NET; it does not need an SDK. Create your Manager account at first launch. There is no default password. Add products, receive/adjust opening stock, open a shift, then sell. The app uses one cash drawer and permits only one open shift, owned by its cashier. Manager privileges do not automatically let a manager sell in another employee's shift.

Source projects: Core → Application; Infrastructure implements persistence/file adapters; WPF composes services and owns only UI/printing concerns. Tests exercise actual SQLite databases. Critical business operations do not depend on XAML visibility or code-behind. Five projects, generated EF migrations and a model snapshot are included.

## Build / test / publish

Install a .NET 10 SDK. Use the terminal in this directory:

```powershell
dotnet restore CashierPOS.sln
dotnet build CashierPOS.sln -c Release -m:1
dotnet test CashierPOS.Tests/CashierPOS.Tests.csproj -c Release -m:1
dotnet run --project CashierPOS.WPF
dotnet publish CashierPOS.WPF -c Release -r win-x64 --self-contained true -p:PublishTrimmed=false -o Portable
```

NuGet is required only when building/installing dependencies, not for normal operation. Never replace your production database with a developer database. Startup calls Migrate, then seeds permission/role metadata and settings; it never creates a user or password automatically. Do not run two app versions against the same file. The Windows process mutex prevents a second instance per Windows session.

The generated migrations are under Infrastructure/Migrations. For future schema changes:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.12
dotnet ef migrations add YourChange --project CashierPOS.Infrastructure
```

## Data and accounting

Data: `%LOCALAPPDATA%\CashierPOS\pos.db`; log: `Logs/app.log`; remembered username: `username.txt`. This is a single Windows-user, single-terminal application. If different Windows accounts launch it, they get different stores. Back up before replacing the executable with an upgraded release. Restore only accepts the exact migration history for this release, verifies SQLite integrity and foreign keys, creates a safety backup, and invalidates all sessions.

Amounts use decimal calculations and integer hundredths in SQLite. Currency is a display label; calculations assume two decimal places. Quantities are positive whole items. Discounts are allocated proportionately to lines, with remaining cents distributed deterministically. Tax is rounded on each discounted line, away from zero. Inclusive tax is extracted from the discounted price; exclusive tax is added. No rounding uses binary floating-point money. Percentage entry converts to a rounded fixed discount before checkout.

Checkout validates current catalog prices and stock in a database transaction. Receipt items carry price/tax/discount/cost snapshots. Every stock change creates a movement. A unique request ID and payload hash make retries idempotent and reject reuse for different data. Negative stock is disallowed. Suspended carts do not reserve stock and are repriced at resume; suspended discounts/tenders are not retained. Normal checkout rejects a stale expected price instead of silently charging a changed catalog price. Price overrides are explicit and require permission/approval.

Purchase receiving uses a rounded weighted average per-unit cost. COGS on a sale uses the product cost snapshot at checkout, not a later cost. Restockable refunds reverse the corresponding COGS; damaged/nonrestocked refunds do not. Partial refund allocations use cumulative rounding and settle the final residual exactly. Refund tender recording fills original payment balances in payment record order, never beyond each original payment. This records a refund; it does not execute card-processing refunds.

Reported net revenue excludes tax and refunds; gross profit subtracts COGS; net profit estimate subtracts recorded operating expenses. Cash-drawer expectation is opening cash + net cash sales − cash refunds + signed cash movements. Expenses do not automatically withdraw drawer cash: record a separate cash-out when applicable. Reports are operational estimates, not a general ledger or statutory accounts. Dates in the database are UTC; screen/report day boundaries use the computer's local time. Sales reports count sales and refund events in their own event periods. Purchase-order balances, receivables and credit sales are not implemented.

## Security boundaries

PBKDF2-SHA256 passwords use random 16-byte salts, 600,000 iterations and constant-time comparison. Passwords/hashes are never exported. Five failures temporarily lock an account for five minutes. Session tokens are issued in memory by authentication; a forged user ID or role flag does not grant access. Disabling/resetting users increments their session version. Permission overrides are checked freshly from the database. Manager-only grants cannot be assigned to Employees. Session idle locking is handled by WPF input activity.

Approval tokens bind requester, permission and a hash of the exact operation, expire after two minutes, and are consumed inside the same transaction as the protected operation. A rolled-back operation does not consume its approval. Approval does not change the employee's permissions. Audit logs record protected operations and actors. Local authorization controls application operations; it does not make the SQLite file tamper-proof against someone who can modify the Windows user's files. Database/backups are not encrypted in this release. No remote server or payment gateway is used.

## Verification

See verification/VERIFICATION.md and the TRX report. Core/Application/Infrastructure and WPF compilation were performed using the .NET SDK on Linux; integration tests run against real SQLite files. A Windows executable was cross-published. Interactive WPF behavior, Windows printer drivers, DPI scaling and physical receipt paper require the included Windows acceptance checks before store use. Build success is not a claim that every original requested feature is complete.

## Auto-update installation (GitHub Releases)

The existing `Portable/CashierPOS.WPF.exe` remains a **manual portable build**. For automatic updates, install **Setup.exe** once from the latest published release at <https://github.com/waleedzidan2020/CashierPOS/releases/latest>. Then use the installed Start-menu shortcut, not the old Portable shortcut.

On every push to `main`, GitHub Actions first builds/tests and then packages a new Windows self-contained release using Velopack. The installed app checks GitHub Releases on startup and every 15 minutes; it automatically downloads new packages, and asks before restarting **only when there is no active cart or operation**. If the cashier is busy or declines to restart, the update is applied on the next launch. Network failure does not prevent sales or offline use. No self-hosted runner or personal access token on the cashier PC is needed because this repository is public.

The application continues to store SQLite data, logs, and backups outside the installation in `%LOCALAPPDATA%\\CashierPOS`. Create a backup before deploying schema-changing versions; auto-update is not a replacement for a database migration/rollback plan. The first Velopack Setup.exe installation is necessary: the previously published raw exe cannot self-update retroactively.
