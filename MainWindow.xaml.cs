using System;
using System.Collections.Generic;
using System.Drawing; // For SystemIcons
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms; // For NotifyIcon
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

namespace CopyAlert
{
    public partial class MainWindow : Window
    {
        private const int TextPreviewLength = 60;
        private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".webp", ".svg", ".ico", ".heic", ".heif", ".avif", ".jfif"
        };

        private ClipboardMonitor? _clipboardMonitor;
        private DispatcherTimer _hideTimer;
        private bool _isFirstLoad = true;
        private DateTime _lastUpdate = DateTime.MinValue;
        private NotifyIcon? _notifyIcon;
        private System.Drawing.Icon? _trayIcon;
        private AppSettings _settings;
        private SettingsWindow? _settingsWindowRef;
        private bool _isDragMode = false;
        private NotificationPayload _currentPayload = NotificationPayload.Generic();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private sealed class NotificationPayload
        {
            public string CompactText { get; init; } = "Copied to Clipboard";
            public string Title { get; init; } = "Copied to Clipboard";
            public string Detail { get; init; } = "Clipboard content updated.";
            public PackIconKind Icon { get; init; } = PackIconKind.ClipboardCheckOutline;

            public static NotificationPayload Generic()
            {
                return new NotificationPayload();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
            _hideTimer = new DispatcherTimer();
            _hideTimer.Interval = TimeSpan.FromSeconds(2);
            _hideTimer.Tick += HideTimer_Tick;

            _settings = AppSettings.Load();
            SetupSystemTray();
        }

        private void SetupSystemTray()
        {
            _notifyIcon = new NotifyIcon();
            
            try 
            {
                System.Windows.Resources.StreamResourceInfo iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/CopyAlert_Logo.png"));
                if (iconStream != null)
                {
                    _trayIcon = CreateCrispTrayIcon(iconStream.Stream);
                    _notifyIcon.Icon = _trayIcon;
                }
                else
                {
                    _notifyIcon.Icon = SystemIcons.Information;
                }
            }
            catch 
            {
                _notifyIcon.Icon = SystemIcons.Information;
            }

            _notifyIcon.Visible = true;
            _notifyIcon.Text = "CopyAlert is running";

            ContextMenuStrip menu = new ContextMenuStrip();
            
            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += (s, e) => { ShowSettingsWindow(); };
            menu.Items.Add(settingsItem);

            menu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => { System.Windows.Application.Current.Shutdown(); };
            menu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = menu;
            
            // Double click opens settings too
            _notifyIcon.DoubleClick += (s, e) => { ShowSettingsWindow(); };
        }

        private static System.Drawing.Icon CreateCrispTrayIcon(Stream logoStream)
        {
            using Bitmap source = new Bitmap(logoStream);

            float dpiScale = 1.0f;
            using (Graphics desktopGraphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiScale = Math.Max(1.0f, desktopGraphics.DpiX / 96.0f);
            }

            int targetSize = Math.Clamp((int)Math.Round(16 * dpiScale), 16, 32);
            using Bitmap resized = new Bitmap(targetSize, targetSize, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.Clear(Color.Transparent);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle destination = GetLetterboxedRect(source.Width, source.Height, targetSize, targetSize);
                g.DrawImage(source, destination);
            }

            IntPtr hIcon = resized.GetHicon();
            try
            {
                using System.Drawing.Icon nativeIcon = System.Drawing.Icon.FromHandle(hIcon);
                return (System.Drawing.Icon)nativeIcon.Clone();
            }
            finally
            {
                DestroyIcon(hIcon);
            }
        }

        private static Rectangle GetLetterboxedRect(int sourceWidth, int sourceHeight, int targetWidth, int targetHeight)
        {
            if (sourceWidth <= 0 || sourceHeight <= 0)
            {
                return new Rectangle(0, 0, targetWidth, targetHeight);
            }

            float ratio = Math.Min(targetWidth / (float)sourceWidth, targetHeight / (float)sourceHeight);
            int width = Math.Max(1, (int)Math.Round(sourceWidth * ratio));
            int height = Math.Max(1, (int)Math.Round(sourceHeight * ratio));
            int x = (targetWidth - width) / 2;
            int y = (targetHeight - height) / 2;

            return new Rectangle(x, y, width, height);
        }

        private void ShowSettingsWindow()
        {
            if (_settingsWindowRef == null || !_settingsWindowRef.IsLoaded)
            {
                _settingsWindowRef = new SettingsWindow(this);
            }
            _settingsWindowRef.Show();
            _settingsWindowRef.Activate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplySettings();

            _clipboardMonitor = new ClipboardMonitor(this);
            _clipboardMonitor.ClipboardUpdate += ClipboardMonitor_ClipboardUpdate;
        }

        public void ApplySettings()
        {
            _settings = AppSettings.Load();
            ApplyNotificationStyle();

            var desktopWorkingArea = SystemParameters.WorkArea;

            switch (_settings.Location)
            {
                case LocationType.BottomRight:
                    this.Left = desktopWorkingArea.Right - this.Width;
                    this.Top = desktopWorkingArea.Bottom - this.Height;
                    break;
                case LocationType.BottomLeft:
                    this.Left = desktopWorkingArea.Left;
                    this.Top = desktopWorkingArea.Bottom - this.Height;
                    break;
                case LocationType.TopRight:
                    this.Left = desktopWorkingArea.Right - this.Width;
                    this.Top = desktopWorkingArea.Top;
                    break;
                case LocationType.TopLeft:
                    this.Left = desktopWorkingArea.Left;
                    this.Top = desktopWorkingArea.Top;
                    break;
                case LocationType.Custom:
                    this.Left = _settings.CustomX;
                    this.Top = _settings.CustomY;
                    break;
            }
        }

        private void ClipboardMonitor_ClipboardUpdate(object? sender, EventArgs e)
        {
            if (_isDragMode) return; // don't interfere if dragging

            if ((DateTime.Now - _lastUpdate).TotalMilliseconds < 500)
                return;
            
            if (_isFirstLoad)
            {
                _isFirstLoad = false;
                return;
            }

            _lastUpdate = DateTime.Now;
            _currentPayload = Dispatcher.CheckAccess()
                ? BuildNotificationPayload()
                : Dispatcher.Invoke(BuildNotificationPayload);
            ShowPopup();
        }

        private void ShowPopup()
        {
            Dispatcher.Invoke(() =>
            {
                _hideTimer.Stop();

                ApplyNotificationStyle();
                UpdatePopupContent(_currentPayload);
                AnimateShow();

                _hideTimer.Start();
            });
        }

        private void HideTimer_Tick(object? sender, EventArgs e)
        {
            _hideTimer.Stop();
            if (_isDragMode)
            {
                return;
            }

            AnimateHide();
        }

        public void EnterDragMode(SettingsWindow caller)
        {
            _isDragMode = true;
            _hideTimer.Stop();
            _settingsWindowRef = caller;

            // Show UI for drag
            CompactContentPanel.Visibility = Visibility.Collapsed;
            FullCardContentPanel.Visibility = Visibility.Collapsed;
            DragContentPanel.Visibility = Visibility.Visible;
            
            // Force opaque
            StopPopupAnimations();
            PopupBorder.Opacity = 1;
            PopupTranslateTransform.X = 0;
            PopupTranslateTransform.Y = 0;

            this.Show();
            this.Activate();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isDragMode)
            {
                this.DragMove();
            }
        }

