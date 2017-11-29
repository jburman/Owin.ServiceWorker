namespace Owin.ServiceWorker
{
    public class ServiceWorkerRegistration
    {
        private static readonly string _registerScriptTemplate = @"    
    <script type=""text/javascript"">
        if ('serviceWorker' in navigator) {{
            window.addEventListener('load', function() {{
                navigator.serviceWorker.register('{0}');
            }});
        }}
    </script>";

        private ServiceWorkerOptions _options;

        public ServiceWorkerRegistration(ServiceWorkerOptions options)
        {
            _options = options;
        }

        public string RenderScriptTag()
        {
            if (_options?.RegisterServiceWorker == true)
                return string.Format(_registerScriptTemplate, _options.ServiceWorkerRoute);
            else
                return string.Empty;
        }
    }
}
