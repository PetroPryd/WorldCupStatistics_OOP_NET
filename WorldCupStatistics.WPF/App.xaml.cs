using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using WorldCupStatistics.DataLayer;
using WorldCupStatistics.DataLayer.Services;
using WorldCupStatistics.DataLayer.Settings;
using WorldCupStatistics.WPF.Infrastructure;
using WorldCupStatistics.WPF.Localization;
using WorldCupStatistics.WPF.Views;

namespace WorldCupStatistics.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddDataLayer(configuration);
            Services = services.BuildServiceProvider();

            var settingsService = Services.GetRequiredService<ISettingsService>();
            var worldCup = Services.GetRequiredService<IWorldCupService>();

            var settings = settingsService.Load();
            if (settings is null)
            {
                var setup = new SettingsWindow(new AppSettings());
                if (setup.ShowDialog() != true) { Shutdown(); return; }
                settings = setup.Result;
                settingsService.Save(settings);
            }

            Loc.Instance.SetLanguage(settings.Language);

            var main = new MainWindow(worldCup, settingsService, settings);
            DisplayModeHelper.Apply(main, settings);
            MainWindow = main;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            main.Show();
        }
    }
}
