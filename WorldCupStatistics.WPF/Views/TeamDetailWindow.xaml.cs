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

namespace WorldCupStatistics.WPF.Views
{
    public partial class TeamDetailWindow : Window
    {
        public TeamDetailWindow(Team team)
        {
            InitializeComponent();

            TeamName.Text = team.Country;
            FifaCode.Text = team.FifaCode;
            Games.Text = team.GamesPlayed.ToString();
            Wins.Text = team.Wins.ToString();
            Draws.Text = team.Draws.ToString();
            Losses.Text = team.Losses.ToString();
            GoalsFor.Text = team.GoalsFor.ToString();
            GoalsAgainst.Text = team.GoalsAgainst.ToString();
            GoalDiff.Text = team.GoalDifferential.ToString();
        }
    }
}
