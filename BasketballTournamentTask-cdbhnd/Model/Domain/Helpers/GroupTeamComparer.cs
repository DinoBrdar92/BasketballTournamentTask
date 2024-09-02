using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Domain.Helpers
{
    public class GroupTeamComparer : IComparer<GroupEntry>
    {
        public int Compare(GroupEntry? x, GroupEntry? y)
        {
            x = x ?? new GroupEntry();
            y = y ?? new GroupEntry();

            // Poređenje bodova
            if (x.Points != y.Points)
            {
                Func<GroupEntry, GroupEntry, string> pointsMsg = (t1, t2) => { return $"{t1.Team.ISOCode} has more points than {t2.Team.ISOCode} ({t1.Points} to {t2.Points})"; };

                Debug.WriteLine(x.Points > y.Points ? pointsMsg(x, y) : pointsMsg(y, x));

                return y.Points.CompareTo(x.Points);
            }

            // Međusobno poređenje između jedno ili više timova
            if (x.HeadToHead.ContainsKey(y.Team.ISOCode) && y.HeadToHead.ContainsKey(x.Team.ISOCode))
            {
                string threeWayTieMsg = "";

                var h2hX = x.HeadToHead[y.Team.ISOCode];
                var h2hY = y.HeadToHead[x.Team.ISOCode];

                // 1. Poređenje pobednika u međusobnom duelu
                if (x.IsInThreeTeamTie && y.IsInThreeTeamTie)
                {
                    Debug.WriteLine($"{x.Team.ISOCode} and {y.Team.ISOCode} are involved in a 3-way tie");
                    threeWayTieMsg = "three-way ";

                    h2hX = x.HeadToHead["otherTwo"];
                    h2hY = y.HeadToHead["otherTwo"];
                }

                if (h2hX.Wins != h2hY.Wins)
                {
                    Func<GroupEntry, GroupEntry, string> h2hMsg = (t1, t2) => { return $"{t2.Team.ISOCode} has won H2H against {t1.Team.ISOCode}"; };

                    Debug.WriteLine(h2hY.Wins > h2hX.Wins ? h2hMsg(x, y) : h2hMsg(y, x));

                    return h2hY.Wins.CompareTo(h2hX.Wins);
                }



                // 2. Poređenje poen razlike u međusobnim duelima
                if (h2hX.PointsDifference != h2hY.PointsDifference)
                {
                    Func<GroupEntry, HeadToHeadStats, GroupEntry, HeadToHeadStats, string> ptsDiffMsg = (t1, h2hT1, t2, h2hT2) => { return $"{t2.Team.ISOCode} has a better {threeWayTieMsg}point differential compared to {t1.Team.ISOCode} ({(h2hT2.PointsDifference > 0 ? "+" : "")}{h2hT2.PointsDifference} to {(h2hT1.PointsDifference > 0 ? "+" : "")}{h2hT1.PointsDifference})"; };

                    Debug.WriteLine(h2hY.PointsDifference > h2hX.PointsDifference ? ptsDiffMsg(x, h2hX, y, h2hY) : ptsDiffMsg(y, h2hY, x, h2hX));

                    return h2hY.PointsDifference.CompareTo(h2hX.PointsDifference);
                }

                // 3. Poređenje datih poena u međusobnim duelima
                if (h2hX.PointsScored != h2hY.PointsScored)
                {
                    Func<GroupEntry, HeadToHeadStats, GroupEntry, HeadToHeadStats, string> ptsScoredMsg = (t1, h2hT1, t2, h2hT2) => { return $"{t2.Team.ISOCode} leads in {threeWayTieMsg}points scored compared to {t1.Team.ISOCode} ({h2hT2.PointsScored} to {h2hT1.PointsScored})"; };

                    Debug.WriteLine(h2hY.PointsScored > h2hX.PointsScored ? ptsScoredMsg(x, h2hX, y, h2hY) : ptsScoredMsg(y, h2hY, x, h2hX));

                    return h2hY.PointsScored.CompareTo(h2hX.PointsScored);
                }
            }

            // Poređenje ukupne poen razlike
            if (x.PointsDifference != y.PointsDifference)
            {
                Func<GroupEntry, GroupEntry, string> ptsTotMsg = (t1, t2) => { return $"{t1.Team.ISOCode} has a better total point differential than {t2.Team.ISOCode} ({(t1.PointsDifference > 0 ? "+" : "")}{t1.PointsDifference} to {(t2.PointsDifference > 0 ? "+" : "")}{t2.PointsDifference})"; };

                Debug.WriteLine(x.PointsDifference > y.PointsDifference ? ptsTotMsg(x, y) : ptsTotMsg(y, x));

                return y.PointsDifference.CompareTo(x.PointsDifference);
            }

            // Poređenje ukupno datih poena
            if (x.PointsScored != y.PointsScored)
            {
                Func<GroupEntry, GroupEntry, string> ptsScoredMsg = (t1, t2) => { return $"{t1.Team.ISOCode} has scored more total points than {t2.Team.ISOCode} ({t1.PointsScored} to {t2.PointsScored})"; };

                Debug.WriteLine(x.PointsScored > y.PointsScored ? ptsScoredMsg(x, y) : ptsScoredMsg(y, x));

                return y.PointsScored.CompareTo(x.PointsScored);
            }

            // Poređenje pozicije na FIBA rang listi
            Func<GroupEntry, GroupEntry, string> fibaRankingMsg = (t1, t2) => { return $"{t1.Team.ISOCode} is ranked higher than {t2.Team.ISOCode} in FIBA rankings ({t1.Team.FIBARanking}. to {t2.Team.FIBARanking}.)"; };

            Debug.WriteLine(x.Team.FIBARanking < y.Team.FIBARanking ? fibaRankingMsg(x, y) : fibaRankingMsg(y, x));

            return x.Team.FIBARanking.CompareTo(y.Team.FIBARanking);
        }
    }
}
