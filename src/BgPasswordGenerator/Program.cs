using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Velopack;

namespace BgPasswordGenerator;

public static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        // Handles Velopack's install/update/uninstall hooks and exits early when one is running
        VelopackApp.Build().Run();

        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start(initParams =>
        {
            SynchronizationContext.SetSynchronizationContext(
                new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
            _ = new App();
        });
    }
}
