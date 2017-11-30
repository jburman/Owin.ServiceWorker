using System;
using Microsoft.Owin;

namespace Owin.ServiceWorker
{
    public static class ServiceWorkerMiddlewareExtensions
    {
        public static void UseServiceWorker(this IAppBuilder builder,
            IServiceWorkerConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var options = new ServiceWorkerOptions(config);

            _UseServiceWorker(builder, options);
        }
        
        public static void UseServiceWorker(this IAppBuilder builder,
            IServiceWorkerConfig config,
            string offlineRoute = Constants.DefaultOfflineRoute, 
            ServiceWorkerStrategy strategy = ServiceWorkerStrategy.CacheFirstSafe, 
            bool registerServiceWorker = true, 
            bool registerWebManifest = true, 
            string cacheId = Constants.DefaultCacheId, 
            string routesToPreCache = "")
        {
            var options = new ServiceWorkerOptions
            {
                OfflineRoute = offlineRoute,
                Strategy = strategy,
                RegisterServiceWorker = registerServiceWorker,
                CacheId = cacheId,
                RoutesToPreCache = routesToPreCache
            };

            _UseServiceWorker(builder, options);
        }

        private static void _UseServiceWorker(IAppBuilder builder, ServiceWorkerOptions options)
        {
            builder.Use(async (context, next) =>
            {
                context.Set(ServiceWorkerMiddleware.ContextKey_ServiceWorker, options);
                await next();
            });

            builder.Use<ServiceWorkerMiddleware>(options);
        }

        public static ServiceWorkerOptions ServiceWorkerOptions(this IOwinContext context) =>
            context.Get<ServiceWorkerOptions>(ServiceWorkerMiddleware.ContextKey_ServiceWorker);

        public static ServiceWorkerRegistration ServiceWorkerRegistration(this IOwinContext context) =>
            new ServiceWorkerRegistration(context.Get<ServiceWorkerOptions>(ServiceWorkerMiddleware.ContextKey_ServiceWorker));
    }
}
