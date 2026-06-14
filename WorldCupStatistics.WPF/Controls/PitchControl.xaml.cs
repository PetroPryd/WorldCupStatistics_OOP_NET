using System;
using System.Collections.Generic;
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

namespace WorldCupStatistics.WPF.Controls
{
    public partial class PitchControl : UserControl
    {
        private static readonly Position[] RowOrder =
            { Position.Goalie, Position.Defender, Position.Midfield, Position.Forward };

        private IReadOnlyList<Player> _topPlayers = [];
        private IReadOnlyList<Player> _bottomPlayers = [];
        private Func<Player, string>? _imagePathResolver;

        public event Action<Player>? PlayerSelected;

        public PitchControl()
        {
            InitializeComponent();
        }

        public void SetLineups(
            IReadOnlyList<Player> topTeam,
            IReadOnlyList<Player> bottomTeam,
            Func<Player, string> imagePathResolver)
        {
            _topPlayers = topTeam;
            _bottomPlayers = bottomTeam;
            _imagePathResolver = imagePathResolver;
            Redraw();
        }

        public void Clear()
        {
            _topPlayers = [];
            _bottomPlayers = [];
            PlayersCanvas.Children.Clear();
            MarkingsCanvas.Children.Clear();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) => Redraw();

        private void Redraw()
        {
            double w = ActualWidth, h = ActualHeight;
            if (w <= 0 || h <= 0 || _imagePathResolver is null) return;

            DrawMarkings(w, h);

            PlayersCanvas.Children.Clear();
            PlaceHalf(_topPlayers, w, topHalf: true, h);
            PlaceHalf(_bottomPlayers, w, topHalf: false, h);
        }

        private void DrawMarkings(double w, double h)
        {
            MarkingsCanvas.Children.Clear();
            var pen = new SolidColorBrush(Color.FromArgb(160, 255, 255, 255));

            var halfway = new Line { X1 = 0, Y1 = h / 2, X2 = w, Y2 = h / 2, Stroke = pen, StrokeThickness = 2 };
            MarkingsCanvas.Children.Add(halfway);

            double r = Math.Min(w, h) * 0.12;
            var circle = new Ellipse { Width = r * 2, Height = r * 2, Stroke = pen, StrokeThickness = 2 };
            Canvas.SetLeft(circle, w / 2 - r);
            Canvas.SetTop(circle, h / 2 - r);
            MarkingsCanvas.Children.Add(circle);
        }

        private void PlaceHalf(IReadOnlyList<Player> players, double w, bool topHalf, double h)
        {
            var rows = RowOrder
                .Select(pos => players.Where(p => p.Position == pos).ToList())
                .ToList();

            var unknown = players.Where(p => !RowOrder.Contains(p.Position)).ToList();
            if (unknown.Count > 0) rows[2].AddRange(unknown);

            const int maxPerLine = 4;
            var lines = new List<List<Player>>();
            foreach (var row in rows)
            {
                if (row.Count == 0) continue;
                for (int i = 0; i < row.Count; i += maxPerLine)
                    lines.Add(row.Skip(i).Take(maxPerLine).ToList());
            }
            if (lines.Count == 0) return;

            double halfHeight = h / 2;
            double cardH = 48;

            double edgeMargin = cardH * 0.9;
            double centreMargin = cardH * 0.7;
            double bandStart = edgeMargin;
            double bandEnd = halfHeight - centreMargin;
            if (bandEnd <= bandStart) { bandStart = cardH / 2; bandEnd = halfHeight - cardH / 2; }

            for (int line = 0; line < lines.Count; line++)
            {
                double t = lines.Count == 1 ? 0.0 : (double)line / (lines.Count - 1);
                double offset = bandStart + t * (bandEnd - bandStart);
                double y = topHalf ? offset : h - offset;

                var linePlayers = lines[line];
                for (int i = 0; i < linePlayers.Count; i++)
                {
                    double x = (i + 1.0) / (linePlayers.Count + 1) * w;
                    var card = new PlayerCard(linePlayers[i], _imagePathResolver!(linePlayers[i]));
                    card.Selected += p => PlayerSelected?.Invoke(p);

                    card.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double cardW = card.DesiredSize.Width;
                    double ch = card.DesiredSize.Height;
                    Canvas.SetLeft(card, x - cardW / 2);
                    Canvas.SetTop(card, y - ch / 2);
                    PlayersCanvas.Children.Add(card);
                }
            }
        }
    }
}