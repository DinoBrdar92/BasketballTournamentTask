using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Entities
{
    public class TeamDto
    {
        public string Team { get; set; }
        public string ISOCode { get; set; }
        public int FIBARanking { get; set; }

        public TeamDto(string team, string iSOCode, int fIBARanking)
        {
            Team = team;
            ISOCode = iSOCode;
            FIBARanking = fIBARanking;
        }
    }
}
