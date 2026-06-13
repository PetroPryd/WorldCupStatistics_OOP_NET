using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.WinForms.Controls
{
    internal static class ImageLoader
    {
        /// <summary>Loads an image without locking the source file. Returns null on any failure.</summary>
        public static Image? Load(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                var bytes = File.ReadAllBytes(path);
                if (bytes.Length == 0) return null;            // e.g. the empty placeholder
                using var ms = new MemoryStream(bytes);
                return Image.FromStream(ms);
            }
            catch
            {
                return null;                                    // corrupt/unsupported → no image, no crash
            }
        }
    }
}
