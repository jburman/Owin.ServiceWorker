namespace Owin.ServiceWorker
{
    public interface IServiceWorkerConfig
    {
        string this[string key] { get; }
    }
}
