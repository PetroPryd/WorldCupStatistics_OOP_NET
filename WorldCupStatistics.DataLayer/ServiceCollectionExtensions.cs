using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Configuration;
using WorldCupStatistics.DataLayer.Repositories;
using WorldCupStatistics.DataLayer.Services;

namespace WorldCupStatistics.DataLayer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataSourceConfig>(configuration.GetSection(DataSourceConfig.SectionName));

            services.AddHttpClient<ApiWorldCupRepository>((sp, client) =>
            {
                var cfg = sp.GetRequiredService<IOptions<DataSourceConfig>>().Value;
                client.BaseAddress = new Uri(cfg.ApiBaseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddSingleton<JsonWorldCupRepository>();

            services.AddSingleton<IWorldCupRepository>(sp =>
            {
                var mode = sp.GetRequiredService<IOptions<DataSourceConfig>>().Value.Mode;
                return mode == DataSourceMode.Api
                    ? sp.GetRequiredService<ApiWorldCupRepository>()
                    : sp.GetRequiredService<JsonWorldCupRepository>();
            });

            services.AddSingleton<ISettingsService, FileSettingsService>();
            services.AddSingleton<IImageService, ImageService>();

            services.AddSingleton<IWorldCupService, WorldCupService>();

            return services;
        }
    }
}
