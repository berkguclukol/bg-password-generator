using Velopack;
using Velopack.Sources;

namespace BgPasswordGenerator;

/// <summary>
/// Checks this repository's GitHub releases for a newer version and installs it through Velopack.
/// </summary>
internal sealed class AppUpdater
{
    private const string RepositoryUrl = "https://github.com/berkguclukol/bg-password-generator";

    private readonly UpdateManager _manager;
    private UpdateInfo? _update;

    public AppUpdater(IUpdateSource source)
    {
        _manager = new UpdateManager(source);
    }

    public static AppUpdater FromGitHub() => new(new GithubSource(RepositoryUrl, accessToken: null, prerelease: false));

    // Only copies installed by Velopack (Setup or Portable) can update themselves; dev builds can't
    public bool CanUpdate => _manager.IsInstalled;

    /// <summary>Returns the newer version, or null when this one is the latest.</summary>
    public async Task<string?> CheckAsync()
    {
        _update = await _manager.CheckForUpdatesAsync();
        return _update?.TargetFullRelease.Version.ToString();
    }

    public Task DownloadAsync(Action<int> onProgress) =>
        _manager.DownloadUpdatesAsync(PendingUpdate(), onProgress);

    public void ApplyAndRestart() => _manager.ApplyUpdatesAndRestart(PendingUpdate().TargetFullRelease);

    private UpdateInfo PendingUpdate() =>
        _update ?? throw new InvalidOperationException("No update found; call CheckAsync first.");
}
