# Ny subdomän

* Ny binding i .vs\config\applicationhost.config
* Ny Url reservation: netsh http add urlacl url=http://___.snittlistan.se:61026/ user=everyone
* Lägg till domänen i C:\Windows\System32\drivers\etc\hosts
* Lägg till connectionString och appSettings i Snittlistan.Tool
* Skapa databasen
* Ny connectionString i Snittlistan.Tool.exe.config och Snittlistan.Web.config
* Kör Snittlistan.Tool.exe initialize
* Fixa bilderna med https://realfavicongenerator.net
* Ny TenantConfiguration i Global.asax.cs
* Lägg till A record för domänen i Netcetera Control Panel. Ska peka på 81.27.111.95
* Uppdatera bindings i Snittlistan.main.wxs
* Installera ny version
* Kör LEWS med alla domännamn. Från C:\Users\Administrator\Downloads\letsencrypt-win-simple.v1.9.7.0-beta10
    N: Create New Certificate
    2: SAN certificate for all bindings of an IIS site
    1: Snittlistan
    6: [http-01] Self-host verification files (port 80 will be unavailable during validation)
* Lägg till webbplatsövervakning i site24x7 med en timmes intervall.
