using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Services;
using WorldCupStatistics.DataLayer.Settings;
using WorldCupStatistics.WPF.Infrastructure;
using WorldCupStatistics.WPF.Localization;
using WorldCupStatistics.WPF.ViewModels;

namespace WorldCupStatistics.WPF.Views
{
    public partial class MainWindow : Window
    {
        private readonly IWorldCupService _worldCup;
        private readonly ISettingsService _settingsService;
        private readonly AppSettings _settings;
        private readonly MainViewModel _vm;

        public MainWindow(IWorldCupService worldCup, ISettingsService settingsService, AppSettings settings)
        {
            InitializeComponent();
            _worldCup = worldCup;
            _settingsService = settingsService;
            _settings = settings;

            _vm = new MainViewModel(worldCup, settings);
            _vm.ErrorOccurred += ex => MessageBox.Show(this, ex.Message, Loc.Instance["ErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            DataContext = _vm;

            var imageService = App.Services.GetRequiredService<IImageService>();

            _vm.MatchLoaded += match =>
            {
                LeftSubs.Items.Clear();
                RightSubs.Items.Clear();

                if (match is null) { Pitch.Clear(); LeftSubsTitle.Text = ""; RightSubsTitle.Text = ""; return; }

                var topCode = _vm.SelectedTeamCode;
                var bottomCode = _vm.SelectedOpponentCode;

                var top = MatchStatsFor(match, topCode);
                var bottom = MatchStatsFor(match, bottomCode);
                Pitch.SetLineups(top, bottom,
                    player => imageService.GetPlayerImagePath(_vm.Gender, CodeForPlayer(match, player), player.ShirtNumber));

                FillSubs(LeftSubs, SubsFor(match, topCode), match, topCode);
                FillSubs(RightSubs, SubsFor(match, bottomCode), match, bottomCode);
                LeftSubsTitle.Text = _vm.SelectedTeamName;
                RightSubsTitle.Text = _vm.SelectedOpponentName;
            };

            Pitch.PlayerSelected += player =>
            {
                // Phase 13: open the animated player-detail window here.
            };

            Loaded += async (_, _) => await _vm.LoadAsync();
            Closing += OnClosing;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsWindow(_settings) { Owner = this };
            if (dialog.ShowDialog() != true) return;

            var result = dialog.Result;
            var languageChanged = result.Language != _settings.Language;
            var genderChanged = result.Gender != _settings.Gender;

            _settings.Gender = result.Gender;
            _settings.Language = result.Language;
            _settings.DisplayMode = result.DisplayMode;
            _settings.WindowWidth = result.WindowWidth;
            _settings.WindowHeight = result.WindowHeight;
            if (genderChanged)
            {
                _settings.FavoriteTeamFifaCode = null;
                _settings.FavoritePlayerNumbers.Clear();
            }

            try { _settingsService.Save(_settings); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, Loc.Instance["ErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error); }

            if (languageChanged) Loc.Instance.SetLanguage(_settings.Language);
            DisplayModeHelper.Apply(this, _settings);
            if (genderChanged) _ = _vm.LoadAsync();
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            if (!ConfirmDialog.Ask(this, Loc.Instance["ConfirmExitTitle"], Loc.Instance["ConfirmExitMessage"]))
                e.Cancel = true;
        }

        private static IReadOnlyList<Player> MatchStatsFor(Match match, string? fifaCode)
        {
            if (string.Equals(match.HomeTeam.Code, fifaCode, StringComparison.OrdinalIgnoreCase))
                return match.HomeTeamStatistics.StartingEleven;
            if (string.Equals(match.AwayTeam.Code, fifaCode, StringComparison.OrdinalIgnoreCase))
                return match.AwayTeamStatistics.StartingEleven;
            return [];
        }

        private string CodeForPlayer(Match match, Player player)
        {
            if (match.HomeTeamStatistics.StartingEleven.Contains(player)) return match.HomeTeam.Code;
            if (match.AwayTeamStatistics.StartingEleven.Contains(player)) return match.AwayTeam.Code;
            return _vm.SelectedTeamCode ?? "";
        }

        private static IReadOnlyList<Player> SubsFor(Match match, string? fifaCode)
        {
            if (string.Equals(match.HomeTeam.Code, fifaCode, StringComparison.OrdinalIgnoreCase))
                return match.HomeTeamStatistics.Substitutes;
            if (string.Equals(match.AwayTeam.Code, fifaCode, StringComparison.OrdinalIgnoreCase))
                return match.AwayTeamStatistics.Substitutes;
            return [];
        }

        private void FillSubs(System.Windows.Controls.ItemsControl host, IReadOnlyList<Player> subs, Match match, string? code)
        {
            var imageService = App.Services.GetRequiredService<IImageService>();
            foreach (var p in subs.OrderBy(p => p.ShirtNumber))
            {
                var card = new Controls.PlayerCard(p, imageService.GetPlayerImagePath(_vm.Gender, code ?? "", p.ShirtNumber))
                {
                    Margin = new System.Windows.Thickness(2),
                    Width = 160
                };
                card.Selected += player => { /* Phase 13: player detail */ };
                host.Items.Add(card);
            }
        }
    }
}
