using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace BgPasswordGenerator;

/// <summary>What a launch asks for: the taskbar's jump list starts the app with one of these arguments.</summary>
internal enum StartupCommand
{
    Open,
    OpenHash,
    CopyPassword,
    CopyUuid,
}

internal static class StartupCommands
{
    public const string CopyPasswordArgument = "--copy-password";
    public const string CopyUuidArgument = "--copy-uuid";
    public const string OpenHashArgument = "--hash";

    public static StartupCommand Parse(IEnumerable<string> arguments)
    {
        var set = arguments.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return set.Contains(CopyPasswordArgument) ? StartupCommand.CopyPassword
            : set.Contains(CopyUuidArgument) ? StartupCommand.CopyUuid
            : set.Contains(OpenHashArgument) ? StartupCommand.OpenHash
            : StartupCommand.Open;
    }

    // A redirected launch carries its whole command line as one string; the flags never contain spaces
    public static StartupCommand Parse(AppActivationArguments activation) =>
        activation.Data is ILaunchActivatedEventArgs launch
            ? Parse(launch.Arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            : StartupCommand.Open;
}
