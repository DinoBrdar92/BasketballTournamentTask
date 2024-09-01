using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain
{
    public class GroupTeamComparer : IComparer<Team>
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

            // Head-to-head comparison
            if (x.HeadToHead.ContainsKey(y.ISOCode) && y.HeadToHead.ContainsKey(x.ISOCode))
            {
                string threeWayTieMsg = "";

                var h2hX = x.HeadToHead[y.ISOCode];
                var h2hY = y.HeadToHead[x.ISOCode];

                // 1. Compare H2H Wins
                if (x.IsInThreeTeamTie && y.IsInThreeTeamTie)
                {
                    Debug.WriteLine($"{x.ISOCode} and {y.ISOCode} are involved in a 3-way tie");
                    threeWayTieMsg = "three-way ";

                    h2hX = x.HeadToHead["otherTwo"];
                    h2hY = y.HeadToHead["otherTwo"];
                }

                if (h2hX.Wins != h2hY.Wins)
                {
                    Func<Team, Team, string> h2hMsg = (t1, t2) => { return $"{t2.ISOCode} has won H2H against {t1.ISOCode}"; };

                    Debug.WriteLine(h2hY.Wins > h2hX.Wins ? h2hMsg(x, y) : h2hMsg(y, x));

                    return h2hY.Wins.CompareTo(h2hX.Wins);
                }
                


                // 2. Compare H2H Points Difference
                if (h2hX.PointsDifference != h2hY.PointsDifference)
                {
                    Func<Team, HeadToHeadStats, Team, HeadToHeadStats, string> ptsDiffMsg = (t1, h2hT1, t2, h2hT2) => { return $"{t2.ISOCode} has a better {threeWayTieMsg}point differential compared to {t1.ISOCode} ({(h2hT2.PointsDifference > 0 ? "+" : "")}{h2hT2.PointsDifference} to {(h2hT1.PointsDifference > 0 ? "+" : "")}{h2hT1.PointsDifference})";  };
                    
                    Debug.WriteLine(h2hY.PointsDifference > h2hX.PointsDifference ? ptsDiffMsg(x, h2hX, y, h2hY) : ptsDiffMsg(y, h2hY, x, h2hX));
                    
                    return h2hY.PointsDifference.CompareTo(h2hX.PointsDifference);
                }

                // 3. Compare H2H Points Scored
                if (h2hX.PointsScored != h2hY.PointsScored)
                {
                    Func<Team, HeadToHeadStats, Team, HeadToHeadStats, string> ptsScoredMsg = (t1, h2hT1, t2, h2hT2) => { return $"{t2.ISOCode} leads in {threeWayTieMsg}points scored compared to {t1.ISOCode} ({h2hT2.PointsScored} to {h2hT1.PointsScored})"; };

                    Debug.WriteLine(h2hY.PointsScored > h2hX.PointsScored ? ptsScoredMsg(x, h2hX, y, h2hY) : ptsScoredMsg(y, h2hY, x, h2hX));

                    return h2hY.PointsScored.CompareTo(h2hX.PointsScored);
                }
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
