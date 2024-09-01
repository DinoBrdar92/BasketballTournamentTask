using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain
{
    internal class CrossGroupTeamComparer : IComparer<Team>
    {
        public int Compare(Team? x, Team? y)
        {
            // Points comparison
            if (x.Points != y.Points)
            {
                Func<Team, Team, string> pointsMsg = (t1, t2) => { return $"{t1.ISOCode} has more points than {t2.ISOCode} ({t1.Points} to {t2.Points})"; };

                Debug.WriteLine(x.Points > y.Points ? pointsMsg(x, y) : pointsMsg(y, x));

                return y.Points.CompareTo(x.Points);
            }

            // Total points difference
            if (x.PointsDifference != y.PointsDifference)
            {
                Func<Team, Team, string> ptsTotMsg = (t1, t2) => { return $"{t1.ISOCode} has a better total point differential than {t2.ISOCode} ({(t1.PointsDifference > 0 ? "+" : "")}{t1.PointsDifference} to {(t2.PointsDifference > 0 ? "+" : "")}{t2.PointsDifference})"; };

                Debug.WriteLine(x.PointsDifference > y.PointsDifference ? ptsTotMsg(x, y) : ptsTotMsg(y, x));

                return y.PointsDifference.CompareTo(x.PointsDifference);
            }

            // Total points scored
            if (x.PointsScored != y.PointsScored)
            {
                Func<Team, Team, string> ptsScoredMsg = (t1, t2) => { return $"{t1.ISOCode} has scored more total points than {t2.ISOCode} ({t1.PointsScored} to {t2.Points})"; };

                Debug.WriteLine(x.PointsScored > y.PointsScored ? ptsScoredMsg(x, y) : ptsScoredMsg(y, x));

                return y.PointsScored.CompareTo(x.PointsScored);
            }

            // FIBA ranking
            Debug.WriteLine($"{y.ISOCode} is ranked higher than {x.ISOCode} in FIBA rankings ({y.Points}. to {x.Points}.)");
            return x.FIBARanking.CompareTo(y.FIBARanking);
        }
    }
}
