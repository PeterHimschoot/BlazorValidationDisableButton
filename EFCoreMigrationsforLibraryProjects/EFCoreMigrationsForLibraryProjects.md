# EF Core Migrations for Library Projects

![EFCoreMigrations](https://u2ublogimages.blob.core.windows.net/peter/EFCoreMigrationsWithLibraryProjects.jpg)

Summary: This post is about how to enable migrations for library projects, with user-secrets. 

So why would you need this? For me, I want to keep the actual data access code out of my web project. This way my web project does not have a direct dependency on Entity Framework, and ensures that I will need to apply **Dependency Inversion**, making my code a lot easier to test.

The problem is, there is not a lot of guidance about how to do this. And the stuff you find puts the connection
string in the code, which I don't want. Doing this exposes your connection string (which you probably don't want 
to do), and makes it hard to use for automated building (which likely uses another connection).

## Let's look at an example

Assuming you have a simple `DbContext` like this, part of a library project.

``` csharp
public class CurrencyDb : DbContext
{
  public CurrencyDb(DbContextOptions<CurrencyDb> options)
    : base(options) { }

  public DbSet<Currency> Currencies { get; set; }
}
```

And you have added the right packages to your project:

``` xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="2.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="2.0.0" />
</ItemGroup>
<ItemGroup>
  <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
</ItemGroup>
```

## Enabling EF Core Migrations

So now you want to use EF Core migrations. Just follow a couple to simple steps

### Step 1

Open your project file and set the `GenerateRuntimeConfigurationFiles` to `true` (I got this one
from [Julie Lerman's excellent blog](http://thedatafarm.com/data-access/the-secret-to-running-ef-core-2-0-migrations-from-a-net-core-or-net-standard-class-library/)).
While you're at it, also add an `UserSecretsId` with some unique value (I prefer to use a guid here).

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netcoreapp2.0</TargetFramework>
    <UserSecretsId>CC9621B7-62F3-4956-BEEC-FC253E090FB0</UserSecretsId>
    <GenerateRuntimeConfigurationFiles>true</GenerateRuntimeConfigurationFiles>
  </PropertyGroup>
```

Also add dependencies for [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio):

``` xml
  <ItemGroup>
    ...
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="2.0.0" />
  </ItemGroup>
  <ItemGroup>
    ...
    <DotNetCliToolReference Include="Microsoft.Extensions.SecretManager.Tools" Version="2.0.0" />
  </ItemGroup>
```

Your project will not compile after these changes, so proceed to step 2.

## Step 2

Add the following base class to your project, or even better, to some shared project.

``` csharp
public abstract class DesignTimeDbContextFactory<T> : IDesignTimeDbContextFactory<T> 
where T : DbContext
{
  private IConfiguration Configuration {get; }
  private string ConfigKey {get;}
  
  public DesignTimeDbContextFactory(string configKey) {
    this.ConfigKey = configKey ?? throw new ArgumentNullException(nameof(configKey));
    ConfigurationBuilder cb = new ConfigurationBuilder();
    AddConfigurationSources(cb, Assembly.GetCallingAssembly());
    Configuration = cb.Build();
  }
  
  protected virtual void AddConfigurationSources(ConfigurationBuilder builder, Assembly asm) {
    builder.AddUserSecrets(asm)
           .AddEnvironmentVariables();
  }
  
  public T CreateDbContext(string[] args)
  {
    var builder = new DbContextOptionsBuilder<T>();
    builder.UseSqlServer(Configuration.GetConnectionString(ConfigKey));
    return CreateDbContext(builder.Options);
  }
  
  protected abstract T CreateDbContext(DbContextOptions<T> options);
}
```

## Step 3

Add a class deriving from `DesignTimeDbContextFactory`, for example:

``` csharp
public class CurrencyDbContextFactory : DesignTimeDbContextFactory<CurrencyDb>
{
  public CurrencyDbContextFactory() : base("CurrencyDb") {}
  
  protected override CurrencyDb CreateDbContext(DbContextOptions<CurrencyDb> options)
  => new CurrencyDb(options);
}
```

You need to give it a default constructor, which calls the base constructor with 
the name of the connection string (more about that in the next step).

Your project should compile. One more step to go...

## Step 4

In the constructor you specify the name of the connection string. But where will your library
get the connection string? During development, I like to store connection strings and other confidential stuff
in **user-secrets**. That is why you needed to add this dependency in your project in step 1.

If you like, you can also opt to store the connection string in other places, such as environment variables, 
or any preferred configuration by overriding the `AddConfigurationSources` method in your `DbContextFactory`.

So now you need to save the connection string in your preferred configuratin source, for example:

`secrets.json`
``` json
{
  "ConnectionStrings" : {
    "CurrencyDb" : "ConnectionstringToCurrencyDb"
  }
}
```

**Tip** If you have multiple library projects, you can use the same `UserSecretsId` if you want to 
store all the connection strings in the same place.

## Adding a migration and updating the database

Ok, so now you are ready to add a migration. The steps are the usual, but for your convenience I'm going
to repeat them again.

First make sure your project build without compile-time errors.

``` cli
dotnet build
```

Now you can add your migration, for example:

``` cli
dotnet ef migrations add <<YoUrMiGraTiOnNaMe>>
```

And to create or update the database

``` cli
dotnet ef database update
```

That's it!

