using BgPasswordGenerator.Core;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics;

namespace BgPasswordGenerator;

public sealed partial class MainWindow : Window
{
    private const int DefaultLength = 18;
    private const int WindowWidth = 640;
    private const int WindowHeight = 660;
    private const double PasswordFontSize = 26;
    private const double UuidFontSize = 18;
    private static readonly TimeSpan CopiedNoticeDuration = TimeSpan.FromSeconds(3);

    private readonly ClipboardGuard _clipboard = new();
    private readonly AppUpdater _updater = AppUpdater.FromGitHub();
    private readonly DispatcherTimer _copiedNoticeTimer = new() { Interval = CopiedNoticeDuration };
    private bool _clipboardCheckedOnClose;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "icon.ico"));
        SizeWindow();
        VersionText.Text = $"v{typeof(MainWindow).Assembly.GetName().Version?.ToString(3)}";

        LengthSlider.Minimum = PasswordGenerator.MinLength;
        LengthSlider.Maximum = PasswordGenerator.MaxLength;
        LengthSlider.Value = DefaultLength;
        LengthSlider.ValueChanged += (_, _) => UpdateLengthText();
        UpdateLengthText();

        ModeSelector.SelectionChanged += (_, _) => ApplyMode();

        _copiedNoticeTimer.Tick += (_, _) =>
        {
            _copiedNoticeTimer.Stop();
            CopiedInfoBar.IsOpen = false;
        };
        AppWindow.Closing += OnClosing;

        ApplyMode();
        _ = CheckForUpdatesAsync();
    }

    private bool IsUuidMode => ModeSelector.SelectedItem == UuidModeItem;

    private void SizeWindow()
    {
        // AppWindow works in physical pixels, so scale the logical size by the monitor DPI
        var scale = GetDpiForWindow(WinRT.Interop.WindowNative.GetWindowHandle(this)) / 96.0;
        var width = (int)(WindowWidth * scale);
        var height = (int)(WindowHeight * scale);
        AppWindow.Resize(new SizeInt32(width, height));
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumWidth = width;
            presenter.PreferredMinimumHeight = height;
        }
    }

    private void ApplyMode()
    {
        var uuid = IsUuidMode;
        PasswordOptions.Visibility = uuid ? Visibility.Collapsed : Visibility.Visible;
        UuidOptions.Visibility = uuid ? Visibility.Visible : Visibility.Collapsed;
        OptionsTitle.Text = uuid ? "Customize your UUID." : "Customize your password.";
        GenerateButtonText.Text = uuid ? "Generate UUID" : "Generate Password";
        // A 36-character UUID doesn't fit at the password size
        ResultText.FontSize = uuid ? UuidFontSize : PasswordFontSize;
        CopiedInfoBar.IsOpen = false;
        Render();
    }

    private void UpdateLengthText() => LengthText.Text = $"Password length : {(int)LengthSlider.Value}";

    private void Render()
    {
        if (IsUuidMode)
        {
            ResultText.Text = UuidGenerator.Generate(UuidUppercaseToggle.IsOn, UuidHyphensToggle.IsOn);
            DetailText.Text = "Random UUID (version 4)";
            return;
        }

        ResultText.Text = PasswordGenerator.Generate(
            (int)LengthSlider.Value,
            LowercaseToggle.IsOn,
            UppercaseToggle.IsOn,
            NumbersToggle.IsOn,
            SymbolsToggle.IsOn);
        DetailText.Text = $"Strength: {PasswordStrength.Evaluate(ResultText.Text)}";
    }

    private void OnGenerateClick(object sender, RoutedEventArgs e) => Render();

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (ResultText.Text.Length == 0)
        {
            return;
        }

        // UUIDs aren't secrets, so they're copied normally instead of being hidden from history and cleared
        if (IsUuidMode)
        {
            ClipboardGuard.CopyPlain(ResultText.Text);
            CopiedInfoBar.Message = "Copied to the clipboard.";
        }
        else
        {
            _clipboard.Copy(ResultText.Text);
            CopiedInfoBar.Message = "Copied to the clipboard. It will be cleared in 30 seconds.";
        }
        CopiedInfoBar.IsOpen = true;
        _copiedNoticeTimer.Stop();
        _copiedNoticeTimer.Start();
    }

    private async Task CheckForUpdatesAsync()
    {
        if (!_updater.CanUpdate)
        {
            return;
        }
        try
        {
            var version = await _updater.CheckAsync();
            if (version is null)
            {
                return;
            }
            UpdateInfoBar.Message = $"Version {version} is available.";
            UpdateInfoBar.IsOpen = true;
        }
        catch (Exception)
        {
            // Offline or GitHub unreachable: the generator works fine without updates, so stay quiet
        }
    }

    private async void OnUpdateClick(object sender, RoutedEventArgs e)
    {
        UpdateButton.IsEnabled = false;
        UpdateInfoBar.IsClosable = false;
        try
        {
            await _updater.DownloadAsync(progress =>
                DispatcherQueue.TryEnqueue(() => UpdateInfoBar.Message = $"Downloading update… {progress}%"));
            _updater.ApplyAndRestart();
        }
        catch (Exception ex)
        {
            UpdateInfoBar.Severity = InfoBarSeverity.Error;
            UpdateInfoBar.Title = "Update failed";
            UpdateInfoBar.Message = ex.Message;
            UpdateInfoBar.IsClosable = true;
            UpdateButton.IsEnabled = true;
        }
    }

    // Clipboard reads are async, so hold the close until the check finishes, then close again
    private async void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (!_clipboard.HasCopied || _clipboardCheckedOnClose)
        {
            return;
        }
        args.Cancel = true;
        _clipboardCheckedOnClose = true;
        await _clipboard.ClearLastCopiedIfStillThereAsync();
        Close();
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(nint hwnd);
}
