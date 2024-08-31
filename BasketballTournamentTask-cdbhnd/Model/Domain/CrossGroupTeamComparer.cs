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

            // Total points difference
            if (x.PointsDifference != y.PointsDifference)
            {
                if (x.PointsDifference > y.PointsDifference)
                {
                    Debug.WriteLine($"{x.ISOCode} has a better points differential than {y.ISOCode} ({x.PointsDifference} to {y.PointsDifference} point diff) - {x.ISOCode} goes in front of {y.ISOCode}");
                }
                else
                {
                    Debug.WriteLine($"{y.ISOCode} has a better points differential than {x.ISOCode} ({y.PointsDifference} to {x.PointsDifference} point diff) - {y.ISOCode} goes in front of {x.ISOCode}");
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
            Debug.WriteLine($"{y.ISOCode} is placed higher than {x.ISOCode} in FIBA rankings - ({y.Points} to {x.Points} points) - {y.ISOCode} goes in front of {x.ISOCode}");
            return x.FIBARanking.CompareTo(y.FIBARanking);
        }
    }
}
