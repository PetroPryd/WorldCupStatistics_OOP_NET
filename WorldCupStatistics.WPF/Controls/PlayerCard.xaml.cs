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
        }
    }
}
