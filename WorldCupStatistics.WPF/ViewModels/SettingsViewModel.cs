using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Settings;
using WorldCupStatistics.WPF.Infrastructure;
using WorldCupStatistics.WPF.Localization;

namespace WorldCupStatistics.WPF.ViewModels
{
    public sealed record GenderOption(string Text, Gender Value);
    public sealed record LanguageOption(string Text, string Code);
    public sealed record DisplayOption(string Text, DisplayMode Mode, int Width, int Height);

    public sealed class SettingsViewModel : ViewModelBase
    {
        private readonly AppSettings _current;

        public GenderOption[] Genders { get; }
        public LanguageOption[] Languages { get; }
        public DisplayOption[] DisplayOptions { get; }

        private GenderOption _selectedGender = null!;
        public GenderOption SelectedGender { get => _selectedGender; set => SetProperty(ref _selectedGender, value); }

        private LanguageOption _selectedLanguage = null!;
        public LanguageOption SelectedLanguage { get => _selectedLanguage; set => SetProperty(ref _selectedLanguage, value); }

        private DisplayOption _selectedDisplay = null!;
        public DisplayOption SelectedDisplay { get => _selectedDisplay; set => SetProperty(ref _selectedDisplay, value); }

        public SettingsViewModel(AppSettings current)
        {
            _current = current;

            Genders = new[]
            {
            new GenderOption(Loc.Instance["Men"], Gender.Men),
            new GenderOption(Loc.Instance["Women"], Gender.Women)
        };
            Languages = new[]
            {
            new LanguageOption(Loc.Instance["English"], "en"),
            new LanguageOption(Loc.Instance["Croatian"], "hr")
        };
            DisplayOptions = new[]
            {
            new DisplayOption(Loc.Instance["Fullscreen"], DisplayMode.Fullscreen, 0, 0),
            new DisplayOption("1280 × 720", DisplayMode.Windowed, 1280, 720),
            new DisplayOption("1600 × 900", DisplayMode.Windowed, 1600, 900),
            new DisplayOption("1920 × 1080", DisplayMode.Windowed, 1920, 1080)
        };

            _selectedGender = Genders.First(g => g.Value == current.Gender);
            _selectedLanguage = Languages.FirstOrDefault(l => l.Code == current.Language) ?? Languages[0];
            _selectedDisplay = current.DisplayMode == DisplayMode.Fullscreen
                ? DisplayOptions[0]
                : DisplayOptions.FirstOrDefault(d => d.Width == current.WindowWidth && d.Height == current.WindowHeight) ?? DisplayOptions[1];
        }

        public AppSettings BuildSettings() => new()
        {
            Gender = SelectedGender.Value,
            Language = SelectedLanguage.Code,
            DisplayMode = SelectedDisplay.Mode,
            WindowWidth = SelectedDisplay.Mode == DisplayMode.Windowed ? SelectedDisplay.Width : _current.WindowWidth,
            WindowHeight = SelectedDisplay.Mode == DisplayMode.Windowed ? SelectedDisplay.Height : _current.WindowHeight,
            FavoriteTeamFifaCode = _current.FavoriteTeamFifaCode,
            FavoritePlayerNumbers = [.. _current.FavoritePlayerNumbers]
        };
    }
}
