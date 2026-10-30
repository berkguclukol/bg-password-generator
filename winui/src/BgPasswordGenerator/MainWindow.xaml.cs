using BgPasswordGenerator.Core;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace BgPasswordGenerator;

public sealed partial class MainWindow : Window
{
    private const int DefaultLength = 18;
    private const int WindowWidth = 640;
    private const int WindowHeight = 600;
    private static readonly TimeSpan CopiedNoticeDuration = TimeSpan.FromSeconds(3);

    private readonly ClipboardGuard _clipboard = new();
    private readonly DispatcherTimer _copiedNoticeTimer = new() { Interval = CopiedNoticeDuration };
    private bool _clipboardCheckedOnClose;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "icon.ico"));
        SizeWindow();

        LengthSlider.Minimum = PasswordGenerator.MinLength;
        LengthSlider.Maximum = PasswordGenerator.MaxLength;
        LengthSlider.Value = DefaultLength;
        LengthSlider.ValueChanged += (_, _) => UpdateLengthText();
        UpdateLengthText();

        _copiedNoticeTimer.Tick += (_, _) =>
        {
            _copiedNoticeTimer.Stop();
            CopiedInfoBar.IsOpen = false;
        };
        AppWindow.Closing += OnClosing;

        RenderPassword();
    }

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

    private void UpdateLengthText() => LengthText.Text = $"Password length : {(int)LengthSlider.Value}";

    private void RenderPassword()
    {
        PasswordText.Text = PasswordGenerator.Generate(
            (int)LengthSlider.Value,
            LowercaseToggle.IsOn,
            UppercaseToggle.IsOn,
            NumbersToggle.IsOn,
            SymbolsToggle.IsOn);
        StrengthText.Text = $"Strength: {PasswordStrength.Evaluate(PasswordText.Text)}";
    }

    private void OnGenerateClick(object sender, RoutedEventArgs e) => RenderPassword();

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (PasswordText.Text.Length == 0)
        {
            return;
        }
        _clipboard.Copy(PasswordText.Text);
        CopiedInfoBar.IsOpen = true;
        _copiedNoticeTimer.Stop();
        _copiedNoticeTimer.Start();
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
