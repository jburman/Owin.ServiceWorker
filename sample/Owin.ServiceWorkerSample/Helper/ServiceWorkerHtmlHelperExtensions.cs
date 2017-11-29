using System.Web;
using System.Web.Mvc;
using Owin.ServiceWorker;

namespace Owin.ServiceWorkerSample
{
    public static class ServiceWorkerHtmlHelperExtensions
    {
        public static HtmlString RegisterServiceWorker(this HtmlHelper html) =>
            new HtmlString(HttpContext.Current.GetOwinContext().ServiceWorkerRegistration().RenderScriptTag());
    }
}