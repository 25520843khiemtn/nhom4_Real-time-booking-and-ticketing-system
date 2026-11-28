# Verification

Executed against current production sources on branch `feat/server_testqa`, 2026-11-28. Production code was not modified.

Environment: Windows SDK 10.0.401, .NET runtime 8.0.31. Native WSL dotnet fails with missing `/usr/lib/dotnet/host/fxr`; bash runner falls back to Windows SDK and translates its absolute path with wslpath.

## Full run (Release)

    '/mnt/c/Program Files/dotnet/dotnet.exe' test Tests/TicketBooking.Tests.sln -c Release --results-directory Tests/TestResults --logger 'trx;LogFilePrefix=tests' --verbosity minimal

Exit code 1: 78 tests, 66 passed, 12 failed, 0 skipped.

- BookingTests: 49 total, 45 passed, 4 failed.
- TransportTests: 29 total, 21 passed, 8 failed.
- Existing SQL source emits CS0618 warnings under the test-only System.Data.SqlClient package. No SQL integration was executed.

TRX evidence (ignored build artifacts):

- `TestResults/tests_net8.0_20261128232501.trx` (transport)
- `TestResults/tests_net8.0_20261128232502.trx` (booking)

All 12 failures are active intended-behavior tests tagged `Category=KnownDefect`:

1. Blank-only seat list creates a zero-ticket transaction through BookingService.
2. Empty seat list creates a transaction through the RAM repository.
3. Duplicate case-variant IDs charge twice for one seat at repository boundary.
4. Remote sync does not prevent a second RAM node holding the same seat.
5–8. Anonymous forged UserId authorizes HOLD_SEAT, CANCEL_HOLD, BUY_TICKET and GET_HISTORY.
9–11. Wrong typed Data throws rather than returning correlated ERROR for HOLD_SEAT, LOGIN and GET_SEATS.
12. Anonymous SERVER_SYNC_SEAT emits a remote event.

These failures are not skipped/inverted, and the default runner does not filter them out.

## Baseline rerun

    bash Tests/run-tests.sh --no-build --filter 'Category!=KnownDefect' --verbosity minimal

Exit code 0: 66 passed, 0 failed, 0 skipped (45 booking + 21 transport). This is explicitly a filtered baseline, not a clean full-suite result.

TRX: `TestResults/tests_net8.0_20261128232547.trx`, `TestResults/tests_net8.0_20261128232553.trx`.

Verified behaviors include seat ownership/expiry, atomic RAM purchase validation, contention, events, protocol serialization, real connection management, 40 KB bidirectional TCP forwarding, failover, TLS 1.2 PING and peer payload transmission with generated test certificates. Test helper reflection is used only to discover private listeners' ephemeral ports.

Shell syntax check and `git diff --check` passed. Linked-source net8 tests do not verify original net472 binaries, WinForms UI or SQL Server locking. See README for remaining gates.
