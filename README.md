# Payroll

Payroll is an ASP.NET Core application that performs weekly time and attendance calculations using SQL and Quick Base (SaaS) as
data sources.  

This application is currently in development.

## Get Started

1. Restore nuget packages
1. Update the local instance of MSSQLLocalDB for PrimaCompany.IDP by opening the Package Manager Console, setting the default project to PrimaCompany.IDP and executing the following command:
	* `Update-Database -Context ConfigurationDbContext`
	* `Update-Database -Context PersistedGrandDbContext`
	* `Update-Database -Context IdentityDbContext`
1. Update the local instance of MSSQLLocalDB for Payroll.Data by opening the Package Manager Console, setting the default project to Payroll.data and typing `Update-Database`
1. Add required app secrets using `dotnet user-secrets set "<Key>" "<Value>" --project payroll`

### Secrets
| Key | Description |
|-----|-------------|
| QuickBase:UserToken | User token with access to the Payroll application |
| QuickBase:Realm | The subdomain name used to access Quick Base e.g. "gerawan" |
