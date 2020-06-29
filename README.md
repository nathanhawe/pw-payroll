# Payroll

Payroll is an ASP.NET Core application that performs weekly time and attendance calculations using SQL and Quick Base (SaaS) as
data sources.  

This application is currently in development.

## Get Started

1. Restore nuget packages
1. Update the local instance of MSSQLLocalDB for PrimaCompany.IDP by opening the Package Manager Console, setting the default project to PrimaCompany.IDP and executing the following command:
	* `Update-Database -Context ConfigurationDbContext -StartupProject PrimaCompany.IDP`
	* `Update-Database -Context PersistedGrantDbContext -StartupProject PrimaCompany.IDP`
	* `Update-Database -Context IdentityDbContext -StartupProject PrimaCompany.IDP`
1. Update the local instance of MSSQLLocalDB for Payroll.Data by opening the Package Manager Console, setting the default project to Payroll.data and typing `Update-Database -StartupProject Payroll`.
1. Add required app secrets using `dotnet user-secrets set "<Key>" "<Value>" --project payroll`
1. Ensure that both PrimaCompany.IDP and Payroll are set as startup projects by right-clicking the solution, choosing "Set Startup Projects", and setting multiple startup projects.

### Secrets
| Key | Description |
|-----|-------------|
| QuickBase:UserToken | User token with access to the Payroll application |
| QuickBase:Realm | The subdomain name used to access Quick Base e.g. "gerawan" |
