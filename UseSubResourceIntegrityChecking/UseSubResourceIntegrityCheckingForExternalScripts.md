## Use SubResource Integrity Checking for External Scripts

![SubResource Integrity](https://u2ublogimages.blob.core.windows.net/peter/SRIHeader.png)

Recently **British Airways** got [hacked](https://gbhackers.com/british-airways-hacked/) and more than **380.000 payment cards** got compromised. So how could this have happened? Imagine that BA uses some external JavaScript library. If a hacker can change this external source and add his/her own code to the library, it is a piece of cake to steal any information that the user enters on the website. So how can you avoid this hack?

## Add SubResource Integrity Checking for External Scripts

[SubResource Integrity (SRI) checking](https://frederik-braun.com/using-subresource-integrity.html) add a hash value to the `<
script>` tag, so if the external source gets modified the browser will refuse to load and execute it.
For example:
```
<script src="https://code.jquery.com/jquery-3.3.1.min.js" 
        integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8="
        crossorigin="anonymous"></script>
```
You can also use it for external stylesheets
```
<link rel="stylesheet" href="https://site53.example.net/style.css" 
          integrity="sha256-vjnUh7+rXHH2lg/5vDY8032ftNVCIEC21vL6szrVw9M=" 
          crossorigin="anonymous">
```

## Browser Support

Using [CanIUse.com](https://caniuse.com/#feat=subresource-integrity) we can see that all modern browsers support this. And if you need another reason to dump Internet Explorer... No SRI support.

![Internet Explorer](https://u2ublogimages.blob.core.windows.net/peter/comics-microsoft-call-internet-explorer-542880.png)
> From [iamwire](http://www.iamwire.com/2015/04/journey-internet-explorer-good-bad-funny/113181)

## Enabling SRI

Most external libraries get downloaded from a [CDN](https://en.wikipedia.org/wiki/Content_delivery_network) server. Lots of CDNs provided a link with SRI automatically so there is no excuse not to use it (but SRI is still heavily underused by a lot of web sites, even though it has been [available](https://www.w3.org/TR/SRI/) for over two years!).

For example, [CloudFlare's CDN](https://cdnjs.com/) allows you to copy a link with SRI enabled:

![CloudFlare with SRI](https://u2ublogimages.blob.core.windows.net/peter/CloudFlareSRI.gif)

But even when the CDN doesn't provide an SRI link, it is easy to generate one yourself using [SRI Hash Generator](https://www.srihash.org/).

![SRI Hash Generator](https://u2ublogimages.blob.core.windows.net/peter/SRIGenerator.PNG)

## Using SRI with CSP

You can use [Content Security Policy](https://blogs.u2u.be/peter/post/protect-your-dotnet-core-website-with-content-security-policy
) to require all your scripts and/or stylesheets to use SRI.
```
Content-Security-Policy: require-sri-for script;
```

## CORS

What does the `crossorigin` attribute do? When you download resources from a source other than your own website the browser will check if it should do so using [Cross-Origin Resource Sharing (CORS)](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS). Therefore, the resource must be served with an `Access-Control-Allow-Origin` header that allows the resource to be shared with the requesting origin:
```
Access-Control-Allow-Origin: *
```

## Call to Action

Dear web developers, always use SRI for your external scripts and stylesheets! It's very easy to use and will make your website a more secure place!
