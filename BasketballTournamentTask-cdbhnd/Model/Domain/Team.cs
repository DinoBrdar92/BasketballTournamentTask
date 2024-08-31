using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain
{
    public class Team
    {
        public string ISOCode { get; set; }
        public string Name { get; set; } = string.Empty;
        public int FIBARanking { get; set; }
        public string Group { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses {  get; set; }
        public int PointsScored { get; set; }
        public int PointsReceived { get; set; }
        public double ELORating { get; set; }

        public Team(string iSOCode, string name, int fIBARanking, string group)
        {
            ISOCode = iSOCode;
            Name = name;
            FIBARanking = fIBARanking;
            Group = group;
        }
    }
}
