using System;
using System.IO;
using System.Text.Json;

namespace ClipAlert
{
    public enum LocationType
    {
        BottomRight,
        BottomLeft,
        TopRight,
        TopLeft,
        Custom
    }

    public enum NotificationStyle
    {
        Compact,
        FullCard
    }

    public enum PopupAnimationMode
    {
        Fade,
        Slide,
        Smart
    }

    public class AppSettings
    {
        public LocationType Location { get; set; } = LocationType.BottomRight;
        public double CustomX { get; set; } = 0;
        public double CustomY { get; set; } = 0;
        public bool AutoStart { get; set; } = false;
        public bool ShowDetailedClipboardInfo { get; set; } = true;
        public NotificationStyle Style { get; set; } = NotificationStyle.Compact;
        public PopupAnimationMode Animation { get; set; } = PopupAnimationMode.Smart;

        private static string SettingsFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static AppSettings Load()
        {
            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                catch
                {
                    return new AppSettings();
                }
            }
            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, json);
            }
            catch { }
        }
    }
}
