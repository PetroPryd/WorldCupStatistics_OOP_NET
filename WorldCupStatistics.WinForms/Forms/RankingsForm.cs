using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Results;
using WorldCupStatistics.DataLayer.Services;
using WorldCupStatistics.WinForms.Controls;
using WorldCupStatistics.WinForms.Localization;
using WorldCupStatistics.WinForms.Printing;

namespace WorldCupStatistics.WinForms.Forms
{
    internal sealed class RankingsForm : Form
    {
        private readonly IWorldCupService _worldCup;
        private readonly IImageService _imageService;
        private readonly Gender _gender;
        private readonly Team _team;

        private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
        private readonly TabPage _goalsTab = new();
        private readonly TabPage _yellowTab = new();
        private readonly TabPage _attendanceTab = new();
        private readonly DataGridView _goalsGrid = CreateGrid();
        private readonly DataGridView _yellowGrid = CreateGrid();
        private readonly DataGridView _attendanceGrid = CreateGrid();
        private readonly Button _printButton = new() { Dock = DockStyle.Fill, Height = 36 };
        private readonly LoadingOverlay _loading = new();

        private IReadOnlyList<PlayerRanking> _byGoals = [];
        private IReadOnlyList<PlayerRanking> _byYellow = [];
        private IReadOnlyList<MatchAttendance> _attendance = [];
        private readonly List<Image> _loadedImages = new();

        public RankingsForm(IWorldCupService worldCup, IImageService imageService, Gender gender, Team team)
        {
            _worldCup = worldCup;
            _imageService = imageService;
            _gender = gender;
            _team = team;

            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(820, 560);

            _goalsTab.Controls.Add(_goalsGrid);
            _yellowTab.Controls.Add(_yellowGrid);
            _attendanceTab.Controls.Add(_attendanceGrid);
            _tabs.TabPages.AddRange(new[] { _goalsTab, _yellowTab, _attendanceTab });

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(10, 6, 10, 6) };
            _printButton.Click += OnPrintClicked;
            bottom.Controls.Add(_printButton);

            Controls.Add(_tabs);
            Controls.Add(bottom);
            Controls.Add(_loading);

            FormClosed += (_, _) => DisposeImages();
            Load += async (_, _) => await LoadAsync();

            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            Text = $"{Loc.T("RankingsButton")} — {_team.DisplayName}";
            _goalsTab.Text = Loc.T("TabPlayersByGoals");
            _yellowTab.Text = Loc.T("TabPlayersByYellow");
            _attendanceTab.Text = Loc.T("TabMatchesByAttendance");
            _printButton.Text = Loc.T("PrintButton");
        }

        private static DataGridView CreateGrid() => new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        private async Task LoadAsync()
        {
            _loading.Begin();
            try
            {
                var players = await _worldCup.GetPlayerRankingsAsync(_gender, _team.FifaCode);
                _byGoals = players.OrderByDescending(p => p.Goals).ThenBy(p => p.Name).ToList();
                _byYellow = players.OrderByDescending(p => p.YellowCards).ThenBy(p => p.Name).ToList();
                _attendance = await _worldCup.GetMatchAttendanceRankingAsync(_gender, _team.FifaCode);

                BuildPlayerGrid(_goalsGrid, _byGoals);
                BuildPlayerGrid(_yellowGrid, _byYellow);
                BuildAttendanceGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Loc.T("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loading.End();
            }
        }

        private void BuildPlayerGrid(DataGridView grid, IReadOnlyList<PlayerRanking> data)
        {
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewImageColumn
            {
                HeaderText = Loc.T("ColPicture"),
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                FillWeight = 12
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColName"), FillWeight = 32 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColShirt"), FillWeight = 9 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColPosition"), FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColAppearances"), FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColGoals"), FillWeight = 8 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColYellowCards"), FillWeight = 9 });
            grid.RowTemplate.Height = 54;

            foreach (var p in data)
            {
                var path = _imageService.GetPlayerImagePath(_gender, _team.FifaCode, p.ShirtNumber);
                var img = ImageLoader.Load(path);
                if (img is not null) _loadedImages.Add(img);
                grid.Rows.Add(img, p.Name, p.ShirtNumber, p.Position.ToString(), p.Appearances, p.Goals, p.YellowCards);
            }
        }

        private void BuildAttendanceGrid()
        {
            _attendanceGrid.Columns.Clear();
            _attendanceGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColLocation"), FillWeight = 30 });
            _attendanceGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColAttendance"), FillWeight = 14 });
            _attendanceGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColHome"), FillWeight = 17 });
            _attendanceGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColAway"), FillWeight = 17 });
            _attendanceGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = Loc.T("ColDate"), FillWeight = 22 });

            foreach (var m in _attendance)
                _attendanceGrid.Rows.Add(m.Location, m.Attendance.ToString("N0"), m.HomeTeam, m.AwayTeam, m.DateTimeUtc.ToString("yyyy-MM-dd HH:mm"));
        }

        private void OnPrintClicked(object? sender, EventArgs e)
        {
            if (_tabs.SelectedTab == _attendanceTab)
            {
                var rows = _attendance.Select(m => new[]
                {
                m.Location, m.Attendance.ToString("N0"), m.HomeTeam, m.AwayTeam, m.DateTimeUtc.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

                PrintTable(Loc.T("TabMatchesByAttendance"),
                    new[] { Loc.T("ColLocation"), Loc.T("ColAttendance"), Loc.T("ColHome"), Loc.T("ColAway"), Loc.T("ColDate") },
                    rows, new float[] { 26, 13, 17, 17, 27 });   // 5 columns
            }
            else
            {
                var data = _tabs.SelectedTab == _yellowTab ? _byYellow : _byGoals;
                var title = _tabs.SelectedTab == _yellowTab ? Loc.T("TabPlayersByYellow") : Loc.T("TabPlayersByGoals");

                var rows = data.Select(p => new[]
                {
                p.Name, p.ShirtNumber.ToString(), p.Position.ToString(),
                p.Appearances.ToString(), p.Goals.ToString(), p.YellowCards.ToString()
            }).ToList();

                PrintTable(title,
                    new[] { Loc.T("ColName"), Loc.T("ColShirt"), Loc.T("ColPosition"), Loc.T("ColAppearances"), Loc.T("ColGoals"), Loc.T("ColYellowCards") },
                    rows, new float[] { 32, 9, 20, 14, 12, 13 });   // 6 columns
            }
        }

        private void PrintTable(string title, string[] headers, IReadOnlyList<string[]> rows, float[] weights)
        {
            using var printer = new GridPrinter($"{title} — {_team.DisplayName}", headers, rows, weights);
            printer.ShowPreview(this);
        }

        private void DisposeImages()
        {
            foreach (var img in _loadedImages) img.Dispose();
            _loadedImages.Clear();
        }
    }
}
