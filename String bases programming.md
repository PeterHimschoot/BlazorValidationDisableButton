# String based programming
I really don't like the coding style of using strings instead of identifiers...
For example in MVC you can redirect to another action using

```
RedirectToAction("Index");
```
However, using the string "Index" instead of the name of the method (an action typically corresponds to a method in MVC right?!) make your code harder to maintain. Renaming the method will not rename the string for example.
That is why I like to use

```
RedirectToAction(nameof(HomeController.Index))
```
However, this does not work with Controller names. Because of the way routing is implemented, using the full name of the controller confuses routing and you end up with an invalid Uri.

```
RedirectToAction(nameof(HomeController.Index), nameof(HomeController))
```

This problem is fixed by `U2U.AspNetCore.Mvc.NameOf`
# U2U.AspNetCore.Mvc.NameOf

This package updates Mvc6 to allows the use of the `nameof()` keyword in your views 

For example, instead of using strings for the name of the controller and action

```
<a asp-controller="Home" asp-action="Index" class="navbar-brand">Index</a>
```

you can use `nameof()`

```
<a asp-controller="@nameof(HomeController)" asp-action="@nameof(HomeController.Index)" class="navbar-brand">Index</a>
```

## Setup

All you need to do extra is to add a `using` to your `_ViewImports.cshtml`

```
@using WebApplication2.Controllers
```

and in `Startup.ConfigureServices` add a call to `AddNameOf` after the call to `AddMvc`.

```
services.AddMvc();
services.AddNameOf();
```

# Sources

I you're interested in how this packages works, it is available on github at
<https://github.com/PeterHimschoot/U2U.AspNetCore.Mvc.NameOf>

Any bugs, remarks, etc... can always be sent to <peter@u2u.be>