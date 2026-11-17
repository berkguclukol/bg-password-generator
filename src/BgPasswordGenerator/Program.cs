using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Velopack;

namespace BgPasswordGenerator;

public static class Program
{
    private const string InstanceKey = "main";

    [STAThread]
    private static void Main(string[] args)
    {
        // Handles Velopack's install/update/uninstall hooks and exits early when one is running
        VelopackApp.Build().Run();

        WinRT.ComWrappersSupport.InitializeComWrappers();
        if (RedirectToRunningInstance())
        {
            return;
        }

        Application.Start(initParams =>
        {
            SynchronizationContext.SetSynchronizationContext(
                new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
            _ = new App();
        });
    }

    // Taskbar tasks and second launches go to the copy that's already running, so there's one window
    // and one process owning the clipboard
    private static bool RedirectToRunningInstance()
    {
        var mainInstance = AppInstance.FindOrRegisterForKey(InstanceKey);
        if (mainInstance.IsCurrent)
        {
            return false;
        }

        // This launch came from the user, so it may hand its right to take the foreground to the running copy
        AllowSetForegroundWindow(mainInstance.ProcessId);
        var activation = AppInstance.GetCurrent().GetActivatedEventArgs();
        var redirected = CreateEvent(0, manualReset: true, initialState: false, null);
        try
        {
            // The STA thread has to keep pumping COM messages while it waits, hence CoWaitForMultipleObjects
            Task.Run(() =>
            {
                mainInstance.RedirectActivationToAsync(activation).AsTask().Wait();
                SetEvent(redirected);
            });
            CoWaitForMultipleObjects(CwmoDefault, Infinite, 1, [redirected], out _);
        }
        finally
        {
            CloseHandle(redirected);
        }
        return true;
    }

    private const uint CwmoDefault = 0;
    private const uint Infinite = 0xFFFFFFFF;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint CreateEvent(nint attributes, bool manualReset, bool initialState, string? name);

    [DllImport("kernel32.dll")]
    private static extern bool SetEvent(nint handle);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(nint handle);

    [DllImport("ole32.dll")]
    private static extern int CoWaitForMultipleObjects(uint flags, uint timeout, uint count, nint[] handles, out uint index);

    [DllImport("user32.dll")]
    private static extern bool AllowSetForegroundWindow(uint processId);
}
