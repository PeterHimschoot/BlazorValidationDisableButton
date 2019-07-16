# Debug NuGet Packages with Source Link

Using NuGet packages in your project is very practical, until you need to debug... With **SourceLink** you can now step through NuGet packages with ease!

![SourceLink](https://u2ublogimages.blob.core.windows.net/peter/SourceLink.gif)

## What is Source Link?

Source Link is a [NuGet package](https://www.nuget.org/packages/Microsoft.SourceLink.GitHub) that updates your pdb files to point to sources in source control instead of your disk. 
While you are debugging Visual Studio will download these files on the fly allowing you to step into the sources of the nuget package.
This makes a lot of sense for open source packages because the sources are easy to grab.
Current source control repositories supported are **GitHub**, **Azure DevOps aka VSTS**, **TFS** and **GitLab**.
You can find the specs [here](https://github.com/dotnet/designs/blob/master/accepted/diagnostics/source-link.md#source-link-file-specification).

Source Link does require that you have at least .NET Core 2.1.300.

## Enabling Source Link for Your Own Packages

To enable SourceLink for your own package is very easy, you just need to set some properties and add the SourceLink NuGet package for your source control provider.

```
<Project>
  <PropertyGroup>
    <!-- Optional: Include PDB in the built .nupkg -->
    <AllowedOutputExtensionsInPackageBuildOutputFolder>$(AllowedOutputExtensionsInPackageBuildOutputFolder);.pdb</AllowedOutputExtensionsInPackageBuildOutputFolder>

    <!-- Optional: Declare that the Repository URL can be published to NuSpec -->
    <PublishRepositoryUrl>true</PublishRepositoryUrl>

    <!-- Optional: Embed source files that are not tracked by the source control manager to the PDB -->
    <!-- This is useful if you generate files during the build -->
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
  </PropertyGroup>

  <ItemGroup>
    <!-- Required if your repository is on GitHub -->
    <PackageReference Include="Microsoft.SourceLink.GitHub" Version="1.0.0-*" PrivateAssets="All"/>

    <!-- Required if your repository is on VSTS -->
    <!--<PackageReference Include="Microsoft.SourceLink.Vsts.Git" Version="1.0.0-*" PrivateAssets="All"/>-->

    <!-- Required if your repository is on GitLab -->
    <!--<PackageReference Include="Microsoft.SourceLink.GitLab" Version="1.0.0-*" PrivateAssets="All"/>-->
  </ItemGroup>
</Project>
```

## Using Visual Studio to Debug

To debug with Visual Studio 2017 (at least version 15.3) you will need to enable **Source Link** and disable **Just My Code**:

![Debug Settings](https://u2ublogimages.blob.core.windows.net/peter/VSSourceLink.PNG)

Now you should be able to step into the package sources! The animated GIF at the top of this blog post illustrates me stepping into a NuGet package...

## I Can't get SourceLink to Work with my Package!

What if you're trying to add Source Link to your package and it just doesn't work?! 
While I was preparing this blog post I had an issue. 
Luckily I found this **dotnet global tool** called `sourceLink`, which allows you to examine the pdb file.

You can install it by typing this on your command line (assuming you have .net core 2.1.300 or later):

```
dotnet tool install --global sourcelink
```

Then you can see all available commands with

```
dotnet sourceLink -h
```

```
Source Code On Demand

Usage:  [options] [command]

Options:
  -h|--help  Show help information

Commands:
  print-documents  print the documents stored in the pdb or dll
  print-json       print the Source Link JSON stored in the pdb or dll
  print-urls       print the URLs for each document based on the Source Link JSON
  test             test each URL and verify that the checksums match

Use " [command] --help" for more information about a command.
```

Start by printing the embedded url's with `sourcelink print-urls <<path-to-pdb>>`.

```
sourcelink print-urls bin\Release\netcoreapp2.1\U2U.AspNetCore.CleanArchitecture.pdb
ef0790bae6de4b9d3da8dd4e8b07bc07e22edbc2 sha1 csharp C:\Code\U2U.AspNetCore.CleanArchitecture\AutoConfigAttribute.cs
https://raw.githubusercontent.com/PeterHimschoot/U2U.AspNetCore.CleanArchitecture/4692435d950792deaca1024b9f75845b25056ffb/AutoConfigAttribute.cs
60c4fe8763da3c4dcbc3ec327218bd6a7711262b sha1 csharp C:\Code\U2U.AspNetCore.CleanArchitecture\AutoConfigOptions.cs
https://raw.githubusercontent.com/PeterHimschoot/U2U.AspNetCore.CleanArchitecture/4692435d950792deaca1024b9f75845b25056ffb/AutoConfigOptions.cs
fd4c66e55846ff0106cdf5cd029ae65da374de13 sha1 csharp C:\Code\U2U.AspNetCore.CleanArchitecture\ServiceCollectionExtensions.cs
https://raw.githubusercontent.com/PeterHimschoot/U2U.AspNetCore.CleanArchitecture/4692435d950792deaca1024b9f75845b25056ffb/ServiceCollectionExtensions.cs
8772ab230ba2399211c444543882cd8f545181d9 sha1 csharp C:\Users\Peter\AppData\Local\Temp\.NETCoreApp,Version=v2.1.AssemblyAttributes.cs
embedded
```

In my case I saw that the urls were not pointing to GitHub, instead they were pointing to VSTS. So I checked my sources into GitHub and it was done.

You can also test your pdb file, which will check the urls and their checksums

```
sourcelink test bin\Release\netcoreapp2.1\U2U.AspNetCore.CleanArchitecture.pdb
sourcelink test passed: bin\Release\netcoreapp2.1\U2U.AspNetCore.CleanArchitecture.pdb
```
 
