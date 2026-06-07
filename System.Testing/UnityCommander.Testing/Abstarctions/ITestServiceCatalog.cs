namespace UnityCommander.Testing.Abstarctions
{
    public interface ITestServiceCatalog
    {
        void RegisterInstance<TService>(TService instance);
        void RegisterSingleton<TService>(Func<ITestServiceCatalog, TService> factory);

        TService Resolve<TService>();
    }
}
