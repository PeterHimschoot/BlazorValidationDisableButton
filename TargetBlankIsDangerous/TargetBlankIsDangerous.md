# Be careful using target='_blank'

![header](https://u2ublogimages.blob.core.windows.net/peter/TargetBlank.png)

Be careful using anchors with `target='_blank'`

``` html
<a href="..." target='_blank'>Link</a>
```

Why? Because the tab/window that opens to show this link can access the `window` object of the tab containing the link through the `opener` object:

``` javascript
window.opener.location = 'https://phishingpage'
```

Normally the opener is used to pass information from source to destination tab.

An attacker could use this to redirect this tab to a phishing site, which could then trick the user into typing his or her password. Or (only annoying) close the original tab

``` javascript
window.opener.close();
```

----

If you want to try this you can copy [this url](https://httpsecurityheaders.applephi.net/Vulnerable/TargetBlank) in your browser `https://httpsecurityheaders.applephi.net/Vulnerable/TargetBlank`.
This page contains a link to another page which then modifies the original page.

## So how to I prevent this?

The solution is pretty simple. When using an anchor with a target you can disable the passing of the `opener` using `rel='noopener`.

``` html
<a href="..." target='_blank' rel='noopener'>Link</a>
```

This way no `opener` is available in the target tab/window.
