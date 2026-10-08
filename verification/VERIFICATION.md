# Verification — CashierPOS 0.1

Final checks performed 2026-10-08 UTC using .NET SDK 10.0.401 / runtime 10.0.12 on Ubuntu x64.

| Check | Result |
|---|---|
| Full solution Release build, including WPF XAML | PASS: zero warnings, zero errors |
| Automated real-SQLite service tests | PASS: 38 passed, 0 failed, 0 skipped |
| Self-contained Windows x64 publish | PASS |
| GUI execution on Windows | NOT RUN: this environment is Linux |
| Printer drivers, physical paper, Arabic glyphs, DPI | NOT RUN |

The TRX result and final build/test/publish logs are included. Tests cover issued session identities and forged roles/actors; failed-login lockout; last-Manager protection; account revocation; employee permission overrides; operation-bound, expiring and one-time approvals; cost projection privacy; atomic checkout and rollback after database writes; duplicate request payloads; price changes; mixed tenders/change; shift balances; order receiving; cumulative refund rounding/limits/returned cost snapshots; damaged-return accounting; employee report scope; Excel and backup/restore.

## Windows acceptance before use in a store

1. Extract the complete archive and launch Start-CashierPOS.cmd on Windows x64. Confirm the first-run Manager creation screen appears with no default credentials.
2. Create a Manager, a second Manager for recovery, and an Employee. Check inactive and reset accounts cannot use their previous sessions. Verify the last active Manager cannot be disabled.
3. Add a product, create opening stock, open a shift, scan a known barcode and sell with both cash and split cash/card. Check invoice total, change, stock and expected drawer cash.
4. Switch to Employee: verify costs/profits and administration are inaccessible. Change a grant from the Manager screen and check the service result on Employee's next action.
5. Request a restricted discount or price override. Approve using the Manager dialog, and inspect the audit log. Reject/cancel approval and verify no sale was saved.
6. Return one item, attempt an excess return, and test a damaged nonrestocked return. Verify the refund tender record and cash drawer effects against actual money returned.
7. Receive a purchase in two partial deliveries; try an excessive third delivery and verify it is rejected.
8. Import the template, preserve leading zero barcodes, exercise mapping/duplicates/invalid rows and confirm Quantity ADDS stock. Inspect the exported workbook in Excel.
9. Make a backup, change stock, close the shift, restore, and verify the pre-restore snapshot, sign-out and recovered values. Configure a short backup interval in a Manager session and verify a scheduled backup is created.
10. Switch Arabic/light/dark, test the 1366×768 layout and Windows DPI, editable cart quantities, F2/F4/F5/F6/F7/F8/F9, manual and idle locking. Compare receipt preview with 58/80 mm paper and A4; test an offline printer and Microsoft Print to PDF.

The application's functional scope and remaining features are documented in FEATURES.md. Passing service tests and cross-compilation do not replace these Windows/hardware acceptance checks.
