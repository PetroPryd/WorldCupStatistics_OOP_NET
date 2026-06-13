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
using WorldCupStatistics.DataLayer.Settings;
using WorldCupStatistics.WPF.Localization;
using WorldCupStatistics.WPF.ViewModels;

namespace WorldCupStatistics.WPF.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _vm;

        public AppSettings Result => _vm.BuildSettings();

        public SettingsWindow(AppSettings current)
        {
            InitializeComponent();
            _vm = new SettingsViewModel(current);
            DataContext = _vm;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmDialog.Ask(this, Loc.Instance["SettingsTitle"], Loc.Instance["ConfirmSettingsMessage"]))
                return;
            DialogResult = true;
        }
    }
}
