using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Services;
using WorldCupStatistics.DataLayer.Settings;
using WorldCupStatistics.WPF.Infrastructure;

namespace WorldCupStatistics.WPF.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        private readonly IWorldCupService _worldCup;
        private readonly AppSettings _settings;

        public Gender Gender => _settings.Gender;
        public string? SelectedTeamCode => _selectedTeam?.FifaCode;
        public string? SelectedOpponentCode => _selectedOpponent?.FifaCode;

        public string SelectedTeamName => _selectedTeam?.DisplayName ?? "";
        public string SelectedOpponentName => _selectedOpponent?.DisplayName ?? "";

        public ObservableCollection<Team> Teams { get; } = new();
        public ObservableCollection<Team> Opponents { get; } = new();

        public event Action<Match?>? MatchLoaded;

        private Team? _selectedTeam;
        public Team? SelectedTeam
        {
            get => _selectedTeam;
            set { if (SetProperty(ref _selectedTeam, value)) OnTeamChanged(); }
        }

        private Team? _selectedOpponent;
        public Team? SelectedOpponent
        {
            get => _selectedOpponent;
            set { if (SetProperty(ref _selectedOpponent, value)) _ = UpdateResultAsync(); }
        }

        private string _resultText = "";
        public string ResultText { get => _resultText; set => SetProperty(ref _resultText, value); }

        private bool _isLoading;
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        public event Action<Exception>? ErrorOccurred;

        public MainViewModel(IWorldCupService worldCup, AppSettings settings)
        {
            _worldCup = worldCup;
            _settings = settings;
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                Teams.Clear();
                foreach (var t in await _worldCup.GetTeamsAsync(_settings.Gender)) Teams.Add(t);

                SelectedTeam = Teams.FirstOrDefault(t =>
                    string.Equals(t.FifaCode, _settings.FavoriteTeamFifaCode, StringComparison.OrdinalIgnoreCase))
                    ?? Teams.FirstOrDefault();
            }
            catch (Exception ex) { ErrorOccurred?.Invoke(ex); }
            finally { IsLoading = false; }
        }

        private async void OnTeamChanged()
        {
            ResultText = "";
            SelectedOpponent = null;
            Opponents.Clear();
            if (_selectedTeam is null) return;

            IsLoading = true;
            try
            {
                foreach (var o in await _worldCup.GetOpponentsAsync(_settings.Gender, _selectedTeam.FifaCode))
                    Opponents.Add(o);
            }
            catch (Exception ex) { ErrorOccurred?.Invoke(ex); }
            finally { IsLoading = false; }
        }

        private async Task UpdateResultAsync()
        {
            if (_selectedTeam is null || _selectedOpponent is null)
            {
                ResultText = "";
                MatchLoaded?.Invoke(null);
                return;
            }

            IsLoading = true;
            try
            {
                var match = await _worldCup.GetMatchBetweenAsync(_settings.Gender, _selectedTeam.FifaCode, _selectedOpponent.FifaCode);
                ResultText = BuildResultText(match);
                MatchLoaded?.Invoke(match);
            }
            catch (Exception ex) { ErrorOccurred?.Invoke(ex); }
            finally { IsLoading = false; }
        }

        private string BuildResultText(Match? match)
        {
            if (match is null) return "–";

            var selectedIsHome = string.Equals(match.HomeTeam.Code, _selectedTeam!.FifaCode, StringComparison.OrdinalIgnoreCase);
            var selectedGoals = selectedIsHome ? match.HomeTeam.Goals : match.AwayTeam.Goals;
            var opponentGoals = selectedIsHome ? match.AwayTeam.Goals : match.HomeTeam.Goals;

            // 🏠 home side
            var selectedSide = selectedIsHome ? $"🏠 {selectedGoals}" : $"{selectedGoals}";
            var opponentSide = selectedIsHome ? $"{opponentGoals}" : $"{opponentGoals} 🏠";
            return $"{selectedSide} : {opponentSide}";
        }
    }
}
