using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;

namespace WorldCupStatistics.DataLayer.Services
{
    public interface IImageService
    {
        string DefaultImagePath { get; }
        bool HasCustomImage(Gender gender, string fifaCode, int shirtNumber);
        string GetPlayerImagePath(Gender gender, string fifaCode, int shirtNumber);
        Task SetPlayerImageAsync(Gender gender, string fifaCode, int shirtNumber, string sourceFilePath, CancellationToken ct = default);
    }
}
