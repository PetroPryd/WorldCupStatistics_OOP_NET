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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorldCupStatistics.DataLayer.Models;

namespace WorldCupStatistics.WPF.Controls
{
    public partial class PlayerCard : UserControl
    {
        public Player Player { get; }

        public event Action<Player>? Selected;

        public PlayerCard(Player player, string imagePath)
        {
            InitializeComponent();
            Player = player;

            NumberText.Text = $"#{player.ShirtNumber}";
            NameText.Text = player.Name;
            PlayerImage.Source = ImageLoader.Load(imagePath);

            MouseLeftButtonUp += (_, _) => Selected?.Invoke(Player);

            if (player.IsCaptain)
            {
                CardBorder.BorderThickness = new System.Windows.Thickness(2);
                CardBorder.BorderBrush = new SolidColorBrush(Colors.Gold);

                var pulse = new DoubleAnimation
                {
                    From = 4,
                    To = 16,
                    Duration = TimeSpan.FromSeconds(0.9),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                var opacity = new DoubleAnimation
                {
                    From = 0.4,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.9),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                CardGlow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, pulse);
                CardGlow.BeginAnimation(DropShadowEffect.OpacityProperty, opacity);
            }
        }

        public void UseLargeLayout()
        {
            Width = 150;
            ImageHost.Width = 44;
            ImageHost.Height = 44;
            ImageHost.CornerRadius = new System.Windows.CornerRadius(22);
            NumberText.FontSize = 13;
            NameText.FontSize = 11;
            NameText.MaxWidth = 140;
        }
    }
}
