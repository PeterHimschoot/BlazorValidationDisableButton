# Pin your certificate to your web site with the HTTP Public Key Pinning header

![HTTP Public Key Pinning](http://www.freeimageslive.co.uk/files/images009/marker_pin.preview.jpg)

So what is public key pinning? The **HTTP public key pinning** header can prevent fraudulent use of TLS certificates to impersonate valid web sites. Most **Man-in-the-middle (MITM)** attacks can be prevented by proper
use of HTTPS and HSTS ([see my blog on HSTS)](http://blogs.u2u.be/peter/post/enforce-https-everywhere-with-the-hsts-header)). However, if a hacker can convince a certificate authority to issue a fraudulent certificate for your web site, they can still spoof your website using a MITM attack. This is pretty hard, but not impossible [as history illustrates](https://en.wikipedia.org/wiki/DigiNotar).

When your server sends the HPKP header containing pinned certificate keys to the browser, these pins are then stored by said browser. Any future response (within a certain time-frame) from the same website not using one of these pinned keys is simply refused by the browser.

[More on DigiNotar](https://blog.mozilla.org/security/2011/09/02/diginotar-removal-follow-up/), [Lax policies](https://security.googleblog.com/2015/03/maintaining-digital-certificate-security.html).

## Pinning your certificate(s)

There are three kinds of keys that can be pinned by HPKP:
* The public key of the certificate issued to a site
* Public keys corresponding to a certificate authority and their intermediate certificates
* Backup keys

Browsers will only accept HPKP headers if they contain **at least two pinned keys**. At least one key must be in the
trust-chain formed by the browser when verifying the site's certificate, and there must be at least one pin which is **not** in the trust-chain (the backup key).

For example, here is the HPKP header from [GitHub.com](https://github.com)

```
Public-Key-Pins:max-age=5184000; 
                pin-sha256="WoiWRyIOVNa9ihaBciRSC7XHjliYS9VwUGOIud4PB18="; 
                pin-sha256="RRM1dGqnDFsCJXBTHky16vi1obOlCgFFn/yOhI/y+ho="; 
                pin-sha256="k2v657xBsOVe1PQRwOsHsw3bsGT2VzIqz5K+59sNQws="; 
                pin-sha256="K87oWBWM9UZfyddvDfoxL+8lpNyoUB2ptGtn0fv6G2Q="; 
                pin-sha256="IQBnNBEiFuhj+8x6X8XLgh01V9Ic5/V3IRQLNFFc7v4="; 
                pin-sha256="iie1VXtL7HzAMF+/PVPR9xzT80kQxdZeJ+zduCB3uj0="; pin-sha256="LvRiGEjRqfzurezaWuj8Wie2gyHMrW5Q06LspMnox7A="; 
                includeSubDomains
 ```

The first pinned key (identified by the `WoiWRyIOVNa9ihaBciRSC7XHjliYS9VwUGOIud4PB18=` SHA-256 hash) corresponds to the DigiCert High Assurance EV Root CA. This is the root of the chain of trust currently used by github.com.

The second pinned key (identified by the `RRM1dGqnDFsCJXBTHky16vi1obOlCgFFn/yOhI/y+ho=` SHA-256 hash) is the intermediate key for the DigiCert SHA2 Extended Validation Server CA.

The third hash (`k2v657xBsOVe1PQRwOsHsw3bsGT2VzIqz5K+59sNQws=`) is GitHub's backup pin. The fourth key (`K87oWBWM9UZfyddvDfoxL+8lpNyoUB2ptGtn0fv6G2Q=`) is another backup pin, corresponding to the GlobalSign Root CA.
As these keys do not appear in GitHub's served certificate chain, they are treated as backup pins.

How do I know this? I've used the [HPKP analyzer](https://report-uri.io/home/pkp_analyse). Simply enter the website's url (using the **https** scheme!).

May I also suggest you read the [Guidance on HPKP pinning](https://scotthelme.co.uk/guidance-on-setting-up-hpkp/)

## Adding the HSTS header to your website hosted in IIS

Adding the header is easy when your website is deployed with **IIS**. 
Simply add this in configuration:

```
<system.webServer>
  <customHeaders>
    <add name="Strict-Transport-Security" 
         value="pin-sha256="yh0kYiYm4YN+0DAKp4bB16pGqrQq9btXHMeR9jz834o=";
                pin-sha256="YLh1dUR9y6Kja30RrAn7JKnbQG/uEtLMkBgFF2Fuihg="; 
                pin-sha256="SEnt86CqqSYlSIlLcfnKdJdoS8NJG1EG+/5b5qtvmUY="; 
                max-age=864000; includeSubdomains;" />
  </customHeaders>
</system.webServer>
```

## Adding the HTTP Public Key Pinning header to your .net Core site

If you're using .net core, then adding this header is pretty easy.

Start by adding the `U2U.AspNetCore.Security.Headers` package:

```
dotnet add package U2U.AspNetCore.Security.Headers
```

Then add this somewhere in the `Configure` method:

```csharp
app.UseResponseHeaders(builder => {
  builder.SetPublicKeyPinning(new PublicKeyPinning {
    MaxAge = TimeSpan.FromMinutes(10),
    IncludeSubdomains = true,
    Pins = new List<string> {
        "yh0kYiYm4YN+0DAKp4bB16pGqrQq9btXHMeR9jz834o=", // current certificate
        "SEnt86CqqSYlSIlLcfnKdJdoS8NJG1EG+/5b5qtvmUY=" // demo Certificate Signing Request
        }
  });
  // Add other headers
});
```

This package is open source and can be found in [github](https://github.com/PeterHimschoot/U2U.AspNetCore.Security.Headers)

## Getting your own pins

First of all, you need [to get a certificate](http://blogs.u2u.be/peter/post/configure-https-with-letsencrypt-manually). **Don't use IIS manager to get a certificate**, since it will [generate a certificate using a SHA1 hash](http://dotnetstock.com/technical/how-to-generate-a-sha256-certificate-and-how-to-install-sha256-certificate-in-iis/)!

So how can I get the pin of my **certificate**?

If you have already installed your certificate in your web site, which is up and running using HTTPS, use the [HPKP hash generator](https://report-uri.io/home/pkp_hash). Copy your website's https url in there, click on `Hash` and it will show you the pins you can use.

But what about the backup pin(s)?!

The following commands use **openssl**, you make sure you've installed it.

To generate the pin for a certificate, use: 

```
openssl x509 -in <your-certificate> -pubkey -noout | openssl rsa -pubin -outform der | openssl dgst -sha256 -binary | openssl enc -base64
```

And this way you can get the pin for a **certificate signing request**:

```
openssl req -in CertificateRequest.csr -pubkey -noout | openssl rsa -pubin -outform der | openssl dgst -sha256 -binary | openssl enc -base64
```

And finally, you can get the pin for an **existing HTTPS web site**, for example `httpsecurityheaders.applephi.net`:

```
openssl s_client -servername httpsecurityheaders.applephi.net -connect httpsecurityheaders.applephi.net:443 | openssl x509 -pubkey -noout | openssl rsa -pubin -outform der | openssl dgst -sha256 -binary | openssl enc -base64
```

## Browser support

Visiting [CanIUse.com](https://www.caniuse.com) reveals that Chrome, Firefox and Opera fully support it, and hopefully Edge soon.

![CanIUse](https://u2ublogimages.blob.core.windows.net/peter/CanIUseHPKP.png)

## HPKP backfiring

HPKP is a very effective way of protecting your web site from being imporsonated, but you have to be very careful!

For starters, it is **essential** that you specify **at least two public key pins**, where at least one if a backup pin. This is in case you lose control of your certificate. Some web sites only specify one pin, and browsers will not enable HPKP in this case (the header might just as well be removed from your web site).

Also make sure that the pins are actually hashes, and use the exact format (using double quotes). For example, this web site is not protected at all (because the pins need to be enclosed in double qoutes):

```
Public-Key-Pins: max-age=86400;
  pin-sha256=rhdxr9/utGWqudj8bNbG3sEcyMYn5wspiI5mZWkHE8A=
  pin-sha256=lT09gPUeQfbYrlxRtpsHrjDblj9Rpz+u7ajfCrg4qDM=
```

Don't bother with the **HPKP header over HTTP** either, since browsers will ignore it in this case. The reason is simple: if a hacker could inject the HPKP header in a man-in-the-middle attack, it would be very easy to prevent users from accessing a web site.

The HPKP header contains the `max-age` directive. Some sites use **a very short max-age** -- like 5 or 10 seconds --, which renders the header useless because the browser will forget about your pins way to fast to be effective. 

