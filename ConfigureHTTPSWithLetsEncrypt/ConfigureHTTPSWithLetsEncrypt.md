# Configure HTTPS with LetsEncrypt MANUALLY

In this blog post I want to show you how to get an HTTPS certificate (for free) to protect your web site. 
The certificate will be issued by [LetsEncrypt](https://letsencrypt.org/), but you will use [GetHttpsForFree](https://gethttpsforfree.com) to get your certificate.

[So why would you use HTTPS?](https://scotthelme.co.uk/still-think-you-dont-need-https)? 

# Some setup required

In these steps you will use [OpenSSL](https://www.openssl.org/), so please install it first.
After installation, make sure **openssl.exe** is in your **PATH** environment variable.

In step 3 you're going to execute a number of commands. However these are `bash` commands, so on windows you have a choice. If you already have Windows 10 64-bit anniversary update you can install `bash for Windows` (but read on for a simpler solution).
This [link](https://www.howtogeek.com/249966/how-to-install-and-use-the-linux-bash-shell-on-windows-10/) explains how to install the bash shell in Windows 10.

The other way (easier and no restart required) is to install [Git for Windows](https://git-scm.com/download/win). Simply take all the defaults during installation. This includes a bash shell where you can run the commands.

Open the git bash shell (Windows key followed by `git bash`)

## Step 1: Registering your public key and e-mail

Start by opening your favorite browser and go to [GetHttpsForFree](https://gethttpsforfree.com).

This part should be pretty straightforward. In **Step1 : Account Info** you enter your e-mail and public account key (read on).

### Generating your key-pair

SSL Certificates are actually asymmetric key-pairs with meta-data (including your domain), so the first thing to do is to create your key-pair. This is actually easy to do with **openssl**.

To generate your key-pair, execute following command (and keep this key safe and **private**! Compare it to your credit card number). The command will generate a key pair that is used for communication (signing a bunch of stuff in step 3), it is not the certificate's key.
This command uses a file named `account.key`, feel free to substitute this with your own choice of file name.

```
openssl genrsa 4096 > account.key
```

This file now contains your private key, which you don't want to share.
Now you need to extract your public key, and as the name suggests, **this part you can share**.

```
openssl rsa -in account.key -pubout
```

This will write to your console the **base64 encoded** public key.

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

Now copy everthing (and including) between the ---- into the Account Public Key input in your browser.

Click on **Validate Account Info** to check if you made a mistake.

## Step 2 : Creating the Certificate Signing Request

A **Certificate Signing Request** (also CSR or certification request) is a message sent from an applicant to a **Certificate Authority** in order to apply for a digital identity certificate. It usually contains the **public key** for which the certificate should be issued, identifying information (such as a **domain name**) and integrity protection (e.g., a **digital signature**).

Start by creating a seperate key for the CSR:

```
openssl genrsa 4096 > certificate.key
```

This will generate another key-pair, which will be used for creating the CSR. Execute following command to do this (and read on on how to fill in a number of questions):

```
openssl req -new -sha256 -key certificate.key -out certificate.csr
```

You will now answer some questions, most of which are pretty straightforward, but make sure you select your domain when entering the common name. 

For example:

```
Country Name (2 letter code) [AU]:BE
State or Province Name (full name) [Some-State]:Oost-Vlaanderen
Locality Name (eg, city) []:Melle
Organization Name (eg, company) [Internet Widgits Pty Ltd]:applephi.net
Organizational Unit Name (eg, section) []:IT
Common Name (e.g. server FQDN or YOUR name) []:addinghttps.applephi.net
Email Address []:peter@u2u.be
```

This will create an `certificate.csr` file next to your key.

Copy the contents for this file into the **Certificate Signing Request** field and click on `Validate CSR`. From the command line you can easily dump the contents of a file with `type certificate.csr` on Windows, or `cat certificate.csr` on OSX or Linux (and even Windows in the Git bash shell).

Check if your domain was found.

## Step 3 : Signing stuff

The next steps are tricky, so if something goes wrong the site tells you to restart from step 1. This actually means you go back to step 1, and click the verify button, step 2, click verify again. So it is not as bad as it sounds...

Here you simply copy the contents of each field and execute it on the command line, then copy the result back into the next field. Click on `Validate Signatures` to verify if you did everyting correctly.

## Step 4 : Verify ownership

Time to prove that you actually own the domain. One way of doing this is uploading a special file in a designated location in your web site. Since this option works with any kind of web server we will use this one.

Start by running the signature command in the bash shell (like step 3).

Now click on **Option 2**. Very carefully create a file with the requested content. Rename the file with the requested name, and upload it to your website to the designated path. 

Copy the url into a new tab in your browser and see it it can download the file. When all of this works click on **I'm now serving this file...**

Now you are ready for step 5 (so you're almost there!).

## Step 5: Create your pfx file from the cert and intermediate

Many web servers such as IIS and Azure require a certificate in the .pfx format. So that is what you're going to do right now.

Create a new file calling it `certificate.crt` and copy-paste (all of it) the **Signed Certificate** field into this file.
Create another file called `intermediate.pem` and copy the **Intermediate Certificate** fields into this file.

Execute following command (in the bash shell of git read on):

```
openssl pkcs12 -export -out certificate.pfx -inkey certificate.key -in certificate.crt -certfile intermediate.pem
```

The openssl command sometimes has problems interacting with the git bash shell, in this case prefix the command with `winpty`:

```
winpty openssl pkcs12 -export -out certificate.pfx -inkey certificate.key -in certificate.crt -certfile intermediate.pem
```

You will be asked to enter a password, make sure you don't forget it because you will need it to install the certificate.

## Step 6 : Installing the certificate

Well... That depends on your web server.

On windows you can double-click the certificate to install it. Simply follow the wizard.

Now open IIS on the machine where you installed it. Double-click the **Server Certificates** icon. Your certificate should be listed.

Double-click your certificate to inspect it.



