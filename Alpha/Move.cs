using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.WorldGeneration;

namespace Alpha
{
    class Move
    {
        public VoronoiSite Source { get; set; }
        public VoronoiSite Destination { get; set; }
        public Fleet Entity { get; set; }
    }
}
