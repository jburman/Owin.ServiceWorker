using System;
using System.Collections.Specialized;

namespace Owin.ServiceWorker
{
    public class DefaultServiceWorkerConfig : IServiceWorkerConfig
    {
        private NameValueCollection _values;

        public DefaultServiceWorkerConfig(NameValueCollection values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public string this[string key]
        {
            get => _values[key];
        }
    }
}
