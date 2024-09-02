using BasketballTournamentTask_cdbhnd.Database;
using BasketballTournamentTask_cdbhnd.Model;
using BasketballTournamentTask_cdbhnd.Model.Domain;
using BasketballTournamentTask_cdbhnd.Model.Domain.Helpers;
using BasketballTournamentTask_cdbhnd.Model.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace BasketballTournamentTask_cdbhnd
{
    internal class Program
    {
        private const bool TEST_PREDEFINED_GAMES = false;
        public static DbContext Dbc { get; set; } = new();

        private static Dictionary<string, Team> allTeams = new();

        private static Dictionary<string, Dictionary<string, GroupEntry>> allGroups = new();

        static void Main(string[] args)
        {
            //inicijalizacija iz baze (čitanje json fajlova i deserijalizacija u objekte)
            Dbc = new DbContext();

            
            foreach (var groupDto in Dbc.GroupsDto)
            {
                string groupLetter = groupDto.Key;

                allGroups.Add(groupLetter, new Dictionary<string, GroupEntry>());

                foreach (TeamDto teamDto in groupDto.Value)
                {
                    Team team = new(teamDto.ISOCode, teamDto.Team, teamDto.FIBARanking);

                    allTeams.Add(teamDto.ISOCode, team);

                    allGroups[groupLetter].Add(team.Name, new GroupEntry(groupLetter, team));
                }


            }

            //računanje inicijalnog ELO ratinga za sve timove
            foreach (string teamCode in allTeams.Keys)
            {
                ExhibitionGamesToElo(teamCode);
            }

            Console.WriteLine("  ***************************************************");
            Console.WriteLine(" *    OLYMPICS BASKETBALL TOURNAMENT SIMULATOR     *");
            Console.WriteLine("**************************************************");

            if (!args.Contains("-v"))
                Console.WriteLine("      .,::OOO::,.     .,ooOOOoo,.     .,::OOO::,.\r\n    .:'         `:. .8'         `8. .:'         `:.\r\n    :\"           \": 8\"           \"8 :\"           \":\r\n    :,        .,:::\"\"::,.     .,:o8OO::,.        ,:__  o\\\r\n     :,,    .:' ,:   8oo`:. .:'oo8   :,,`:.    ,,:  W    \\O\r\n      `^OOoo:\"O^'     `^88oo:\"8^'     `^O\":ooOO^'         |\\_\r\n            :,           ,: :,           ,:              /-\\\r\n             :,,       ,,:   :,,       ,,:               \\   \\\r\n              `^Oo,,,oO^'     `^OOoooOO^'");

            Console.WriteLine("\n");
            Console.WriteLine("                   - GROUP STAGE -");

            //raspored u kojem kolu ko s kim igra


            Dictionary<string, List<(int, int)>> roundsSchedule = new Dictionary<string, List<(int, int)>>
            {
                { "I kolo", [(0, 1),(2, 3)] },
                { "II kolo", [(1, 2),(3, 0)] },
                { "III kolo", [(1, 3),(0, 2)] }
            };



#pragma warning disable CS0162 // Unreachable code detected

            //namerno potisnuo upozorenje jer tako radi po dizajnu, u zavisnosti od vrednosti polja TEST_PREDEFINED_GAMES iz zaglavlja
            //simulacija mečeva u grupi
            if (!TEST_PREDEFINED_GAMES)
                SimulateAllGroupStageGames(roundsSchedule);
            else
                CreatePredefinedGroupStageGames();


#pragma warning restore CS0162 // Unreachable code detected



            SortedDictionary<string, List<GroupEntry>> allGroupsList = new();

            Console.WriteLine("\n\n                    - GROUPS -");

            //ispisivanje tabela po odigravanju svih mečeva
            foreach (var group in allGroups)
            {
                Console.WriteLine($"\nGrupa {group.Key}:\t\t|Pts| W | L | FOR | AGT | -/+");
                Console.WriteLine("------------------------+---+---+---+-----+-----+-----");

                List<GroupEntry> groupList = allGroups[group.Key].Values.ToList();
                
                //grupiši timove u grupi po bodovima i proveri postoji li situacija gde 3 tima imaju isti broj bodova
                List<IGrouping<int, GroupEntry>> threeTeamTie = groupList.GroupBy(x => x.Points).Where(x => x.Count() > 2).ToList();

                if (threeTeamTie.Count == 1 && threeTeamTie.FirstOrDefault()?.Count() == 3)
                {
                    threeTeamTie.FirstOrDefault()?.All(x => x.IsInThreeTeamTie = true);

                    GroupEntry oneOfThreeTeams = threeTeamTie.FirstOrDefault()?.FirstOrDefault() ?? new GroupEntry();
                    int threeTeamTiePointsAmount = oneOfThreeTeams.Points;

                    List<GroupEntry> tttMembersList = groupList.Where(x => x.Points == threeTeamTiePointsAmount).ToList();

                    string[] tttNameCodes = tttMembersList.Select(x => x.Team.ISOCode).ToArray();


                    foreach (GroupEntry tttMember in tttMembersList)
                    {
                        //KeyValuePair<string, HeadToHeadStats> otherTwo = new KeyValuePair<string, HeadToHeadStats>(team.ISOCode, new HeadToHeadStats());
                        tttMember.HeadToHead.Add("otherTwo", new HeadToHeadStats());

                        foreach (string nameCode in tttNameCodes)
                        {
                            HeadToHeadStats h2hWithOneOfOtherTeams;

                            if (tttMember.Team.ISOCode == nameCode)
                            {
                                continue;
                            }

                            h2hWithOneOfOtherTeams = tttMember.HeadToHead[nameCode];

                            tttMember.HeadToHead["otherTwo"].Wins += h2hWithOneOfOtherTeams.Wins;
                            tttMember.HeadToHead["otherTwo"].Losses += h2hWithOneOfOtherTeams.Losses;
                            tttMember.HeadToHead["otherTwo"].PointsScored += h2hWithOneOfOtherTeams.PointsScored;
                            tttMember.HeadToHead["otherTwo"].PointsReceived += h2hWithOneOfOtherTeams.PointsReceived;

                        }
                    }

                }

                //sortiranje
                groupList.Sort(new GroupTeamComparer());

                allGroupsList.Add(group.Key, groupList);
                
                //ispis tabele na konzolu
                for (int i = 0; i < groupList.Count; i++)
                {
                    string preSpaces = Math.Abs(groupList[i].PointsDifference).ToString().Length == 1 ? "  " : Math.Abs(groupList[i].PointsDifference).ToString().Length == 2 ? " " : "";
                    char prefix = groupList[i].PointsDifference < 0 ? '-' : '+';
                    string tabSeparator = groupList[i].Team.Name.Length > 16 ? "\t" : "\t\t";

                    Console.WriteLine($"{i + 1}. {groupList[i].Team.Name}{tabSeparator}| {groupList[i].Points} | {groupList[i].Wins} | {groupList[i].Losses} | {groupList[i].PointsScored} | {groupList[i].PointsReceived} | {preSpaces}{prefix}{Math.Abs(groupList[i].PointsDifference)}");
                }
                Console.WriteLine("------------------------+---+---+---+-----+-----+-----");
            }

            Console.WriteLine("Pts - Bodovi\nW - Pobede\nL - Porazi\nFOR - Dati poeni\nAGT - Primljeni poeni\n-/+ - Koš razlika");


            //kreiranje plasmana od 1. do 9. mesta za šešire
            List<GroupEntry> bigTable = new List<GroupEntry>();

            for (int i = 0; i < 3; i++)
            {
                List<GroupEntry> sameRankSubTable = new List<GroupEntry>();

                foreach (List<GroupEntry> group in allGroupsList.Values)
                {
                    sameRankSubTable.Add(group[i]);
                }

                sameRankSubTable.Sort(new FinalGroupTeamComparer());
                bigTable.AddRange(sameRankSubTable);
            }

            Console.WriteLine("\nPlasman od 1. do 9. mesta nakon grupne faze:");
            Console.WriteLine($"\n\t\t\t|Pts| W | L | FOR | AGT | -/+");
            Console.WriteLine("------------------------+---+---+---+-----+-----+-----");
            for (int i = 0; i < bigTable.Count; i++)
            {
                string preSpaces = Math.Abs(bigTable[i].PointsDifference).ToString().Length == 1 ? "  " : Math.Abs(bigTable[i].PointsDifference).ToString().Length == 2 ? " " : "";
                char prefix = bigTable[i].PointsDifference < 0 ? '-' : '+';
                string tabSeparator = bigTable[i].Team.Name.Length > 16 ? "\t" : "\t\t";
                
                //odvojiti 9. na tabeli da se naglasi da je eliminisan
                if (i == bigTable.Count - 1)
                {
                    Console.WriteLine("------------------------+---+---+---+-----+-----+-----");
                }

                Console.WriteLine($"{i + 1}. {bigTable[i].Team.Name}{tabSeparator}| {bigTable[i].Points} | {bigTable[i].Wins} | {bigTable[i].Losses} | {bigTable[i].PointsScored} | {bigTable[i].PointsReceived} | {preSpaces}{prefix}{Math.Abs(bigTable[i].PointsDifference)}");
            }

            Console.WriteLine("\n\n                      - DRAW -");

            //simulacija žreba

            List<List<GroupEntry>> hats = new List<List<GroupEntry>>();

            for (int i = 0; i < bigTable.Count - 1; i++)
            {
                int hatNo = i / 2;
                int placeInHat = i % 2;
                
                if (placeInHat == 0)
                {
                    hats.Add(new List<GroupEntry>());
                }

                hats[hatNo].Add(bigTable[i]);
            }

            Console.WriteLine("\nŠeširi:");
            char firstLetter = 'D';

            for (int i = 0; i < hats.Count; i++)
            {
                Console.WriteLine($"\tŠešir {(char)((int)firstLetter + i)}");

                foreach (var team in hats[i])
                {
                    Console.WriteLine($"\t\t[{team.Group}] {team.Team.Name}");
                }
            }

            List<Game> quarterfinalGames = new List<Game>();

            for (int i = 0; i < hats.Count / 2; i++)
            {
                List<Game> potentialPairs = new List<Game>();

                List<GroupEntry> topHat = new List<GroupEntry>(hats[i]);
                List<GroupEntry> bottomHat = new List<GroupEntry>(hats[hats.Count - i - 1]);

                Team? topTeam1 = null, bottomTeam1 = null, topTeam2 = null, bottomTeam2 = null;

                // prvo proveri da li postoje timovi u gornjem i donjem šeširu koji su iz iste grupe
                foreach (string groupLetter in allGroups.Keys)
                {
                    if (!topHat.Any(t => t.Group == groupLetter) || !bottomHat.Any(bt => bt.Group == groupLetter))
                    {
                        continue;
                    }

                    // ako postoje, upari timove tako da oni iz istih grupa ne igraju jedan protiv drugog
                    GroupEntry topGroupEntry1 = topHat.Where(t => t.Group == groupLetter).FirstOrDefault() ?? new();
                    GroupEntry bottomGroupEntry1 = bottomHat.Where(t => t.Group != groupLetter).FirstOrDefault() ?? new();

                    topTeam1 = topGroupEntry1.Team;
                    bottomTeam1 = bottomGroupEntry1.Team;

                    topHat.Remove(topGroupEntry1);
                    bottomHat.Remove(bottomGroupEntry1);

                    topTeam2 = topHat[0].Team;
                    bottomTeam2 = bottomHat[0].Team;

                    break;
                }

                // ako vrednost nijednog tima nije dodeljena, znači da su iz različitih šešira, te biraj nasumično
                // (dovoljno je proveriti samo za jedan, ali sam proverio za sve, jer u suprotnom viče ispod da vrednost može biti null)
                // ovde će ući samo ako imaju situacije da su npr. u šeširu E dva tima iz A, a u šeširu F da su timovi iz grupa B i C.
                if (topTeam1 is null || bottomTeam1 is null || topTeam2 is null || bottomTeam2 is null)
                {
                    Random rand1 = new Random();

                    int randomIndexTop = rand1.Next(0, 2);
                    int randomIndexBottom = rand1.Next(0, 2);

                    topTeam1 = topHat[randomIndexTop].Team;
                    bottomTeam1 = bottomHat[randomIndexBottom].Team;

                    topHat.RemoveAt(randomIndexTop);
                    bottomHat.RemoveAt(randomIndexBottom);

                    topTeam2 = topHat[0].Team;
                    bottomTeam2 = bottomHat[0].Team;
                }
                
                Game game1 = new Game(topTeam1, bottomTeam1);
                potentialPairs.Add(game1);

                Game game2 = new Game(topTeam2, bottomTeam2);
                potentialPairs.Add(game2);


                quarterfinalGames.AddRange(potentialPairs);
            }

            //"Winners of the Quarter-Finals played by the teams from Pot D cannot play each other in the Semi-Final games."
            Random rand = new Random();
            int randomIndex = rand.Next(2, 4);

            Game temp = quarterfinalGames[1];
            quarterfinalGames[1] = quarterfinalGames[randomIndex];
            quarterfinalGames[randomIndex] = temp;

            //staviti par D/G da je gore, a E/F da je dole
            if (randomIndex == 3)
            {
                temp = quarterfinalGames[2];
                quarterfinalGames[2] = quarterfinalGames[3];
                quarterfinalGames[3] = temp;
            }
            
            //štampaj izvučene parove
            Console.WriteLine("\nIzvučeni parovi:");
            for (int i = 0; i < quarterfinalGames.Count; i++)
            {
                Console.WriteLine($"\t{quarterfinalGames[i].Team1.Name} - {quarterfinalGames[i].Team2.Name}");

                if (i != 0 && i % 2 == 1)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\n\n              - KNOCKOUT STAGE -");


            //simulacija mečeva nokaut faze (četvrtfinale, polufinale, meč za 3. mesto, finale)
            List<Game> semifinalGames = new List<Game>();

            Console.WriteLine("\nČetvrtfinale:");
            for (int i = 0; i <= quarterfinalGames.Count / 2; i = i + 2)
            {
                Team team1 = KnockoutStageGame(quarterfinalGames[i]).Item1;
                Team team2 = KnockoutStageGame(quarterfinalGames[i + 1]).Item1;


                semifinalGames.Add(new Game(team1, team2));
                Console.WriteLine();
            }

            Game finalGame;
            Game thirdPlaceGame;

            Console.WriteLine("Polufinale:");
            (Team, Team) teams1 = KnockoutStageGame(semifinalGames[0]);
            (Team, Team) teams2 = KnockoutStageGame(semifinalGames[1]);

            finalGame = new Game(teams1.Item1, teams2.Item1);
            thirdPlaceGame = new Game(teams1.Item2, teams2.Item2);
            Console.WriteLine();

            Console.WriteLine("Utakmica za treće mesto:");
            Team bronzeTeam = KnockoutStageGame(thirdPlaceGame).Item1;

            Console.WriteLine("\nFinale:");
            (Team, Team) finalTeams = KnockoutStageGame(finalGame);
            Team silverTeam = finalTeams.Item2;
            Team goldTeam = finalTeams.Item1;

            Console.WriteLine("\nMedalje:");
            Console.WriteLine($"\t1. GOLD:\t{goldTeam.Name}");
            Console.WriteLine($"\t2. SILVER:\t{silverTeam.Name}");
            Console.WriteLine($"\t3. BRONZE:\t{bronzeTeam.Name}");

            if (!args.Contains("-v"))
                Console.WriteLine($"\n\n              {goldTeam.ISOCode}\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⡿⠿⠿⢿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣶⠀⢸⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⠿⠀⠸⢿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀{silverTeam.ISOCode}⠀⠀⠀⢸⣿⣿⣿⣶⣶⣶⣾⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⣤⣤⣤⣤⣤⣤⣤⣤⣤⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⣿⣿⣟⢉⣉⠉⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀{bronzeTeam.ISOCode}⠀⠀⠀⠀\r\n⠀⣿⣿⣿⣿⠟⢀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣶⣶⣶⣶⣶⣶⣶⣶⠀\r\n⠀⣿⣿⣟⠁⠐⠛⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣉⣉⡉⢹⣿⣿⠀\r\n⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣉⡀⢸⣿⣿⠀\r\n⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣉⣉⣁⣼⣿⣿⠀\r\n⠀⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠀");

        }


        /// <summary>
        /// Aproksimira ELO rejting na osnovu pozicije na FIBA rang listi
        /// </summary>
        /// <param name="fibaRanking">pozicija reprezentacije na FIBA rang listi</param>
        /// <returns>ELO rejting</returns>
        private static double FibaRankingToElo(int fibaRanking)
        {
            return 4200 / (fibaRanking + 4) + 950;
        }

        /// <summary>
        /// Ažurira ELO rejting tima na osnovu odigranih prijateljskih mečeva
        /// </summary>
        /// <param name="teamCode">ISO oznaka tima</param>
        private static void ExhibitionGamesToElo(string teamCode)
        {
            int adjustment = 30;
            Team team = allTeams[teamCode];
            double elo = FibaRankingToElo(team.FIBARanking);
            List<GameDto> exhibitionsByTeam = Dbc.ExhibitionsDto.GetValueOrDefault(team.ISOCode) ?? new();

            foreach (GameDto exhibition in exhibitionsByTeam)
            {
                double opponentElo = FibaRankingToElo(allTeams[exhibition.Opponent].FIBARanking);
                double expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (opponentElo - elo) / 400.0) );

                int[] result = exhibition.Result.Split('-').Select(int.Parse).ToArray();
                int scoreDifference = result[0] - result[1];

                int actualOutcome = scoreDifference > 0 ? 1 : 0;

                elo = elo + adjustment * (actualOutcome - expectedOutcome);
            }

            team.ELORating = elo;
        }

        private static void SimulateAllGroupStageGames(Dictionary<string, List<(int, int)>> roundsSchedule)
        {
            foreach (var round in roundsSchedule)
            {
                Console.WriteLine($"{round.Key}:");

                foreach (var group in allGroups)
                {
                    string groupLetter = group.Key;

                    Console.WriteLine($"\tGrupa {groupLetter}:");

                    foreach ((int, int) pair in round.Value)
                    {
                        GroupEntry teamInGroup1 = group.Value.ElementAt(pair.Item1).Value;
                        GroupEntry teamInGroup2 = group.Value.ElementAt(pair.Item2).Value;

                        GroupStageGame(teamInGroup1, teamInGroup2);

                    }
                }
                Console.WriteLine();
            }
        }

        private static void CreatePredefinedGroupStageGames()
        {
            //for copy-pasting and just editing results
            /*
            Console.WriteLine("I kolo:\n\tGrupa A:");
            GroupStageGame(GetGroupEntryFromTeamCode("CAN") ?? new(), GetGroupEntryFromTeamCode("AUS") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("GRE") ?? new(), GetGroupEntryFromTeamCode("ESP") ?? new(), 0, 0, 0);
            Console.WriteLine("\tGrupa B:");
            GroupStageGame(GetGroupEntryFromTeamCode("GER") ?? new(), GetGroupEntryFromTeamCode("FRA") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("BRA") ?? new(), GetGroupEntryFromTeamCode("JPN") ?? new(), 0, 0, 0);
            Console.WriteLine("\tGrupa C:");
            GroupStageGame(GetGroupEntryFromTeamCode("USA") ?? new(), GetGroupEntryFromTeamCode("SRB") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("SSD") ?? new(), GetGroupEntryFromTeamCode("PRI") ?? new(), 0, 0, 0);

            Console.WriteLine("II kolo:\n\tGrupa A:");
            GroupStageGame(GetGroupEntryFromTeamCode("AUS") ?? new(), GetGroupEntryFromTeamCode("GRE") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("ESP") ?? new(), GetGroupEntryFromTeamCode("CAN") ?? new(), 0, 0, 0);
            Console.WriteLine("\tGrupa B:");
            GroupStageGame(GetGroupEntryFromTeamCode("FRA") ?? new(), GetGroupEntryFromTeamCode("BRA") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("JPN") ?? new(), GetGroupEntryFromTeamCode("GER") ?? new(), 0, 0, 0);
            Console.WriteLine("\tGrupa C:");
            GroupStageGame(GetGroupEntryFromTeamCode("SRB") ?? new(), GetGroupEntryFromTeamCode("SSD") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("PRI") ?? new(), GetGroupEntryFromTeamCode("USA") ?? new(), 0, 0, 0);

            Console.WriteLine("III kolo:\n\tGrupa A:");
            GroupStageGame(GetGroupEntryFromTeamCode("AUS") ?? new(), GetGroupEntryFromTeamCode("ESP") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("CAN") ?? new(), GetGroupEntryFromTeamCode("GRE") ?? new(), 0, 0, 0);
            Console.WriteLine("\tGrupa B:");
            GroupStageGame(GetGroupEntryFromTeamCode("FRA") ?? new(), GetGroupEntryFromTeamCode("JPN") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("GER") ?? new(), GetGroupEntryFromTeamCode("BRA") ?? new(), 0, 0, 0);
            Console.WriteLine("\tGrupa C:");
            GroupStageGame(GetGroupEntryFromTeamCode("SRB") ?? new(), GetGroupEntryFromTeamCode("PRI") ?? new(), 0, 0, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("USA") ?? new(), GetGroupEntryFromTeamCode("SSD") ?? new(), 0, 0, 0);
             */

            // actual results
            Console.WriteLine("I kolo:\n\tGrupa A:");
            GroupStageGame(GetGroupEntryFromTeamCode("AUS") ?? new(), GetGroupEntryFromTeamCode("ESP") ?? new(), 92, 80, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("GRE") ?? new(), GetGroupEntryFromTeamCode("CAN") ?? new(), 79, 86, 0);
            Console.WriteLine("\tGrupa B:");
            GroupStageGame(GetGroupEntryFromTeamCode("GER") ?? new(), GetGroupEntryFromTeamCode("JPN") ?? new(), 97, 77, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("FRA") ?? new(), GetGroupEntryFromTeamCode("BRA") ?? new(), 78, 66, 0);
            Console.WriteLine("\tGrupa C:");
            GroupStageGame(GetGroupEntryFromTeamCode("SSD") ?? new(), GetGroupEntryFromTeamCode("PRI") ?? new(), 90, 79, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("SRB") ?? new(), GetGroupEntryFromTeamCode("USA") ?? new(), 84, 110, 0);

            Console.WriteLine("II kolo:\n\tGrupa A:");
            GroupStageGame(GetGroupEntryFromTeamCode("ESP") ?? new(), GetGroupEntryFromTeamCode("GRE") ?? new(), 84, 77, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("CAN") ?? new(), GetGroupEntryFromTeamCode("AUS") ?? new(), 93, 83, 0);
            Console.WriteLine("\tGrupa B:");
            GroupStageGame(GetGroupEntryFromTeamCode("JPN") ?? new(), GetGroupEntryFromTeamCode("FRA") ?? new(), 90, 94, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("BRA") ?? new(), GetGroupEntryFromTeamCode("GER") ?? new(), 73, 86, 0);
            Console.WriteLine("\tGrupa C:");
            GroupStageGame(GetGroupEntryFromTeamCode("PRI") ?? new(), GetGroupEntryFromTeamCode("SRB") ?? new(), 66, 107, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("USA") ?? new(), GetGroupEntryFromTeamCode("SSD") ?? new(), 103, 86, 0);

            Console.WriteLine("III kolo:\n\tGrupa A:");
            GroupStageGame(GetGroupEntryFromTeamCode("AUS") ?? new(), GetGroupEntryFromTeamCode("GRE") ?? new(), 71, 77, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("CAN") ?? new(), GetGroupEntryFromTeamCode("ESP") ?? new(), 88, 85, 0);
            Console.WriteLine("\tGrupa B:");
            GroupStageGame(GetGroupEntryFromTeamCode("JPN") ?? new(), GetGroupEntryFromTeamCode("BRA") ?? new(), 84, 102, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("FRA") ?? new(), GetGroupEntryFromTeamCode("GER") ?? new(), 71, 85, 0);
            Console.WriteLine("\tGrupa C:");
            GroupStageGame(GetGroupEntryFromTeamCode("PRI") ?? new(), GetGroupEntryFromTeamCode("USA") ?? new(), 83, 104, 0);
            GroupStageGame(GetGroupEntryFromTeamCode("SRB") ?? new(), GetGroupEntryFromTeamCode("SSD") ?? new(), 96, 85, 0);
        }

        private static (int team1Score, int team2Score, int overtimeCounter) SimulateGame(GroupEntry teamInGroup1, GroupEntry teamInGroup2)
        {
            int overtimeCounter = 0;
            // adaptacija ELO rejtinga funkciji normalne raspodele za formulu ispod, da ELO rejting nema toliko drastičan efekat na razliku u postignutim poenima
            Func<double, double> Formula = (double eloRating) => { return eloRating / 80 + 25; };

            int team1Score = (int)Math.Round(Formula(teamInGroup1.Team.ELORating) * RandomFromNormalDistribution(2.0, 0.15));
            int team2Score = (int)Math.Round(Formula(teamInGroup2.Team.ELORating) * RandomFromNormalDistribution(2.0, 0.15));

            // ako timovi imaju isti broj poena, ide se u produžetke koji se igraju dokle god jedan tim ne bude imao više poena
            while (team1Score == team2Score)
            {
                team1Score += (int)Math.Round(Formula(teamInGroup1.Team.ELORating) / 8 * RandomFromNormalDistribution(2.0, 0.35));
                team2Score += (int)Math.Round(Formula(teamInGroup2.Team.ELORating) / 8 * RandomFromNormalDistribution(2.0, 0.35));
                overtimeCounter++;
            }

            return (team1Score, team2Score, overtimeCounter);
        }


        /// <summary>
        /// Simulira meč grupne faze između 2 ekipe
        /// </summary>
        /// <param name="teamInGroup1">Prvi tim</param>
        /// <param name="teamInGroup2">Drugi tim</param>
        private static void GroupStageGame(GroupEntry teamInGroup1, GroupEntry teamInGroup2)
        {
            (int team1Score, int team2Score, int overtimeCounter) = SimulateGame(teamInGroup1, teamInGroup2);

            GroupStageGame(teamInGroup1, teamInGroup2, team1Score, team2Score, overtimeCounter);

        }

        private static void GroupStageGame(GroupEntry teamInGroup1, GroupEntry teamInGroup2, int team1Score, int team2Score, int overtimeCounter)
        {
            string team1Name = teamInGroup1.Team.Name;
            string team2Name = teamInGroup2.Team.Name;

            string overtimeText;

            switch (overtimeCounter)
            {
                case 0:
                    overtimeText = "";
                    break;
                case 1:
                    overtimeText = " [OT]";
                    break;
                default:
                    overtimeText = $" [{overtimeCounter}OT]";
                    break;

            }

            Console.WriteLine($"\t\t{team1Name} - {team2Name} ({team1Score}:{team2Score}){overtimeText}");

            HeadToHeadStats team1H2H = new HeadToHeadStats();
            HeadToHeadStats team2H2H = new HeadToHeadStats();

            teamInGroup1.HeadToHead.Add(teamInGroup2.Team.ISOCode, team1H2H);
            teamInGroup2.HeadToHead.Add(teamInGroup1.Team.ISOCode, team2H2H);

            teamInGroup1.PointsScored += team1Score;
            teamInGroup1.PointsReceived += team2Score;
            team1H2H.PointsScored = team1Score;
            team1H2H.PointsReceived = team2Score;

            teamInGroup2.PointsScored += team2Score;
            teamInGroup2.PointsReceived += team1Score;
            team2H2H.PointsScored = team2Score;
            team2H2H.PointsReceived = team1Score;

            if (team1Score > team2Score)
            {
                teamInGroup1.Wins++;
                teamInGroup2.Losses++;

                team1H2H.Wins++;
                team2H2H.Losses++;
            }
            else if (team2Score > team1Score)
            {
                teamInGroup2.Wins++;
                teamInGroup1.Losses++;

                team2H2H.Wins++;
                team1H2H.Losses++;
            }

            CalculateEloRating(teamInGroup1.Team, teamInGroup2.Team, team1Score, team2Score, 25);
        }

        private static GroupEntry? GetGroupEntryFromTeam(Team team)
        {
            GroupEntry? teamInGroup;

            foreach (var group in allGroups.Values)
            {
                teamInGroup = group.Where(g => g.Value.Team.Equals(team)).FirstOrDefault().Value;

                if (teamInGroup != null)
                {
                    return teamInGroup;
                }
            }

            return null;
        }

        private static GroupEntry? GetGroupEntryFromTeamCode(string teamCode)
        {
            Team? team = allTeams.Where(t => t.Key == teamCode).FirstOrDefault().Value;

            if (team != null)
            {
                return GetGroupEntryFromTeam(team);
            }
            else
            {
                return null;
            }
            
        }

        



        /// <summary>
        /// Simulira mečeve nokaut faze. Glavna razlika u odnosu na simuliranje mečeva grupne faze je što se 
        /// ne popunjavaju GroupEntry podaci, i parametri su namešteni tako da timovi postižu manje koševa 
        /// s obzirom da se u nokaut fazi igra čvršća odbrana.
        /// </summary>
        /// <param name="game">meč koji sadrži dva tima</param>
        /// <returns>Torka u kojoj je prvi član (Item1) pobednik, a drugi član (Item2) poraženi</returns>
        private static (Team, Team) KnockoutStageGame(Game game)
        {
            Team team1 = game.Team1;
            Team team2 = game.Team2;

            int overtimeCounter = 0;
            string overtimeText = "";

            string team1Name = team1.Name;
            string team2Name = team2.Name;

            Func<double, double> Formula = (double eloRating) => { return eloRating / 80 + 25; };

            int team1Score = (int)Math.Round(Formula(team1.ELORating) * RandomFromNormalDistribution(1.8, 0.12));
            int team2Score = (int)Math.Round(Formula(team2.ELORating) * RandomFromNormalDistribution(1.8, 0.12));

            while (team1Score == team2Score)
            {
                team1Score += (int)Math.Round(Formula(team1.ELORating) / 8 * RandomFromNormalDistribution(1.8, 0.30));
                team2Score += (int)Math.Round(Formula(team2.ELORating) / 8 * RandomFromNormalDistribution(1.8, 0.30));
                overtimeCounter++;
            }

            game.Team1Score = team1Score;
            game.Team2Score = team2Score;

            switch (overtimeCounter)
            {
                case 0:
                    overtimeText = "";
                    break;
                case 1:
                    overtimeText = " [OT]";
                    break;
                default:
                    overtimeText = $" [{overtimeCounter}OT]";
                    break;

            }

            Console.WriteLine($"\t{team1Name} - {team2Name} ({team1Score}:{team2Score}){overtimeText}");

            

            //računanje novog ELO ratinga
            CalculateEloRating(team1, team2, team1Score, team2Score, 15);


            if (team1Score > team2Score)
            {
                return (team1, team2);
            }
            else
            {
                return (team2, team1);
            }

        }

        /// <summary>
        /// Računa ELO rejting na osnovu odigranog meča.
        /// </summary>
        /// <param name="team1">Prvi tim</param>
        /// <param name="team2">Drugi tim</param>
        /// <param name="team1Score">Broj postignutih poena prvog tima</param>
        /// <param name="team2Score">Broj postignutih poena drugog tima</param>
        /// <param name="adjustment">Koliko obično najviše bude razlika između poena dva tima</param>
        private static void CalculateEloRating(Team team1, Team team2, int team1Score, int team2Score, int adjustment)
        {
            double team1Elo = team1.ELORating;
            double team2Elo = team2.ELORating;

            double team1expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (team2Elo - team1Elo) / 400.0));
            double team2expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (team1Elo - team2Elo) / 400.0));

            int team1ActualOutcome = (team1Score > team2Score) ? 1 : 0;
            int team2ActualOutcome = (team2Score > team1Score) ? 1 : 0;

            team1.ELORating = team1Elo + adjustment * (team1ActualOutcome - team1expectedOutcome);
            team2.ELORating = team2Elo + adjustment * (team2ActualOutcome - team2expectedOutcome);


            Debug.WriteLine($"{team1.ISOCode} - {team2.ISOCode} ({team1Score}:{team2Score});\t ELO update: \t{team1.ISOCode}: {(int)team1Elo} -> {(int)team1.ELORating};\t\t{team2.ISOCode}: {(int)team2Elo} -> {(int)team2.ELORating}");

        }


        /// <summary>
        /// Računa nasumična vrednost na osnovu normalne distribucije
        /// </summary>
        /// <param name="mean">matematičko očekivanje</param>
        /// <param name="stdDev">standardna devijansa</param>
        /// <returns>nasumične vrednosti na osnovu normalne distribucije</returns>
        private static double RandomFromNormalDistribution(double mean, double stdDev)
        {
            Random rand = new Random();
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mean + stdDev * randStdNormal;

            return randNormal;
        }
    }
}
