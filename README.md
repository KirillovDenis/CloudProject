# CloudProject
This project is created for the educational purposes.
## Getting Started
First of all you need to install SqlServer and change connectionString in CloudProject/Web.config file.

```
 <connectionStrings>
        <add name="CloudProjectContext8" connectionString="Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\CloudProject8-ctx.mdf;Initial Catalog=CloudProject8-ctx;Integrated Security=True;User Instance=True" providerName="System.Data.SqlClient" />
  </connectionStrings>
```
Then you shoud to start Scheduler, Worker1, Worker2, CloudProject projects.
After that go to [localhost/CloudProject](http://localhost/CloudProject).
