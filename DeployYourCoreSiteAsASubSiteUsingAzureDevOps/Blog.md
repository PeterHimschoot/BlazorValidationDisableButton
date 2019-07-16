#

What if you have a website already built with the ASP.NET Framework, and you want to add a couple of pages using ASP.NET Core?
In that case you can deploy your new Core web site as a subsite of the existing web site. Of course you want to automate this with Azure DevOps!

## Add a Subsite to Your Existing Azure Site

This step is pretty easy. Open the [Azure Portal](https://portal.azure.com), go to **App Services** and select your existing web site. Now select **Application Settings** and scroll down to **Virtual applications and directories**. Click the **Add a new virtual application or directory** link and enter the url for your subsite and choose a physical path.

![Add a new virtual application or directory](https://u2ublogimages.blob.core.windows.net/peter/AzureSubSite.PNG)

## Add a Release Pipeline to Azure DevOps

For the next step create a **Release Pipeline** in **Azure DevOps**. Select the **Azure App Service deployment** template and click **Apply**. 

Configure the stage with your Azure subscription and App service name. Select the **Deploy Azure App Service** step and type your virtual application's name in the **Virtual application** textbox.

![Virtual application field](https://u2ublogimages.blob.core.windows.net/peter/AzureDevOps.PNG)

Don't forget to select your build artifact. Create a new release and see your subsite appear!

As an example I have deployed a .NET Framework 4.7 site [here](https://deploycoreassubsite.azurewebsites.net) and a .NET Core subsite [here](https://deploycoreassubsite.azurewebsites.net/subsite).

## Bonus

This also works great for [Blazor](https://blazor.net) sites!