using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Owin.ServiceWorker
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Serves serviceworker.js for a specific strategy
    /// </summary>
    public class ServiceWorkerMiddleware
    {
        public const string ContextKey_ServiceWorker = "ServiceWorkerOptions";

        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        private AppFunc _next;
        private ServiceWorkerOptions _options;
        private PathString _serviceWorkerRoute;

        /// <summary>
        /// Creates an instance of the controller.
        /// </summary>
        public ServiceWorkerMiddleware(AppFunc next, ServiceWorkerOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceWorkerRoute = new PathString(_options.ServiceWorkerRoute);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);
            IOwinRequest request = context.Request;

            if (request.Path.StartsWithSegments(_serviceWorkerRoute))
                await HandleServiceWorkerAsync(context, _options);
            else
                await _next(environment);
        }

        internal static async Task HandleServiceWorkerAsync(IOwinContext context, ServiceWorkerOptions options)
        {
            IOwinResponse response = context.Response;
            response.ContentType = "application/javascript; charset=utf-8";

            string fileName = options.Strategy + ".js";
            Assembly assembly = typeof(ServiceWorkerMiddleware).Assembly;
            Stream resourceStream = assembly.GetManifestResourceStream($"Owin.ServiceWorker.ServiceWorker.Files.{fileName}");

            using (var reader = new StreamReader(resourceStream))
            {
                string js = reader.ReadToEnd();
                string modified = js
                    .Replace("{version}", options.CacheId + "::" + options.Strategy)
                    .Replace("{routes}", string.Join(",", options.RoutesToPreCache.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(r => "'" + r.Trim() + "'")))
                    .Replace("{offlineRoute}", options.OfflineRoute);

                await response.WriteAsync(modified);
            }
        }
    }
}
