using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain
{
    public class GroupEntry
    {
        public string Group { get; set; } = null!;
        public Team Team { get; set; } = null!;
        public int Points => Wins * 2 + Losses;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointsScored { get; set; }
        public int PointsReceived { get; set; }
        public int PointsDifference => PointsScored - PointsReceived;
        public bool IsInThreeTeamTie { get; set; } = false;
        public Dictionary<string, HeadToHeadStats> HeadToHead { get; set; } = new Dictionary<string, HeadToHeadStats>();

        public GroupEntry(string group, Team team)
        {
            Group = group;
            Team = team;
        }
    }

    public class HeadToHeadStats
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointsScored { get; set; }
        public int PointsReceived { get; set; }
        public int PointsDifference => PointsScored - PointsReceived;
    }
}
