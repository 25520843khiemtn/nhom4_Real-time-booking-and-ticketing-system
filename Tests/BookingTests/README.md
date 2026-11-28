# Booking unit and regression tests

Standalone `net8.0` xUnit project, following the test-project pattern in
`/mnt/q/DoAnNT106/Tests/ChessTests/ChessTests.csproj`. Production source is linked
at compile time, not copied: `ProtocolModels`, `IBookingRepository`,
`InMemoryBookingRepository`, `BookingService`. No database, WinForms, server
process, or credentials are required.

Package versions: xUnit 2.9.2, runner 2.8.2 and Test SDK 17.12.0 from the reference
project; BCrypt.Net-Next 4.2.0 and Newtonsoft.Json 13.0.4 from production
`TicketBooking.Server/packages.config`. Nullable analysis is disabled because the
linked legacy production source is not nullable-annotated.

## Run all tests (including active defect regressions)

```sh
dotnet test Tests/BookingTests/BookingTests.csproj --logger 'trx;LogFileName=booking.trx'
```

On this WSL host `/usr/bin/dotnet` is broken (missing `/usr/lib/dotnet/host/fxr`).
The real verification used the installed Windows SDK 10.0.401 and runtime 8.0.31:

```sh
'/mnt/c/Program Files/dotnet/dotnet.exe' test Tests/BookingTests/BookingTests.csproj --logger 'trx;LogFileName=booking.trx' --verbosity minimal
```

Verified result: **49 total, 45 passed, 4 failed, 0 skipped**, exit code 1.
Compilation completed without compiler/analyzer warnings. Generated build outputs
and `TestResults/booking.trx` are ignored locally.

## Coverage

- Seeded and registered authentication, duplicate registration preserving password.
- Catalog filtering, snapshot isolation, hold exclusivity and cancellation ownership.
- Atomic purchase validation, failed expiry purchase, transaction pricing/details,
  history isolation/order, sold-seat protection, disconnect release.
- Immediate expiry with holdSeconds 0/-1 (no sleeps), reclaiming expired holds.
- Concurrent holds and purchases with exactly one winner per repository.
- Service forwarding, input normalization/deduplication, failed purchases,
  local/remote event separation, disconnect events, cancellable timeout watcher.
- UTF-8 Base64, invalid Base64, request IDs, JSON round trips, payload defaults,
  routing/correlation, legacy/multiple seat payloads, numeric status compatibility.

## Confirmed defects (tests remain active)

All four tests in `KnownDefectTests` carry `Category=KnownDefect`. No exclusions or
skips are configured and no production fixes have been made.

1. Blank/null-only seat entries normalize to an empty list, but `BuyTickets`
   reports success and creates a zero-ticket transaction. It should reject before
   creating history or emitting sale events.
2. `ExecuteBuyTransaction` directly accepts an empty list and records an empty
   transaction instead of rejecting it.
3. Direct repository purchases accept duplicate/case-variant IDs (`A1`, `a1`),
   creating two details and charging twice for the same physical seat. The service
   deduplicates its own path, but the repository boundary does not protect itself.
4. Two independent in-memory services can both hold the same seat even after
   the first service's event is delivered through `ApplyRemoteSeatSync`. That method
   only raises a UI notification; it does not synchronize repository state or
   provide cluster-wide exclusivity. The remote-notification test documents current
   local behavior; the KnownDefect regression asserts the required cluster invariant.

These are existing-code regression tests, not production implementation work.
The red defect tests are intentionally preserved pending an authorized fix.