        private void SavePosition_Click(object sender, RoutedEventArgs e)
        {
            if (_isDragMode)
            {
                _isDragMode = false;
                
                // Switch back UI
                ApplyNotificationStyle();
                DragContentPanel.Visibility = Visibility.Collapsed;
                PopupBorder.Opacity = 0;
                PopupTranslateTransform.X = 0;
                PopupTranslateTransform.Y = 0;

                // Return values
                if (_settingsWindowRef != null)
                {
                    _settingsWindowRef.OnDragModeComplete(this.Left, this.Top);
                }
            }
        }

        private void ApplyNotificationStyle()
        {
            if (_isDragMode)
            {
                return;
            }

            if (_settings.Style == NotificationStyle.FullCard)
            {
                CompactContentPanel.Visibility = Visibility.Collapsed;
                FullCardContentPanel.Visibility = Visibility.Visible;
                Width = 360;
                Height = 120;
            }
            else
            {
                CompactContentPanel.Visibility = Visibility.Visible;
                FullCardContentPanel.Visibility = Visibility.Collapsed;
                Width = 260;
                Height = 70;
            }
        }

        private void UpdatePopupContent(NotificationPayload payload)
        {
            CompactIcon.Kind = payload.Icon;
            FullCardIcon.Kind = payload.Icon;

            CompactTitleText.Text = payload.CompactText;
            FullCardTitleText.Text = payload.Title;
            FullCardDetailText.Text = payload.Detail;
        }

