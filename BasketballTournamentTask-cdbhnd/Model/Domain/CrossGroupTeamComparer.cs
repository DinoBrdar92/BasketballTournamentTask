using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain
{
    internal class CrossGroupTeamComparer : IComparer<Team>
    {
        public int Compare(Team? x, Team? y)
        {
            if (x.Points != y.Points)
                return y.Points.CompareTo(x.Points);

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
