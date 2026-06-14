using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.WPF.Localization
{
    public sealed class Loc : INotifyPropertyChanged
    {
        public static Loc Instance { get; } = new();
        public static string Localization_Yes() => Instance["Yes"];
        public static string Localization_No() => Instance["No"];

        private readonly ResourceManager _rm = new("WorldCupStatistics.WPF.Localization.Strings", typeof(Loc).Assembly);
        private CultureInfo _culture = CultureInfo.CurrentUICulture;

        public string this[string key] => _rm.GetString(key, _culture) ?? key;

        public void SetLanguage(string language)
        {
            _culture = string.Equals(language, "hr", StringComparison.OrdinalIgnoreCase)
                ? new CultureInfo("hr-HR")
                : new CultureInfo("en-US");
            CultureInfo.CurrentCulture = _culture;
            CultureInfo.CurrentUICulture = _culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private Loc() { }
    }
}
