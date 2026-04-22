namespace RaspberryDashboard.Repository {
    using System.Globalization;
    public class LanguageOption {
        public string Code { get; set; } = "";
        public string Label { get; set; } = "";
        public string Flag { get; set; } = "";
    }
    public static class SupportedLanguages {
        public static readonly List<LanguageOption> All = new()
        {
            new() { Code = "en-GB", Label = "English", Flag = "flags/gb.png" },
            new() { Code = "de-DE", Label = "Deutsch", Flag = "flags/de.png" },
            new() { Code = "ru-RU", Label = "Русский", Flag = "flags/ru.png" }
        };
    }
    public class LanguageService {
        public event Action? OnChange;
        public string CurrentLanguage => CultureInfo.CurrentUICulture.Name;
        public IReadOnlyList<LanguageOption> AvailableLanguages => SupportedLanguages.All;
        public void SetLanguage(string lang) {
            var culture = new CultureInfo(lang);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            OnChange?.Invoke();
        }

        //public void Toggle() {
        //    var index = Array.IndexOf(SupportedLanguages.All.Select(s=>s.Code), CurrentLanguage);
        //    var next = SupportedLanguages.All[(index + 1 + SupportedLanguages.All.Length) % SupportedLanguages.All.Length];
        //    SetLanguage(next);
        //}
    }
}
