#!/usr/bin/env bash
set -euo pipefail
root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
# Prefer a working native SDK; WSL may have a broken Linux dotnet install.
if dotnet --list-sdks >/dev/null 2>&1; then
  runner=(dotnet)
elif [[ -x '/mnt/c/Program Files/dotnet/dotnet.exe' ]]; then
  runner=('/mnt/c/Program Files/dotnet/dotnet.exe')
  root="$(wslpath -w "$root")"
else
  printf 'A working .NET SDK and .NET 8 runtime are required.\n' >&2
  exit 127
fi
# No default exclusion: defect regressions must remain visible.
"${runner[@]}" test "$root/TicketBooking.Tests.sln" --configuration Release \
  --results-directory "$root/TestResults" --logger 'trx;LogFilePrefix=tests' "$@"
