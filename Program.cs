using Topshelf;

namespace FolderLinker
{
    class Program
    {
        static void Main(string[] args)
        {
            
            HostFactory.Run(configure =>
            {
                configure.Service<Service>();
                configure.EnableServiceRecovery(x => x.RestartService(10));
                configure.SetServiceName("FolderLinker");
                configure.SetDisplayName("FolderLinker");
                configure.SetDescription("Watch directories and create junction link to destination.");
                configure.StartAutomatically();
                configure.RunAsLocalSystem();
                configure.OnException(e =>
                {
                    string exception = "Unhandled Exception:\n" + e.StackTrace;
                    Service.Log.Write(exception, LogType.Error);
                });
            });
        }
    }
}
