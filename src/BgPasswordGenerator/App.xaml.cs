using BgPasswordGenerator.Core;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace BgPasswordGenerator;

public partial class App : Application
{
    // Shared with the window so closing it also clears a password copied from the taskbar
    private readonly ClipboardGuard _clipboard = new();
    private MainWindow? _window;

    public App()
    {
        InitializeComponent();
        // Copy tasks run without a window, so the app decides when to exit rather than WinUI
        DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Later launches (taskbar tasks, a second double-click) are redirected here by Program.Main
        var dispatcher = DispatcherQueue.GetForCurrentThread();
        AppInstance.GetCurrent().Activated += (_, activation) =>
            dispatcher.TryEnqueue(() => Run(StartupCommands.Parse(activation)));

        JumpList.Register();
        Run(StartupCommands.Parse(Environment.GetCommandLineArgs()));
    }

    private async void Run(StartupCommand command)
    {
        switch (command)
        {
            case StartupCommand.CopyPassword:
                // Default settings, since the app doesn't save any. The process stays alive until the
                // clipboard is cleared: Windows asks the copying process for the text when it's pasted.
                await _clipboard.Copy(PasswordGenerator.Generate(PasswordGenerator.DefaultLength, true, true, true, true));
                ExitIfIdle();
                break;
            case StartupCommand.CopyUuid:
                ClipboardGuard.CopyPlain(UuidGenerator.Generate(uppercase: false, hyphens: true));
                ExitIfIdle();
                break;
            case StartupCommand.OpenHash:
                ShowWindow().ShowHash();
                break;
            default:
                ShowWindow();
                break;
        }
    }

    private MainWindow ShowWindow()
    {
        if (_window is null)
        {
            _window = new MainWindow(_clipboard);
            // The window already clears its password from the clipboard while closing
            _window.Closed += (_, _) => Exit();
        }
        _window.BringToFront();
        return _window;
    }

    private void ExitIfIdle()
    {
        if (_window is null && !_clipboard.HasPendingClear)
        {
            Exit();
        }
    }
}
