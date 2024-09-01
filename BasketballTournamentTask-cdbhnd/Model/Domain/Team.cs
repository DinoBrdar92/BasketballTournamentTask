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
        public string Name { get; set; } = null!;
        public int FIBARanking { get; set; }
        public double ELORating { get; set; }

        public Dictionary<string, HeadToHeadStats> HeadToHead { get; set; } = new Dictionary<string, HeadToHeadStats>();

        public Team(string iSOCode, string name, int fIBARanking)
        {
            ISOCode = iSOCode;
            Name = name;
            FIBARanking = fIBARanking;
        }
    }
}
