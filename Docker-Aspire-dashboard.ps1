docker run -d --name Aspire-dashboard `
  -p 18888:18888 `
  -p 4317:18889 -d `
  -e 'DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true' `
  'mcr.microsoft.com/dotnet/aspire-dashboard:latest'