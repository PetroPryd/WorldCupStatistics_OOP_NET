using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Exceptions;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Settings;

namespace WorldCupStatistics.DataLayer.Services
{
    internal sealed class FileSettingsService : ISettingsService
    {
        public string SettingsFilePath => AppDataPaths.SettingsFile;
        public bool Exists => File.Exists(AppDataPaths.SettingsFile);

        public AppSettings? Load()
        {
            if (!File.Exists(AppDataPaths.SettingsFile))
                return null;

            try
            {
                var map = File.ReadAllLines(AppDataPaths.SettingsFile)
                    .Select(l => l.Trim())
                    .Where(l => l.Length > 0 && !l.StartsWith('#') && l.Contains('='))
                    .Select(l => l.Split('=', 2))
                    .ToDictionary(p => p[0].Trim(), p => p[1].Trim(), StringComparer.OrdinalIgnoreCase);

                var s = new AppSettings();

                if (map.TryGetValue("gender", out var g) && Enum.TryParse<Gender>(g, true, out var gender))
                    s.Gender = gender;

                if (map.TryGetValue("language", out var lang) && !string.IsNullOrWhiteSpace(lang))
                    s.Language = lang;

                if (map.TryGetValue("favoriteTeam", out var team) && !string.IsNullOrWhiteSpace(team))
                    s.FavoriteTeamFifaCode = team;

                if (map.TryGetValue("favoritePlayers", out var players) && !string.IsNullOrWhiteSpace(players))
                    s.FavoritePlayerNumbers = players
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(x => int.TryParse(x, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : (int?)null)
                        .Where(n => n.HasValue).Select(n => n!.Value).ToList();

                if (map.TryGetValue("displayMode", out var dm) && Enum.TryParse<DisplayMode>(dm, true, out var mode))
                    s.DisplayMode = mode;

                if (map.TryGetValue("windowWidth", out var w) && int.TryParse(w, NumberStyles.Integer, CultureInfo.InvariantCulture, out var width))
                    s.WindowWidth = width;

                if (map.TryGetValue("windowHeight", out var h) && int.TryParse(h, NumberStyles.Integer, CultureInfo.InvariantCulture, out var height))
                    s.WindowHeight = height;

                return s;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return null;   // corrupt/locked → treat as "no settings", don't crash
            }
        }

        public void Save(AppSettings settings)
        {
            try
            {
                AppDataPaths.EnsureRoot();

                var lines = new[]
                {
                "# WorldCupStatistics shared settings",
                $"gender={settings.Gender}",
                $"language={settings.Language}",
                $"favoriteTeam={settings.FavoriteTeamFifaCode}",
                $"favoritePlayers={string.Join(",", settings.FavoritePlayerNumbers)}",
                $"displayMode={settings.DisplayMode}",
                $"windowWidth={settings.WindowWidth.ToString(CultureInfo.InvariantCulture)}",
                $"windowHeight={settings.WindowHeight.ToString(CultureInfo.InvariantCulture)}"
            };

                File.WriteAllLines(AppDataPaths.SettingsFile, lines);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                throw new DataLayerException($"Could not save settings to '{AppDataPaths.SettingsFile}': {ex.Message}", ex);
            }
        }
    }
}
