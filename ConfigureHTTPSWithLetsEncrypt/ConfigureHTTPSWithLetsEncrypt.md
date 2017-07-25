# Configure HTTPS with LetsEncrypt

In this blog post I want to show you how to get an HTTPS certificate to protect your web site. The certificate will be issued by [LetsEncrypt](), but you will use [GetHttpsForFree](https://gethttpsforfree.com) to get your certificate.


## Step 1: Registering your public key and e-mail

This part should be pretty straightforward. In **Step1 : Account Info** you enter your e-mail and public key.

### Generating your key-pair

Certificates are actually asymmetric key-pairs with meta-data, so the first thing to do is to create your key-pair. This is actually easy to do with `openssl`, so if you don't have `openssl` yet, install it.

To generate your key-pair, execute following script (and keep this key safe and **private**! Compare it to your credit card number).

```
openssl genrsa 4096 > account.key
```

Now you can extract your public key, and as the name suggests, **this part you can share**.

```
openssl rsa -in account.key -pubout
```

This will write to your console the base64 encoded public key.

```
writing RSA key
-----BEGIN PUBLIC KEY-----
MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAuL9J76iao3ZmznsKhOb0
id0rC5Pl/ZaqMZW4pGholwZGN9uZNsDPNKPGquM597NwKlGprCCRZmVFrkARrHzw
2KE5r23aoOTvlJdjGdhW73q1rGGLds0bJT6etxCFPDLcIC3cZxHN9xb4dQ1o1rGi
uZmGu2ZG372LRhpnpsZr9KN87J+GU33JorfwNxHO20O2Tw0/wIxxH7qCoccZXNuW
3TVWME++85xnHePu7QiW1WdkZruSxoWwuUaBbPODkQf8q4rji8kELA7WQuN3DnE8
m49xm+g6RDkkU+Msqy7JBcxz4H5yReowE6RbRYDzYBiClgefhwC3Udh675AFbVME
lMC0wjYaoxZ7vpE8wRyXHiDU5o9CINE4rqIlVpgzrse9kDviwjHLZsM9G1nj3ppZ
suEzOxCu/hirmsR+2XXmqECsu3BaeG6uYirXo+m27cVGrXPhHcAg4uqb+M2Ll+iN
kba6oBxxI8iXcWXoM6r2vNX2U/fW0JZSQ0coREsHzcYsb+RQiGEdkXsActeMlkZ5
gCaidBHT2CLfhlLww4oAFyBenOtW3DJwcLv+gLL7ihHY+ZaDhCCBRFPSKWUFWLe2
aEzdFeIgS77bT/orVQBIdW+W+MwnyNZuhsYDCT4wOADkHnMHFy+QHTpqzUCgzdbv
mdEoyfJz2aKvki4OmFzat9kCAwEAAQ==
-----END PUBLIC KEY-----
```

Now copy everthing (and including) between the ---- into the Account Public Key input.

Click on **Validate Account Info** to check if you made a mistake.

## Step 2 : Creating the Certificate Signing Request

Carefull, start by creating a seperate key for the CSR:

```
openssl genrsa 4096 > certificate.key
```

```
openssl req -new -sha256 -key certificate.key -out account.csr
```

You will now how to fill in some fiels, making sure you select your domain when entering the common name. For example:
```
Country Name (2 letter code) [AU]:BE
State or Province Name (full name) [Some-State]:Oost-Vlaanderen
Locality Name (eg, city) []:Melle
Organization Name (eg, company) [Internet Widgits Pty Ltd]:applephi.net
Organizational Unit Name (eg, section) []:IT
Common Name (e.g. server FQDN or YOUR name) []:addinghttps.applephi.net
Email Address []:peter@u2u.be
```

This will create an `account.csr` file next to your key.

Copy the contents for this file into the Certificate Signing Request field and click on `Validate CSR`. 
Check if your domain was found.

## Step 3 : Signing stuff

Here you simply copy the contents of each field and execute it on the command line, then copy the result back into the next field. Click on `Validate Signatures` to verify if you did everyting correctly.

## Step 4 : Verify ownership

Time to prove that you actually own the domain. One way of doing this is uploading a special file in a designated location.











## Create your pfx file from the cert and intermediate

https://www.markbrilman.nl/2012/07/creating-a-pfx-file-with-chain/ 

openssl pkcs12 -export -out account.pfx -inkey account.key -in chained.pem 

openssl pkcs12 -export -out account.pfx -inkey certificate.key -in chained.pem









