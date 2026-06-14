using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Settings;

namespace WorldCupStatistics.DataLayer.Services
{
    public interface ISettingsService
    {
        string SettingsFilePath { get; }
        bool Exists { get; }

        AppSettings? Load();

        void Save(AppSettings settings);
    }
}
