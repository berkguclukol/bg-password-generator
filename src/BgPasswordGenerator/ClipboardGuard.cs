using Windows.ApplicationModel.DataTransfer;

namespace BgPasswordGenerator;

/// <summary>
/// Copies passwords so they stay out of Windows clipboard history (Win+V) and cloud clipboard sync,
/// and clears them again after <see cref="ClearDelay"/>.
/// </summary>
internal sealed class ClipboardGuard
{
    public static readonly TimeSpan ClearDelay = TimeSpan.FromSeconds(30);

    private string? _lastCopied;

    public bool HasCopied => _lastCopied is not null;

    public void Copy(string text)
    {
        var package = new DataPackage();
        package.SetText(text);
        Clipboard.SetContentWithOptions(package, new ClipboardContentOptions
        {
            IsAllowedInHistory = false,
            IsRoamable = false,
        });
        _lastCopied = text;
        _ = ClearLaterAsync(text);
    }

    public Task ClearLastCopiedIfStillThereAsync() =>
        _lastCopied is null ? Task.CompletedTask : ClearIfOursAsync(_lastCopied);

    private async Task ClearLaterAsync(string text)
    {
        await Task.Delay(ClearDelay);
        await ClearIfOursAsync(text);
    }

    // Clear only if the clipboard still holds our password, so later copies by the user aren't wiped
    private static async Task ClearIfOursAsync(string text)
    {
        try
        {
            var content = Clipboard.GetContent();
            if (content.Contains(StandardDataFormats.Text) && await content.GetTextAsync() == text)
            {
                Clipboard.Clear();
            }
        }
        catch (Exception)
        {
            // Another process can hold the clipboard open; skipping one clear is better than crashing
        }
    }
}
