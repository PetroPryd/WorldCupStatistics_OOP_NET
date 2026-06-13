using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Models
{
    public sealed record Player
    {
        public required string Name { get; init; }
        public int ShirtNumber { get; init; }
        public Position Position { get; init; }
        public bool IsCaptain { get; init; }
    }
}
