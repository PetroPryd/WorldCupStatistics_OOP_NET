using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Configuration
{
    public enum DataSourceMode
    {
        Api,
        Json
    }

    /// <summary>Strongly-typed binding of the "DataSource" section in appsettings.json.</summary>
    public sealed class DataSourceConfig
    {
        public const string SectionName = "DataSource";

        public DataSourceMode Mode { get; init; } = DataSourceMode.Json;
        public string ApiBaseUrl { get; init; } = "https://worldcup-vua.nullbit.hr";

        // Relative sub-paths resolved against the app's base directory at runtime — never hard-coded.
        public string DataFolder { get; init; } = "Data";
        public string DefaultPlayerImage { get; init; } = "Assets/default-player.png";
    }
}