        private NotificationPayload BuildNotificationPayload()
        {
            bool showDetailed = _settings.ShowDetailedClipboardInfo;

            try
            {
                System.Windows.IDataObject dataObject = System.Windows.Clipboard.GetDataObject();
                if (dataObject == null)
                {
                    return NotificationPayload.Generic();
                }

                if (TryBuildFilePayload(dataObject, showDetailed, out NotificationPayload filePayload))
                {
                    return filePayload;
                }

                if (dataObject.GetDataPresent(System.Windows.DataFormats.Bitmap))
                {
                    if (!showDetailed)
                    {
                        return new NotificationPayload
                        {
                            CompactText = "Image copied",
                            Title = "Image copied",
                            Detail = "Image copied.",
                            Icon = PackIconKind.ImageOutline
                        };
                    }

                    return new NotificationPayload
                    {
                        CompactText = "Image copied",
                        Title = "Image copied",
                        Detail = "Bitmap content is ready to paste.",
                        Icon = PackIconKind.ImageOutline
                    };
                }

                if (dataObject.GetDataPresent(System.Windows.DataFormats.UnicodeText) || dataObject.GetDataPresent(System.Windows.DataFormats.Text))
                {
                    string text = TryGetClipboardText(dataObject);
                    return showDetailed ? BuildTextPayload(text) : BuildGeneralTextPayload(text);
                }
            }
            catch
            {
                return NotificationPayload.Generic();
            }

            return NotificationPayload.Generic();
        }

        private bool TryBuildFilePayload(System.Windows.IDataObject dataObject, bool showDetailed, out NotificationPayload payload)
        {
            payload = NotificationPayload.Generic();

            if (!dataObject.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                return false;
            }

            if (dataObject.GetData(System.Windows.DataFormats.FileDrop) is not string[] files || files.Length == 0)
            {
                return false;
            }

            string firstName = Path.GetFileName(files[0]);
            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = files[0];
            }

            bool singleFile = files.Length == 1;
            bool allImageFiles = AreAllImageFiles(files);

            if (!showDetailed)
            {
                payload = new NotificationPayload
                {
                    CompactText = allImageFiles ? "Image copied" : (singleFile ? "File copied" : "Files copied"),
                    Title = allImageFiles ? "Image copied" : (singleFile ? "File copied" : "Files copied"),
                    Detail = allImageFiles ? "Image file copied." : (singleFile ? "File copied." : "Files copied."),
                    Icon = allImageFiles ? PackIconKind.ImageOutline : PackIconKind.FileOutline
                };

                return true;
            }

            payload = new NotificationPayload
            {
                CompactText = allImageFiles ? (singleFile ? "Image copied" : $"{files.Length} images copied") : (singleFile ? "File copied" : $"{files.Length} files copied"),
                Title = allImageFiles ? (singleFile ? "Image copied" : "Images copied") : (singleFile ? "File copied" : "Files copied"),
                Detail = singleFile
                    ? firstName
                    : (allImageFiles ? $"{files.Length} image files copied (first: {firstName})" : $"{files.Length} files copied (first: {firstName})"),
                Icon = allImageFiles ? PackIconKind.ImageOutline : PackIconKind.FileOutline
            };

