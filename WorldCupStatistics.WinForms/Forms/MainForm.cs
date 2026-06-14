using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Services;
using WorldCupStatistics.DataLayer.Settings;
using WorldCupStatistics.WinForms.Controls;
using WorldCupStatistics.WinForms.Localization;

namespace WorldCupStatistics.WinForms.Forms
{
    internal sealed class MainForm : Form
    {
        private const string DragFormat = "WCSPlayers";
        private const int MaxFavorites = 3;

        private readonly IWorldCupService _worldCup;
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;
        private readonly AppSettings _settings;

        private readonly Label _teamLabel = new() { AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 6, 0, 0) };
        private readonly ComboBox _teamCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 320 };
        private readonly Button _settingsButton = new() { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(10, 4, 10, 4) };
        private readonly Button _rankingsButton = new() { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(10, 4, 10, 4) };

        private readonly GroupBox _favoritesGroup = new() { Dock = DockStyle.Fill };
        private readonly GroupBox _othersGroup = new() { Dock = DockStyle.Fill };
        private readonly FlowLayoutPanel _favoritesPanel = new() { Dock = DockStyle.Fill, AutoScroll = true, AllowDrop = true };
        private readonly FlowLayoutPanel _othersPanel = new() { Dock = DockStyle.Fill, AutoScroll = true, AllowDrop = true };

        private readonly ContextMenuStrip _playerMenu = new();
        private readonly ToolStripMenuItem _moveToFavItem = new();
        private readonly ToolStripMenuItem _moveToOthersItem = new();
        private readonly ToolStripMenuItem _setImageItem = new();

        private readonly LoadingOverlay _loading = new();

        private readonly List<PlayerControl> _selection = new();
        private PlayerControl? _anchor;
        private Point _dragStartScreen;
        private bool _dragArmed;

        private bool _populating;
        private Team? _currentTeam;

        public MainForm(IWorldCupService worldCup, ISettingsService settingsService, IImageService imageService, AppSettings settings)
        {
            _worldCup = worldCup;
            _settingsService = settingsService;
            _imageService = imageService;
            _settings = settings;

            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 620);

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(10, 12, 10, 0), WrapContents = false };
            _teamCombo.DisplayMember = nameof(Team.DisplayName);
            _teamCombo.ValueMember = nameof(Team.FifaCode);
            _teamCombo.SelectedIndexChanged += OnTeamChanged;
            _settingsButton.Click += OnSettingsClicked;
            _rankingsButton.Click += OnRankingsClicked;

            _teamLabel.AutoSize = true;
            _teamLabel.Anchor = AnchorStyles.Left;
            _teamLabel.Margin = new Padding(3, 10, 3, 3);

            top.Controls.Add(_teamLabel);
            top.Controls.Add(_teamCombo);
            top.Controls.Add(_settingsButton);
            top.Controls.Add(_rankingsButton);

            _favoritesGroup.Controls.Add(_favoritesPanel);
            _othersGroup.Controls.Add(_othersPanel);

            foreach (var panel in new[] { _favoritesPanel, _othersPanel })
            {
                panel.DragEnter += OnPanelDragEnter;
                panel.DragDrop += OnPanelDragDrop;
            }

            _moveToFavItem.Click += (_, _) => MoveSelectionTo(_favoritesPanel);
            _moveToOthersItem.Click += (_, _) => MoveSelectionTo(_othersPanel);
            _setImageItem.Click += (_, _) =>
            {
                var card = ResolvePlayer(_playerMenu.SourceControl) ?? _selection.FirstOrDefault();
                if (card is not null) _ = SetPlayerImageAsync(card);
            };
            _playerMenu.Items.Add(_moveToFavItem);
            _playerMenu.Items.Add(_moveToOthersItem);
            _playerMenu.Items.Add(new ToolStripSeparator());
            _playerMenu.Items.Add(_setImageItem);
            _playerMenu.Opening += OnPlayerMenuOpening;

            var split = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(10) };
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            split.Controls.Add(_favoritesGroup, 0, 0);
            split.Controls.Add(_othersGroup, 1, 0);

            Controls.Add(split);
            Controls.Add(top);
            Controls.Add(_loading);

            FormClosing += OnFormClosing;
            Load += async (_, _) => await LoadTeamsAsync();

            ApplyLocalization();
        }

        private void OnRankingsClicked(object? sender, EventArgs e)
        {
            if (_currentTeam is null) return;
            using var form = new RankingsForm(_worldCup, _imageService, _settings.Gender, _currentTeam);
            form.ShowDialog(this);
        }

        private void ApplyLocalization()
        {
            Text = Loc.T("AppTitle");
            _teamLabel.Text = Loc.T("FavoriteTeamLabel");
            _settingsButton.Text = Loc.T("SettingsButton");
            _favoritesGroup.Text = Loc.T("FavoritePlayersTitle");
            _othersGroup.Text = Loc.T("OtherPlayersTitle");
            _moveToFavItem.Text = Loc.T("MoveToFavorites");
            _moveToOthersItem.Text = Loc.T("MoveToOthers");
            _setImageItem.Text = Loc.T("SetImage");
            _rankingsButton.Text = Loc.T("RankingsButton");
        }

        private async Task LoadTeamsAsync()
        {
            _loading.Begin();
            try
            {
                var teams = (await _worldCup.GetTeamsAsync(_settings.Gender)).ToList();
                _populating = true;
                _teamCombo.DataSource = teams;
                var index = teams.FindIndex(t => string.Equals(t.FifaCode, _settings.FavoriteTeamFifaCode, StringComparison.OrdinalIgnoreCase));
                _teamCombo.SelectedIndex = index >= 0 ? index : 0;
                _populating = false;
                OnTeamChanged(this, EventArgs.Empty);
            }
            catch (Exception ex) { ShowError(ex); }
            finally { _loading.End(); }
        }

        private async void OnTeamChanged(object? sender, EventArgs e)
        {
            if (_populating || _teamCombo.SelectedItem is not Team team) return;
            _currentTeam = team;
            await LoadPlayersAsync(team);
        }

        private async Task LoadPlayersAsync(Team team)
        {
            _loading.Begin();
            try
            {
                var players = await _worldCup.GetFirstMatchPlayersAsync(_settings.Gender, team.FifaCode);
                RenderPlayers(team, players);
            }
            catch (Exception ex) { ShowError(ex); }
            finally { _loading.End(); }
        }

        private void RenderPlayers(Team team, IReadOnlyList<Player> players)
        {
            _selection.Clear();
            _anchor = null;
            ClearPanel(_favoritesPanel);
            ClearPanel(_othersPanel);

            var favouritesApply = string.Equals(team.FifaCode, _settings.FavoriteTeamFifaCode, StringComparison.OrdinalIgnoreCase);

            foreach (var player in players)
            {
                var isFavorite = favouritesApply && _settings.FavoritePlayerNumbers.Contains(player.ShirtNumber);
                var imagePath = _imageService.GetPlayerImagePath(_settings.Gender, team.FifaCode, player.ShirtNumber);
                var control = new PlayerControl(player, imagePath, isFavorite);

                control.Click += (_, _) => HandleCardClick(control);
                control.CardMouseDown += (_, e) => OnCardMouseDown(control, e);
                control.CardMouseMove += (_, e) => OnCardMouseMove(e);
                control.SetImageRequested += c => _ = SetPlayerImageAsync(c);
                control.SetContextMenu(_playerMenu);

                (isFavorite ? _favoritesPanel : _othersPanel).Controls.Add(control);
            }
        }

        private static void ClearPanel(Control panel)
        {
            var existing = panel.Controls.OfType<PlayerControl>().ToList();
            panel.Controls.Clear();
            foreach (var control in existing) control.Dispose();
        }

        private async Task SetPlayerImageAsync(PlayerControl card)
        {
            if (_currentTeam is null) return;

            using var dialog = new OpenFileDialog
            {
                Title = Loc.T("SetImage"),
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*"
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                await _imageService.SetPlayerImageAsync(
                    _settings.Gender, _currentTeam.FifaCode, card.ShirtNumber, dialog.FileName);

                var newPath = _imageService.GetPlayerImagePath(_settings.Gender, _currentTeam.FifaCode, card.ShirtNumber);
                card.ReloadImage(newPath);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void HandleCardClick(PlayerControl control)
        {
            var mods = ModifierKeys;
            if ((mods & Keys.Control) == Keys.Control)
            {
                Toggle(control);
                _anchor = control;
            }
            else if ((mods & Keys.Shift) == Keys.Shift && _anchor is not null && _anchor.Parent == control.Parent)
            {
                RangeSelect(_anchor, control);
            }
            else
            {
                ClearSelection();
                AddToSelection(control);
                _anchor = control;
            }
        }

        private void AddToSelection(PlayerControl c) { if (!_selection.Contains(c)) { _selection.Add(c); c.Selected = true; } }
        private void RemoveFromSelection(PlayerControl c) { if (_selection.Remove(c)) c.Selected = false; }
        private void Toggle(PlayerControl c) { if (_selection.Contains(c)) RemoveFromSelection(c); else AddToSelection(c); }

        private void ClearSelection()
        {
            foreach (var c in _selection) c.Selected = false;
            _selection.Clear();
        }

        private void RangeSelect(PlayerControl anchor, PlayerControl target)
        {
            if (anchor.Parent is not Control panel) return;
            var cards = panel.Controls.OfType<PlayerControl>().ToList();
            int a = cards.IndexOf(anchor), b = cards.IndexOf(target);
            if (a < 0 || b < 0) return;
            ClearSelection();
            for (int i = Math.Min(a, b); i <= Math.Max(a, b); i++) AddToSelection(cards[i]);
        }

        private void OnCardMouseDown(PlayerControl control, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!_selection.Contains(control) && (ModifierKeys & (Keys.Control | Keys.Shift)) == 0)
                {
                    ClearSelection();
                    AddToSelection(control);
                    _anchor = control;
                }
                _dragStartScreen = Cursor.Position;
                _dragArmed = true;
            }
            else if (e.Button == MouseButtons.Right && !_selection.Contains(control))
            {
                ClearSelection();
                AddToSelection(control);
                _anchor = control;
            }
        }

        private void OnCardMouseMove(MouseEventArgs e)
        {
            if (!_dragArmed || (MouseButtons & MouseButtons.Left) == 0 || _selection.Count == 0) return;
            var now = Cursor.Position;
            if (Math.Abs(now.X - _dragStartScreen.X) >= SystemInformation.DragSize.Width ||
                Math.Abs(now.Y - _dragStartScreen.Y) >= SystemInformation.DragSize.Height)
            {
                _dragArmed = false;
                DoDragDrop(new DataObject(DragFormat, true), DragDropEffects.Move);
            }
        }

        private void OnPanelDragEnter(object? sender, DragEventArgs e)
            => e.Effect = e.Data?.GetDataPresent(DragFormat) == true ? DragDropEffects.Move : DragDropEffects.None;

        private void OnPanelDragDrop(object? sender, DragEventArgs e)
        {
            if (sender is FlowLayoutPanel target) MoveSelectionTo(target);
        }

        private void OnPlayerMenuOpening(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var source = ResolvePlayer(_playerMenu.SourceControl);
            if (source is not null && !_selection.Contains(source))
            {
                ClearSelection();
                AddToSelection(source);
                _anchor = source;
            }
            if (_selection.Count == 0) { e.Cancel = true; return; }

            _moveToFavItem.Enabled = _selection.Any(c => c.Parent == _othersPanel);
            _moveToOthersItem.Enabled = _selection.Any(c => c.Parent == _favoritesPanel);
            _setImageItem.Enabled = _selection.Count == 1;
        }

        private static PlayerControl? ResolvePlayer(Control? c)
        {
            while (c is not null and not PlayerControl) c = c.Parent;
            return c as PlayerControl;
        }

        private void MoveSelectionTo(FlowLayoutPanel target)
        {
            var moving = _selection.Where(c => c.Parent != target).ToList();
            if (moving.Count == 0) return;

            if (target == _favoritesPanel)
            {
                var resulting = _favoritesPanel.Controls.OfType<PlayerControl>().Count() + moving.Count;
                if (resulting > MaxFavorites)
                {
                    MessageBox.Show(this, Loc.T("FavoritesLimitMessage"), Loc.T("AppTitle"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            foreach (var c in moving)
            {
                c.Parent?.Controls.Remove(c);
                target.Controls.Add(c);
            }

            UpdateFavorites();
            ClearSelection();
        }

        private void UpdateFavorites()
        {
            var favouriteNumbers = _favoritesPanel.Controls.OfType<PlayerControl>()
                .Select(c => c.ShirtNumber).ToList();

            _settings.FavoritePlayerNumbers = favouriteNumbers;

            _settings.FavoriteTeamFifaCode = favouriteNumbers.Count > 0 ? _currentTeam?.FifaCode : null;
            TrySaveSettings();

            foreach (var c in _favoritesPanel.Controls.OfType<PlayerControl>()) c.IsFavorite = true;
            foreach (var c in _othersPanel.Controls.OfType<PlayerControl>()) c.IsFavorite = false;
        }

        private async void OnSettingsClicked(object? sender, EventArgs e)
        {
            using var dialog = new SettingsForm(_settings);
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            var genderChanged = dialog.Result.Gender != _settings.Gender;
            var languageChanged = dialog.Result.Language != _settings.Language;

            _settings.Gender = dialog.Result.Gender;
            _settings.Language = dialog.Result.Language;
            if (genderChanged)
            {
                _settings.FavoriteTeamFifaCode = null;
                _settings.FavoritePlayerNumbers.Clear();
            }
            TrySaveSettings();

            if (languageChanged)
            {
                Loc.ApplyCulture(_settings.Language);
                ApplyLocalization();
            }
            if (genderChanged)
                await LoadTeamsAsync();
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            Loc.ApplyCulture(_settings.Language);
            if (!ConfirmDialog.Ask(this, Loc.T("ConfirmExitTitle"), Loc.T("ConfirmExitMessage")))
                e.Cancel = true;
        }

        private void TrySaveSettings()
        {
            try { _settingsService.Save(_settings); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ShowError(Exception ex) =>
            MessageBox.Show(this, ex.Message, Loc.T("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}