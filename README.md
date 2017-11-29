## Configure and add a Service Worker via ASP.NET OWIN Middleware

***NOTE:*** Most of the Service Worker code was adapted from Mads Kristensen's ASP.NET Core package: https://github.com/madskristensen/WebEssentials.AspNetCore.ServiceWorker

This package does not include an offline.html. This file must be provided separately by the web app.

## Getting Started

First, register the middleware in Owin Startup. The example below uses default settings and allows them to be overwritten via the web.config. You can use the IServiceWorkerConfig interface to implement a different configuration source.

```c#
public void Configuration(IAppBuilder app)
{
    var config = new DefaultServiceWorkerConfig(ConfigurationManager.AppSettings);
    app.UseServiceWorker(config);
}
```

You can manually register the Service Worker on the client side, for example:
```javascript
if ('serviceWorker' in navigator) {
    window.addEventListener('load', function () {
        navigator.serviceWorker.register('/serviceworker');
    });
}
```

Alternatively, the Owin.ServiceWorker package provides a helper class to generate a script tag (which uses the appropriate configured values from the provided ServiceWorkerOptions). 
A configured instance can be retrieved off of the IOwinContext.

```csharp
string scriptTag = owinContext.ServiceWorkerRegistration().RenderScriptTag();
```

As shown in the Sample app, this could then be called from view code. For example, as shown in the Sample app it could be added to the MVC HtmlHelper as an extension method.

```csharp
public static class ServiceWorkerHtmlHelperExtensions
{
    public static HtmlString RegisterServiceWorker(this HtmlHelper html) =>
        new HtmlString(HttpContext.Current.GetOwinContext().ServiceWorkerRegistration().RenderScriptTag());
}

// and in Index.cshtml
@section scripts {
    @Html.RegisterServiceWorker()
}
```

## Configuring

The service worker middleware can be configured either via passing an instance of IServiceWorkerConfig or by passing individual configuration values to the UseServiceWorker registration method. The Owin.ServiceWorker package includes a DefaultServiceWorkerConfig implementation of IServiceWorkerConfig that can be initialized from web.config App Settings (as shown in **Getting Started.**) An instance of ServiceWorkerOptions is initialized off of the configuration values and added to the OwinContext on each request.

Valid configuration settings are:
```xml
<add key="serviceworker:route" value="/serviceworker" />
<!-- defaults to true if omitted here -->
<add key="serviceworker:registerServiceWorker" value="true" />
<add key="serviceworker:cacheId" value="v1.0" />
<!-- valid values are: cachefirst, cachefirstsafe, minmal, and networkfirst. Defaults to cachefirstsafe if omitted -->
<add key="serviceworker:strategy" value="cachefirst" />
<add key="serviceworker:routesToPreCache" value="site.css,app.js" />
<!-- Defaults to /offline.html if omitted, but is required to be set if different than /offline.html -->
<add key="serviceworker:offlineRoute" value="/offline.html" />
```

***NOTE:*** If you change the path to the Service Worker to have a .js extension and are running your web app in IIS, then you will likely need to add a handler entry 
```xml
<add name="ServiceWorker" path="/sw.js" verb="GET" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" />
```