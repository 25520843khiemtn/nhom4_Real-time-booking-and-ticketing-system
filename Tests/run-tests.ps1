$ErrorActionPreference = 'Stop'
# Pass additional dotnet test arguments, for example: --filter Category!=KnownDefect
& dotnet test (Join-Path $PSScriptRoot 'TicketBooking.Tests.sln') --configuration Release --results-directory (Join-Path $PSScriptRoot 'TestResults') --logger 'trx;LogFilePrefix=tests' @args
exit $LASTEXITCODE
