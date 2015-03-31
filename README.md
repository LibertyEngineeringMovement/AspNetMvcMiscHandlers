#Asp Net Mvc Misc Handlers
This is durived work from several Creative Commons articles. I will add attribution to originators when I find them.

## Asp Net Mvc Cache Manifest Handler
Dynamicly generate a cache manifest file. This is a pretty thin handler, but it works as a starting point.

### Add the following to your Web config
    <system.web>
    <httpHandlers>
    <add name="CacheManifest" verb="GET" path="cache.manifest" type="HTTPHandlers.CacheManifestHandler"/>
    </httpHandlers>
    </system.web>

And this
    
    <system.webServer>
    <handlers>
    <add name="CacheManifest" verb="GET" path="cache.manifest" type="HTTPHandlers.CacheManifestHandler"/>
    </handlers>
    </system.webServer>

### Add the following to your layout HTML tag
    <html manifest="@Href("~/cache.manifest")">

