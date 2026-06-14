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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.WPF.Controls;
using WorldCupStatistics.WPF.Infrastructure;
using WorldCupStatistics.WPF.Localization;

namespace WorldCupStatistics.WPF.Views
{
    public partial class PlayerDetailWindow : Window
    {
        public PlayerDetailWindow(Player player, Match match, string imagePath)
        {
            InitializeComponent();

            var (goals, yellows) = MatchStats.ForPlayer(match, player);

            PlayerImage.Source = ImageLoader.Load(imagePath);
            PlayerName.Text = player.Name;
            Number.Text = $"#{player.ShirtNumber}";
            Position.Text = player.Position.ToString();
            Captain.Text = player.IsCaptain ? Loc.Instance["Yes"] : Loc.Instance["No"];
            Goals.Text = goals.ToString();
            Yellows.Text = yellows.ToString();

            if (player.IsCaptain)
                ApplyCaptainShine();
        }

        private void ApplyCaptainShine()
        {
            CaptainBorder.BorderThickness = new Thickness(3);
            CaptainBorder.BorderBrush = new SolidColorBrush(Colors.Gold);

            var pulse = new DoubleAnimation
            {
                From = 8,
                To = 28,
                Duration = TimeSpan.FromSeconds(0.9),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            var opacity = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.9),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            CaptainGlow.BeginAnimation(System.Windows.Media.Effects.DropShadowEffect.BlurRadiusProperty, pulse);
            CaptainGlow.BeginAnimation(System.Windows.Media.Effects.DropShadowEffect.OpacityProperty, opacity);
        }
    }
}