            return true;
        }

        private string TryGetClipboardText(System.Windows.IDataObject dataObject)
        {
            try
            {
                if (System.Windows.Clipboard.ContainsText(System.Windows.TextDataFormat.UnicodeText))
                {
                    return System.Windows.Clipboard.GetText(System.Windows.TextDataFormat.UnicodeText);
                }

                if (dataObject.GetData(System.Windows.DataFormats.UnicodeText) is string unicodeText)
                {
                    return unicodeText;
                }

                if (dataObject.GetData(System.Windows.DataFormats.Text) is string text)
                {
                    return text;
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        private NotificationPayload BuildTextPayload(string rawText)
        {
            string normalizedText = NormalizePreviewText(rawText);
            if (string.IsNullOrWhiteSpace(normalizedText))
            {
                return new NotificationPayload
                {
                    CompactText = "Text copied",
                    Title = "Text copied",
                    Detail = "(empty text)",
                    Icon = PackIconKind.TextBoxOutline
                };
            }

            string preview = TruncatePreview(normalizedText, TextPreviewLength);
            if (IsAbsoluteUrl(normalizedText))
            {
                return new NotificationPayload
                {
                    CompactText = $"Link: {TruncatePreview(preview, 36)}",
                    Title = "Link copied",
                    Detail = preview,
                    Icon = PackIconKind.LinkVariant
                };
            }

            return new NotificationPayload
            {
                CompactText = $"Text: {TruncatePreview(preview, 36)}",
                Title = "Text copied",
                Detail = preview,
                Icon = PackIconKind.TextBoxOutline
            };
        }

        private NotificationPayload BuildGeneralTextPayload(string rawText)
        {
            string normalizedText = NormalizePreviewText(rawText);
            if (IsAbsoluteUrl(normalizedText))
            {
                return new NotificationPayload
                {
                    CompactText = "Link copied",
                    Title = "Link copied",
                    Detail = "Link copied.",
                    Icon = PackIconKind.LinkVariant
                };
            }

            return new NotificationPayload
            {
                CompactText = "Text copied",
                Title = "Text copied",
                Detail = "Text copied.",
                Icon = PackIconKind.TextBoxOutline
            };
        }

        private static bool AreAllImageFiles(IEnumerable<string> files)
        {
            bool hasAny = false;

            foreach (string file in files)
            {
                hasAny = true;
                string extension = Path.GetExtension(file);
                if (string.IsNullOrWhiteSpace(extension) || !ImageExtensions.Contains(extension))
                {
                    return false;
                }
            }

            return hasAny;
        }

        private static string NormalizePreviewText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            string normalized = text.Replace("\r\n", " ")
                                    .Replace('\n', ' ')
                                    .Replace('\r', ' ')
                                    .Replace('\t', ' ')
                                    .Trim();

            while (normalized.Contains("  "))
            {
                normalized = normalized.Replace("  ", " ");
            }

            return normalized;
        }

        private static string TruncatePreview(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            {
                return text;
            }

            return text.Substring(0, maxLength).TrimEnd() + "...";
        }

        private static bool IsAbsoluteUrl(string text)
        {
            if (!Uri.TryCreate(text, UriKind.Absolute, out Uri? uri) || uri == null)
            {
                return false;
            }

            return uri.Scheme == Uri.UriSchemeHttp
                   || uri.Scheme == Uri.UriSchemeHttps
                   || uri.Scheme == Uri.UriSchemeFtp
                   || uri.Scheme == Uri.UriSchemeFile
                   || uri.Scheme == Uri.UriSchemeMailto;
        }

        private void AnimateShow()
        {
            StopPopupAnimations();

            var easeOut = new CubicEase { EasingMode = EasingMode.EaseOut };
            var (offsetX, offsetY) = GetAnimationOffset();

            switch (_settings.Animation)
            {
                case PopupAnimationMode.Fade:
                    PopupTranslateTransform.X = 0;
                    PopupTranslateTransform.Y = 0;
                    PopupBorder.BeginAnimation(
                        UIElement.OpacityProperty,
                        CreateAnimation(0, 1, 210, easeOut));
                    break;

                case PopupAnimationMode.Slide:
                    PopupTranslateTransform.X = offsetX;
                    PopupTranslateTransform.Y = offsetY;
                    PopupBorder.Opacity = 0;

                    PopupTranslateTransform.BeginAnimation(
                        System.Windows.Media.TranslateTransform.XProperty,
                        CreateAnimation(offsetX, 0, 260, easeOut));
                    PopupTranslateTransform.BeginAnimation(
                        System.Windows.Media.TranslateTransform.YProperty,
                        CreateAnimation(offsetY, 0, 260, easeOut));
                    PopupBorder.BeginAnimation(
                        UIElement.OpacityProperty,
                        CreateAnimation(0, 1, 220, easeOut));
                    break;

                default:
                    double smartFromX = offsetX * 0.75;
                    double smartFromY = offsetY * 0.75;

                    PopupTranslateTransform.X = smartFromX;
                    PopupTranslateTransform.Y = smartFromY;
                    PopupBorder.Opacity = 0;

                    PopupTranslateTransform.BeginAnimation(
                        System.Windows.Media.TranslateTransform.XProperty,
                        CreateAnimation(smartFromX, 0, 300, easeOut));
                    PopupTranslateTransform.BeginAnimation(
                        System.Windows.Media.TranslateTransform.YProperty,
                        CreateAnimation(smartFromY, 0, 300, easeOut));
                    PopupBorder.BeginAnimation(
                        UIElement.OpacityProperty,
                        CreateAnimation(0, 1, 240, easeOut));
                    break;
            }
        }

        private void AnimateHide()
        {
            var easeIn = new CubicEase { EasingMode = EasingMode.EaseIn };
            var (offsetX, offsetY) = GetAnimationOffset();

            switch (_settings.Animation)
            {
                case PopupAnimationMode.Fade:
                    PopupTranslateTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, null);
                    PopupTranslateTransform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, null);
                    PopupTranslateTransform.X = 0;
                    PopupTranslateTransform.Y = 0;

                    PopupBorder.BeginAnimation(
                        UIElement.OpacityProperty,
                        CreateAnimation(PopupBorder.Opacity, 0, 240, easeIn));
                    break;

                case PopupAnimationMode.Slide:
                    PopupTranslateTransform.BeginAnimation(
                        System.Windows.Media.TranslateTransform.XProperty,
                        CreateAnimation(PopupTranslateTransform.X, offsetX, 220, easeIn));
                    PopupTranslateTransform.BeginAnimation(
                        System.Windows.Media.TranslateTransform.YProperty,
                        CreateAnimation(PopupTranslateTransform.Y, offsetY, 220, easeIn));
                    PopupBorder.BeginAnimation(
                        UIElement.OpacityProperty,
                        CreateAnimation(PopupBorder.Opacity, 0, 190, easeIn));
                    break;

                default:
                    double smartToX = offsetX * 0.55;
                    double smartToY = offsetY * 0.55;

                    PopupTranslateTransform.BeginAnimation(
                        System.Windows.Media.TranslateTransform.XProperty,
                        CreateAnimation(PopupTranslateTransform.X, smartToX, 230, easeIn));
                    PopupTranslateTransform.BeginAnimation(
                        System.Windows.Media.TranslateTransform.YProperty,
                        CreateAnimation(PopupTranslateTransform.Y, smartToY, 230, easeIn));
                    PopupBorder.BeginAnimation(
                        UIElement.OpacityProperty,
                        CreateAnimation(PopupBorder.Opacity, 0, 210, easeIn));
                    break;
            }
        }

        private static DoubleAnimation CreateAnimation(double from, double to, int milliseconds, IEasingFunction easing)
        {
            return new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(milliseconds),
                EasingFunction = easing
            };
        }

        private (double x, double y) GetAnimationOffset()
        {
            return _settings.Location switch
            {
                LocationType.BottomRight => (18, 18),
                LocationType.BottomLeft => (-18, 18),
                LocationType.TopRight => (18, -18),
                LocationType.TopLeft => (-18, -18),
                _ => (0, 18)
            };
        }

        private void StopPopupAnimations()
        {
            PopupBorder.BeginAnimation(UIElement.OpacityProperty, null);
            PopupTranslateTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, null);
            PopupTranslateTransform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, null);
        }

        protected override void OnClosed(EventArgs e)
        {
            _notifyIcon?.Dispose();
            _trayIcon?.Dispose();
            _clipboardMonitor?.Dispose();
            base.OnClosed(e);
        }
    }
}