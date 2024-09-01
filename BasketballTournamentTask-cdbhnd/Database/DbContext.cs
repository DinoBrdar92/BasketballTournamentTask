using BasketballTournamentTask_cdbhnd.Database.Helpers;
using BasketballTournamentTask_cdbhnd.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Database
{
    internal class DbContext
    {
        private const string JSON_FILES_PATH = "..\\..\\..\\";
        private const string GROUPS_PATH = JSON_FILES_PATH + "groups.json";
        private const string EXHIBITIONS_PATH = JSON_FILES_PATH + "exibitions.json";

        public Dictionary<string, List<TeamDto>> GroupsDto { get; set; }
        public Dictionary<string, List<GameDto>> ExhibitionsDto { get; set; }

        public DbContext() {
            if (!File.Exists(GROUPS_PATH) || !File.Exists(EXHIBITIONS_PATH))
            {
                InitializeDatabaseContext();
            }

            GroupsDto = DbIO.DeserializeFromJsonFile<string, List<TeamDto>>(GROUPS_PATH) ?? new();
            ExhibitionsDto = DbIO.DeserializeFromJsonFile<string, List<GameDto>>(EXHIBITIONS_PATH) ?? new();
        } 

        protected void InitializeDatabaseContext()
        {

            if (!File.Exists(GROUPS_PATH))
            {
                Dictionary<string, List<TeamDto>>? groups = new Dictionary<string, List<TeamDto>>();
                DbIO.SerializeToJsonFile(GROUPS_PATH, groups);
            }

            if (!File.Exists(EXHIBITIONS_PATH))
            {
                Dictionary<string, List<GameDto>>? exhibitions = new Dictionary<string, List<GameDto>>();
                DbIO.SerializeToJsonFile(EXHIBITIONS_PATH, exhibitions);
            }
        }
    }
}
