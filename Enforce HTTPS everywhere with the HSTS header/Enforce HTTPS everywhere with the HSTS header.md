# Enforce HTTPS everywhere with the HSTS header

With the **HTTP Strict Transport Security** header you can tell the browser to only use secure HTTP (so **HTTPS**) for downloading your website's content. 

This blog post explains the header, but also how to add the HSTS header to your .NET Core website using the `U2U.AspNetCore.Security.Headers` package.

## So how does it work?

Modern browsers can be instructed about how to handle your content in different ways. You can tell it for example to only download scripts from your web site with the Content Security Policy header. You can also tell it to use HTTPS to download your content.

The HSTS header is a great way to protect against [Downgrade attacks](https://en.wikipedia.org/wiki/Downgrade_attack) and [SSL strip attack](https://avicoder.me/2016/02/22/SSLstrip-for-newbies/)

The header itself is pretty straighforward. Here's an example:

```
Strict-Transport-Security:max-age=86400; includeSubdomains; preload;
```

## Options in the HSTS header - the max-age directive

The **max-age** directive marks the period in which insecure requests cannot be made...
The units are in seconds, and the duration is reset on every response of the response header.

**You do have the realize that all requests will now use https, including .css, .js, etc...**

If you want to get rid of the header (especially during development) you can do so, but it heavily depends on the browser. 

For example in chrome go to [chrome://net-internals/#hsts](chrome://net-internals/#hsts)
Here you can add a domain manually, delete a domain, and query the domain to see the header from that site.

## The include-subdomains directive

The scope of the HSTS header can be extended to sub-domains, protecting current and future sub-domains. You do need to be careful with this since it might block pages that don't use HTTPS. This is required for the preload directive.

## Trust On First Use (TOFU)

There is of course an obvious problem with this header, generally knows as **T**rust **O**n **F**irst **U**se, or **TOFU**. If the attacker can intercept the first interaction, he (or she) can easily strip the header from the response. 

## The preload directive

To prevent this window of opportunity, you can register your domain (with all subdomains). Browser builders will then add your domain to a list embedded in the browser, which will automatically use HTTPS for your domain. **[Registering your domain](https://hstspreload.org/#removal) will make it very hard to switch back to HTTP.**

To register your domain go to [https://hstspreload.appspot.com](https://hstspreload.appspot.com); this site belongs to the **chromium** project. It will take some time for your domain to be added to all browsers.

If you want to check if a site is on the **preload list**: here is the [list](https://cs.chromium.org/chromium/src/net/http/transport_security_state_static.json)

## Browser compatibility

Goto [http://caniuse.com/#feat=stricttransportsecurity](http://caniuse.com/#feat=stricttransportsecurity) to check compatibility.
Internet explorer is the unfortunate exception in this list, no support until IE 11...

## Adding the HSTS header to your website hosted in IIS

Adding the header is easy when your website is deployed with **IIS**. 
Simply add this in configuration:

```
<system.webServer>
  <customHeaders>
    <add name="Strict-Transport-Security" 
         value="max-age=6000; includeSubdomains;" />
  </customHeaders>
</system.webServer>
```

## Adding the HSTS header in .NET Core

You can easily add the header using the `U2U.AspNetCore.Security.Headers` package.

After adding the package add this to the `Configure` method in `Startup`

```
app.UseResponseHeaders(builder =>
{
  builder.SetStrictTransportSecurity(new StrictTransportSecurity
  {
    MaxAge = TimeSpan.FromDays(1),
    IncludeSubdomains = true,
    Preload = false
  });
};
```

You can add any header with this middleware using the `builder.SetHeader("Header", "Value")` method. But it also contains a couple of convenience methods such as `SetStrictTransportSecurity`.


