# Payroll

Payroll is an ASP.NET Core application that performs weekly time and attendance calculations using SQL and Quick Base (SaaS) as
data sources.  

This application is currently in development.

## Get Started

1. Restore nuget packages
2. Add required app secrets using `dotnet user-secrets set "<Key>" "<Value>" --project payroll`

### Secrets
| Key | Description |
|-----|-------------|
| QuickBase:UserToken | User token with access to the Payroll application |
| QuickBase:Realm | The subdomain name used to access Quick Base e.g. "gerawan" |
