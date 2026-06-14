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

    public sealed class DataSourceConfig
    {
        public const string SectionName = "DataSource";

        public DataSourceMode Mode { get; init; } = DataSourceMode.Json;
        public string ApiBaseUrl { get; init; } = "https://worldcup-vua.nullbit.hr";

        public string DataFolder { get; init; } = "Data";
        public string DefaultPlayerImage { get; init; } = "Assets/default-player.png";
    }
}
