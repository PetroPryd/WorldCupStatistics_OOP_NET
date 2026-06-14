using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Configuration;
using WorldCupStatistics.DataLayer.Exceptions;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Settings;

namespace WorldCupStatistics.DataLayer.Services
{
    internal sealed class ImageService : IImageService
    {
        private readonly string _defaultImagePath;

        public ImageService(IOptions<DataSourceConfig> options)
            => _defaultImagePath = Path.Combine(AppContext.BaseDirectory, options.Value.DefaultPlayerImage);

        public string DefaultImagePath => _defaultImagePath;

        public bool HasCustomImage(Gender gender, string fifaCode, int shirtNumber)
            => FindCustomImage(gender, fifaCode, shirtNumber) is not null;

        public string GetPlayerImagePath(Gender gender, string fifaCode, int shirtNumber)
            => FindCustomImage(gender, fifaCode, shirtNumber) ?? _defaultImagePath;

        public async Task SetPlayerImageAsync(Gender gender, string fifaCode, int shirtNumber, string sourceFilePath, CancellationToken ct = default)
        {
            if (!File.Exists(sourceFilePath))
                throw new DataLayerException($"Source image not found: {sourceFilePath}");

            try
            {
                var dir = PlayerDir(gender, fifaCode);
                Directory.CreateDirectory(dir);

                foreach (var existing in Directory.EnumerateFiles(dir, $"{shirtNumber}.*"))
                    File.Delete(existing);

                var extension = Path.GetExtension(sourceFilePath);
                if (string.IsNullOrWhiteSpace(extension)) extension = ".png";
                var destination = Path.Combine(dir, $"{shirtNumber}{extension}");

                await using var source = File.OpenRead(sourceFilePath);
                await using var target = File.Create(destination);
                await source.CopyToAsync(target, ct);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                throw new DataLayerException($"Could not save player image: {ex.Message}", ex);
            }
        }

        private static string PlayerDir(Gender gender, string fifaCode)
            => Path.Combine(AppDataPaths.ImagesRoot, gender.ToSegment(), fifaCode);

        private static string? FindCustomImage(Gender gender, string fifaCode, int shirtNumber)
        {
            var dir = PlayerDir(gender, fifaCode);
            return Directory.Exists(dir)
                ? Directory.EnumerateFiles(dir, $"{shirtNumber}.*").FirstOrDefault()
                : null;
        }
    }
}
