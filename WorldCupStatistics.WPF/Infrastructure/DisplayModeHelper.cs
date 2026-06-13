using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorldCupStatistics.DataLayer.Settings;

namespace WorldCupStatistics.WPF.Infrastructure
{
    public static class DisplayModeHelper
    {
        public static void Apply(Window window, AppSettings settings)
        {
            if (settings.DisplayMode == DisplayMode.Fullscreen)
            {
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.WindowState = WindowState.Normal;
                window.Width = settings.WindowWidth;
                window.Height = settings.WindowHeight;
            }
        }
    }
}
