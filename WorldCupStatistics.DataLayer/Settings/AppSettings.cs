using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;

namespace WorldCupStatistics.DataLayer.Settings
{
    public enum DisplayMode { Fullscreen, Windowed }

    public sealed class AppSettings
    {
        public Gender Gender { get; set; } = Gender.Men;
        public string Language { get; set; } = "en";                 // "en" or "hr"
        public string? FavoriteTeamFifaCode { get; set; }
        public List<int> FavoritePlayerNumbers { get; set; } = [];   // shirt numbers, unique within a team
        public DisplayMode DisplayMode { get; set; } = DisplayMode.Fullscreen;
        public int WindowWidth { get; set; } = 1280;                 // used when DisplayMode = Windowed
        public int WindowHeight { get; set; } = 720;
    }
}
