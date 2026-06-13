using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Exceptions
{
    public sealed class DataLayerException : Exception
    {
        public DataLayerException(string message) : base(message) { }
        public DataLayerException(string message, Exception inner) : base(message, inner) { }
    }
}
