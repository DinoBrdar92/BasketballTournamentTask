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
            if (x.Points != y.Points)
            {
                if (x.Points > y.Points)
                {
                    Debug.WriteLine($"{x.ISOCode} has more points than {y.ISOCode} ({x.Points} to {y.Points} points) - {x.ISOCode} goes in front of {y.ISOCode}");
                }
                else
                {
                    Debug.WriteLine($"{y.ISOCode} has more points than {x.ISOCode} ({y.Points} to {x.Points} points) - {y.ISOCode} goes in front of {x.ISOCode}");
                }
                return y.Points.CompareTo(x.Points);
            }

            // Head-to-head comparison
            if (x.HeadToHead.ContainsKey(y.ISOCode) && y.HeadToHead.ContainsKey(x.ISOCode))
            {
                var h2hX = x.HeadToHead[y.ISOCode];
                var h2hY = y.HeadToHead[x.ISOCode];
                string threeWayTieMsg = "";

                // 1. Compare H2H Wins
                if (x.IsInThreeTeamTie && y.IsInThreeTeamTie)
                {
                    Debug.WriteLine($"{x.ISOCode} and {y.ISOCode} are involved in a 3-way tie");
                    h2hX = x.HeadToHead["otherTwo"];
                    h2hY = y.HeadToHead["otherTwo"];
                    threeWayTieMsg = "three-way ";
                }

                if (h2hX.Wins != h2hY.Wins)
                {
                    if (h2hY.Wins > h2hX.Wins)
                    {
                        Debug.WriteLine($"{y.ISOCode} has won H2H against {x.ISOCode} - {y.ISOCode} goes in front of {x.ISOCode}");
                    }
                    else
                    {
                        Debug.WriteLine($"{x.ISOCode} has won H2H against {y.ISOCode} - {x.ISOCode} goes in front of {y.ISOCode}");
                    }

                    return h2hY.Wins.CompareTo(h2hX.Wins);
                }
                


                // 2. Compare H2H Points Difference
                if (h2hX.PointsDifference != h2hY.PointsDifference)
                {
                    if (h2hY.PointsDifference > h2hX.PointsDifference)
                    {
                        Debug.WriteLine($"{y.ISOCode} leads in a {threeWayTieMsg}point difference against {x.ISOCode} ({h2hY.PointsDifference} to {h2hX.PointsDifference}) - {y.ISOCode} goes in front of {x.ISOCode}");
                    }
                    else
                    {
                        Debug.WriteLine($"{x.ISOCode} leads in a {threeWayTieMsg}point difference against {y.ISOCode} ({h2hX.PointsDifference} to {h2hY.PointsDifference}) - {x.ISOCode} goes in front of {y.ISOCode}");
                    }
                    return h2hY.PointsDifference.CompareTo(h2hX.PointsDifference);
                }

                // 3. Compare H2H Points Scored
                if (h2hX.PointsScored != h2hY.PointsScored)
                {
                    if (h2hY.PointsScored > h2hX.PointsScored)
                    {
                        Debug.WriteLine($"{y.ISOCode} leads in {threeWayTieMsg}points scored against {x.ISOCode} ({h2hY.PointsScored} to {h2hX.PointsScored}) - {y.ISOCode} goes in front of {x.ISOCode}");
                    }
                    else
                    {
                        Debug.WriteLine($"{x.ISOCode} leads in {threeWayTieMsg}points scored against {y.ISOCode} ({h2hX.PointsScored} to {h2hY.PointsScored}) - {y.ISOCode} goes in front of {y.ISOCode}");
                    }
                    return h2hY.PointsScored.CompareTo(h2hX.PointsScored);
                }
            }

            // Total points difference
            if (x.PointsDifference != y.PointsDifference)
            {
                if (x.PointsDifference > y.PointsDifference)
                {
                    Debug.WriteLine($"{x.ISOCode} has better point differential than {y.ISOCode} ({x.PointsDifference} to {y.PointsDifference} point diff) - {x.ISOCode} goes in front of {y.ISOCode}");
                }
                else
                {
                    Debug.WriteLine($"{y.ISOCode} has better point differential than {x.ISOCode} ({y.PointsDifference} to {x.PointsDifference} point diff) - {y.ISOCode} goes in front of {x.ISOCode}");
                }
                return y.PointsDifference.CompareTo(x.PointsDifference);
            }

            // Total points scored
            if (x.PointsScored != y.PointsScored)
            {
                if (x.PointsScored > y.PointsScored)
                {
                    Debug.WriteLine($"{x.ISOCode} has scored more points than {y.ISOCode} ({x.PointsScored} to {y.Points} points) - {x.ISOCode} goes in front of {y.ISOCode}");
                }
                else
                {
                    Debug.WriteLine($"{y.ISOCode} has scored more points than {x.ISOCode} ({y.PointsScored} to {x.Points} points) - {y.ISOCode} goes in front of {x.ISOCode}");
                }
                return y.PointsScored.CompareTo(x.PointsScored);
            }

            // FIBA ranking
            Debug.WriteLine($"{y.ISOCode} is ranked higher than {x.ISOCode} in FIBA rankings ({y.Points}. to {x.Points}. place) - {y.ISOCode} goes in front of {x.ISOCode}");
            return x.FIBARanking.CompareTo(y.FIBARanking);
        }
    }
}
