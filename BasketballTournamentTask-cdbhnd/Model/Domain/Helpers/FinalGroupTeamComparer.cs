using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain.Helpers
{
    internal class FinalGroupTeamComparer : IComparer<GroupEntry>
    {
        public int Compare(GroupEntry? x, GroupEntry? y)
        {
            x = x ?? new GroupEntry();
            y = y ?? new GroupEntry();

            // Points comparison
            if (x.Points != y.Points)
            {
                Func<GroupEntry, GroupEntry, string> pointsMsg = (t1, t2) => { return $"{t1.Team.ISOCode} has more points than {t2.Team.ISOCode} ({t1.Points} to {t2.Points})"; };

                Debug.WriteLine(x.Points > y.Points ? pointsMsg(x, y) : pointsMsg(y, x));

                return y.Points.CompareTo(x.Points);
            }

            // Total points difference
            if (x.PointsDifference != y.PointsDifference)
            {
                Func<GroupEntry, GroupEntry, string> ptsTotMsg = (t1, t2) => { return $"{t1.Team.ISOCode} has a better total point differential than {t2.Team.ISOCode} ({(t1.PointsDifference > 0 ? "+" : "")}{t1.PointsDifference} to {(t2.PointsDifference > 0 ? "+" : "")}{t2.PointsDifference})"; };

                Debug.WriteLine(x.PointsDifference > y.PointsDifference ? ptsTotMsg(x, y) : ptsTotMsg(y, x));

                return y.PointsDifference.CompareTo(x.PointsDifference);
            }

            // Total points scored
            if (x.PointsScored != y.PointsScored)
            {
                Func<GroupEntry, GroupEntry, string> ptsScoredMsg = (t1, t2) => { return $"{t1.Team.ISOCode} has scored more total points than {t2.Team.ISOCode} ({t1.PointsScored} to {t2.PointsScored})"; };

                Debug.WriteLine(x.PointsScored > y.PointsScored ? ptsScoredMsg(x, y) : ptsScoredMsg(y, x));

                return y.PointsScored.CompareTo(x.PointsScored);
            }

            // FIBA ranking
            Func<GroupEntry, GroupEntry, string> fibaRankingMsg = (t1, t2) => { return $"{t1.Team.ISOCode} is ranked higher than {t2.Team.ISOCode} in FIBA rankings ({t1.Team.FIBARanking}. to {t2.Team.FIBARanking}.)"; };

            Debug.WriteLine(x.Team.FIBARanking < y.Team.FIBARanking ? fibaRankingMsg(x, y) : fibaRankingMsg(y, x));

            return x.Team.FIBARanking.CompareTo(y.Team.FIBARanking);
        }
    }
}
