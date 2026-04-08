using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace CopyAlert
{
    public partial class SettingsWindow : Window
    {
        private AppSettings _settings = AppSettings.Load();
        private MainWindow _mainWindow;

        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _settings = AppSettings.Load();
            SelectComboBoxItem(LocationTypeCombo, _settings.Location);
            SelectComboBoxItem(NotificationStyleCombo, _settings.Style);
            SelectComboBoxItem(AnimationModeCombo, _settings.Animation);
            CustomXBox.Text = _settings.CustomX.ToString();
            CustomYBox.Text = _settings.CustomY.ToString();
            ShowDetailedInfoCheck.IsChecked = _settings.ShowDetailedClipboardInfo;
            AutoStartCheck.IsChecked = _settings.AutoStart;
        }

        private void SelectComboBoxItem(System.Windows.Controls.ComboBox comboBox, Enum enumValue)
        {
            foreach (System.Windows.Controls.ComboBoxItem item in comboBox.Items)
            {
                if (item != null && item.Tag != null && item.Tag.ToString() == enumValue.ToString())
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void LocationTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LocationTypeCombo.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                string? tag = item.Tag.ToString();
                if (CustomCoordinatesPanel != null)
                {
                    if (tag == "Custom")
                    {
                        CustomCoordinatesPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        CustomCoordinatesPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationTypeCombo.SelectedItem is System.Windows.Controls.ComboBoxItem item && item.Tag != null)
            {
                if (Enum.TryParse(item.Tag.ToString(), out LocationType loc))
                {
                    _settings.Location = loc;
                }
            }

            if (NotificationStyleCombo.SelectedItem is System.Windows.Controls.ComboBoxItem styleItem && styleItem.Tag != null)
            {
                if (Enum.TryParse(styleItem.Tag.ToString(), out NotificationStyle style))
                {
                    _settings.Style = style;
                }
            }

            if (AnimationModeCombo.SelectedItem is System.Windows.Controls.ComboBoxItem animationItem && animationItem.Tag != null)
            {
                if (Enum.TryParse(animationItem.Tag.ToString(), out PopupAnimationMode animationMode))
                {
                    _settings.Animation = animationMode;
                }
            }

            if (double.TryParse(CustomXBox.Text, out double x)) _settings.CustomX = x;
            if (double.TryParse(CustomYBox.Text, out double y)) _settings.CustomY = y;
            
            _settings.ShowDetailedClipboardInfo = ShowDetailedInfoCheck.IsChecked ?? true;
            _settings.AutoStart = AutoStartCheck.IsChecked ?? false;

            _settings.Save();
            
            // apply auto start to registry
            ApplyAutoStart(_settings.AutoStart);
            
            // apply live
            _mainWindow.ApplySettings();

            this.Close();
        }

        private void DragPositionBtn_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.EnterDragMode(this);
            this.Hide();
        }

        private void GitHubLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenGitHubProfile();
        }

        private void GitHubUsername_Click(object sender, RoutedEventArgs e)
        {
            OpenGitHubProfile();
        }

        private void OpenGitHubProfile()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/kalpit118",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to open GitHub profile: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnDragModeComplete(double x, double y)
        {
            SelectComboBoxItem(LocationTypeCombo, LocationType.Custom);
            CustomXBox.Text = ((int)x).ToString();
            CustomYBox.Text = ((int)y).ToString();

            this.Show();
            this.Activate();
        }

        private void ApplyAutoStart(bool enable)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key != null)
                {
                    string appName = "CopyAlert";
                    if (enable)
                    {
                        string? path = Environment.ProcessPath;
                        if (!string.IsNullOrEmpty(path))
                        {
                            key.SetValue(appName, $"\"{path}\"");
                        }
                    }
                    else
                    {
                        key.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to update startup settings: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
