using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain
{
    public class HeadToHeadStats
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointsScored { get; set; }
        public int PointsReceived { get; set; }
        public int PointsDifference => PointsScored - PointsReceived;
    }
}
