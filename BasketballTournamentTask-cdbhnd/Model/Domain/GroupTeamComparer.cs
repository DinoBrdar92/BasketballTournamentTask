using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain
{
    public class GroupTeamComparer : IComparer<Team>
    {
        public int Compare(Team? x, Team? y)
        {
            if (x.Points != y.Points)
                return y.Points.CompareTo(x.Points);

            // Head-to-head comparison
            if (x.HeadToHead.ContainsKey(y.Name) && y.HeadToHead.ContainsKey(x.Name))
            {
                var h2hX = x.HeadToHead[y.Name];
                var h2hY = y.HeadToHead[x.Name];

                if (h2hX.Wins != h2hY.Wins)
                    return h2hX.Wins.CompareTo(h2hY.Wins);

                if (h2hX.PointsDifference != h2hY.PointsDifference)
                    return h2hX.PointsDifference.CompareTo(h2hY.PointsDifference);

                if (h2hX.PointsScored != h2hY.PointsScored)
                    return h2hX.PointsScored.CompareTo(h2hY.PointsScored);
            }

            // Total points difference
            if (x.PointsDifference != y.PointsDifference)
                return y.PointsDifference.CompareTo(x.PointsDifference);

            // Total points scored
            if (x.PointsScored != y.PointsScored)
                return y.PointsScored.CompareTo(x.PointsScored);

            // FIBA ranking
            return x.FIBARanking.CompareTo(y.FIBARanking);
        }
    }
}
