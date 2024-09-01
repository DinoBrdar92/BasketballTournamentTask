using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain
{
    class Game
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public int? Team1Score { get; set; }
        public int? Team2Score { get; set; }

        public Game(Team team1, Team team2)
        {
            Team1 = team1;
            Team2 = team2;
        }
    }
}
