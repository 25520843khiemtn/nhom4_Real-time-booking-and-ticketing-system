# Transport tests

Run `dotnet test Tests/TransportTests/TransportTests.csproj --logger trx` with a working .NET 8+ SDK and .NET 8 runtime. No database, production configuration, or existing certificate is needed.

This xUnit net8.0 project links unchanged production sources because the shipped projects are .NET Framework 4.7.2 WinForms. It does **not** verify the shipped net472 binaries or UI. Real Newtonsoft.Json, BCrypt and System.Data.SqlClient NuGet dependencies are restored; SQL source is compiled only because TcpServer references it, never instantiated in tests. No behavior-replacing mocks are used.

Coverage: request correlation, invalid envelopes/base64, register/login/session identity, anonymous authorization, actual TCP connection management/broadcast/disposal, atomic backend counters, binary bidirectional proxying (40 KB), stale-health connection failover, no-backend disconnect, actual TLS 1.2 TcpServer PING exchange, and PeerSyncManager TLS serialization. Ports are allocated by the OS (0); reflection only discovers the private listener's bound port. Failover reserves a non-listening socket and deliberately restores stale IsAlive state after the initial health check. Network awaits are bounded to 8 seconds and fixtures close sockets/cancel workers in cleanup. Tests generate short-lived self-signed certificates; no production secrets/configuration are read.

## Verification

Linux `/usr/bin/dotnet` was unusable (missing `/usr/lib/dotnet/host/fxr`). Executed a source-only copy in Windows temp using `C:\Program Files\dotnet\dotnet.exe` SDK 10.0.401, targeting net8.0. Command in that copy:

```
dotnet test Tests/TransportTests/TransportTests.csproj --logger "trx;LogFileName=transport.trx"
dotnet test Tests/TransportTests/TransportTests.csproj --no-build --logger "trx;LogFileName=transport-repeat.trx"
```

Both final runs: **29 total, 21 passed, 8 failed, 0 skipped**, exit 1. All 8 failures are active `Category=KnownDefect` cases (not excluded by default):

- Anonymous forged `UserId` authorizes HOLD_SEAT, CANCEL_HOLD, BUY_TICKET and GET_HISTORY: production falls back from session identity to request identity.
- Malformed typed Data for LOGIN, HOLD_SEAT and GET_SEATS throws JsonSerializationException instead of returning a correlated ERROR; the envelope try/catch does not cover routing/data conversion.
- Anonymous SERVER_SYNC_SEAT emits a remote-seat event without authentication.

No production fixes were made. Existing linked SQL source emits CS0618 deprecation warnings with System.Data.SqlClient 4.9.0. Initial fixture portability issues (IPv4-mapped endpoint strings and Windows Schannel ephemeral private keys) were corrected in test code and re-executed successfully. Peer heartbeat interval/recovery, stalled TLS peers, SQL behavior, TLS shutdown before session registration, and load-balancer scheduling under heavy concurrency are not covered.
