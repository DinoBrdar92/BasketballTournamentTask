using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Model.Entities
{
    public class GameDto
    {
        public DateOnly Date { get; set; }
        public string Opponent { get; set; }
        public string Result { get; set; }

        public GameDto(DateOnly date, string opponent, string result)
        {
            Date = date;
            Opponent = opponent;
            Result = result;
        }

    }
}
