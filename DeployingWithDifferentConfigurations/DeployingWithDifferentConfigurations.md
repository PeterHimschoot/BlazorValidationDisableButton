# Release management with different configurations

So **Visual Studio Team Services** now has this automatic deployment option, so you can checkin your changes in source control, have the code deployed to a development/testing/production environment automatically. But how do you cope with different environment's configuration? Read on...

## What we want

As you are developing locally you probably want to develop on a local database. The easiest way to do this is to have everything in `web.config` (I am using a web project as an example here, but things are similar for other project types). This way developers can quickly change things locally, for easy testing with different databases, and even see how things work with live (I hope cloned from production) data.

But when deploying to a test environment (and other environments such as production) you want to use a different `web.config`, preferably **NOT** containing any production secrets (like the connection string to the production database). 

VSTS release management makes this very easy.

Let's say that I have application settings and connection strings that I want to give a different value in development, qa and release builds:

```
  <appSettings>
    <add key="Secret" value="DeveloperSecret"/>
  </appSettings>
  <connectionStrings>
    <add name="DB" connectionString="DeveloperDB"/>
  </connectionStrings>
```

I have already created a Build definition in VSTS, and I am using it to initiate a release with VSTS release management.

In my release definition I have three environments, one for dev, QA and production.

For each environment, click on the ellipsis (...) button and choose *Configure variable*

**Insert**

Add variables who's key matches the key from `appSettings` (or name for `connectionStrings`)

**Insert**

Select your deploy to azure task in release management

Look for the **File Transforms & Variable Substitution Options** section and make sure you check the **XML variable substitution** checkbox.

This will make the deployment task replace any key in `appSettings` and `connectionStrings` with your variable values.








