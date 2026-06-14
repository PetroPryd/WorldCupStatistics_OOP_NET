using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorldCupStatistics.DataLayer;
using WorldCupStatistics.DataLayer.Services;
using WorldCupStatistics.DataLayer.Settings;
using WorldCupStatistics.WinForms.Forms;
using WorldCupStatistics.WinForms.Localization;

namespace WorldCupStatistics.WinForms
{
    internal static class Program
    {
        public static IServiceProvider Services { get; private set; } = default!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

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
            var imageService = Services.GetRequiredService<IImageService>();

            var settings = settingsService.Load();
            if (settings is null)
            {
                using var setup = new SettingsForm(new AppSettings());
                if (setup.ShowDialog() != DialogResult.OK)
                    return;
                settings = setup.Result;
                settingsService.Save(settings);
            }

            Loc.ApplyCulture(settings.Language);
            Application.Run(new MainForm(worldCup, settingsService, imageService, settings));
        }
    }
}