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
        private const bool GENERATE_DATA = true;

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

                if (GENERATE_DATA)
                {
                    groups.Add("A",
                        [
                            new TeamDto("Kanada", "CAN", 7),
                            new TeamDto("Australija", "AUS", 5),
                            new TeamDto("Grčka", "GRE", 14),
                            new TeamDto("Španija", "ESP", 2)
                        ]);

                    groups.Add("B",
                        [
                            new TeamDto("Nemačka", "GER", 3),
                            new TeamDto("Francuska", "FRA", 9),
                            new TeamDto("Brazil", "BRA", 12),
                            new TeamDto("Japan", "JPN", 26)
                        ]);

                    groups.Add("C",
                        [
                            new TeamDto("Sjedinjene Države", "USA", 1),
                            new TeamDto("Srbija", "SRB", 4),
                            new TeamDto("Južni Sudan", "SSD", 34),
                            new TeamDto("Puerto Riko", "PRI", 16)
                        ]);

                }
                
                DbIO.SerializeToJsonFile(GROUPS_PATH, groups);
            }

            if (!File.Exists(EXHIBITIONS_PATH))
            {
                Dictionary<string, List<GameDto>>? exhibitions = new Dictionary<string, List<GameDto>>();

                if (GENERATE_DATA)
                {
                    exhibitions.Add("GER",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);

                    exhibitions.Add("FRA",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);

                    exhibitions.Add("JPN",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);

                    exhibitions.Add("USA",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);

                    exhibitions.Add("CAN",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);

                    exhibitions.Add("AUS",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);

                    exhibitions.Add("SRB",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);

                    exhibitions.Add("PRI",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);
                    exhibitions.Add("GRE",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);
                    exhibitions.Add("BRA",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);
                    exhibitions.Add("SSD",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);
                    exhibitions.Add("ESP",
                        [
                            new GameDto(new DateOnly(2024, 7, 6), "FRA", "66-90"),
                            new GameDto(new DateOnly(2024, 7, 19), "JPN", "104-83")
                        ]);

                }
                
                DbIO.SerializeToJsonFile(EXHIBITIONS_PATH, exhibitions);
            }
        }
    }
}
