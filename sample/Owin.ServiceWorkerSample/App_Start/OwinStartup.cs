using System.Configuration;
using Microsoft.Owin;
using Owin.ServiceWorker;

[assembly: OwinStartup(typeof(Owin.ServiceWorkerSample.OwinStartup))]

namespace Owin.ServiceWorkerSample
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new DefaultServiceWorkerConfig(ConfigurationManager.AppSettings);
            app.UseServiceWorker(config);
        }
    }
}
