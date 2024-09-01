using BasketballTournamentTask_cdbhnd.Database;
using BasketballTournamentTask_cdbhnd.Model;
using BasketballTournamentTask_cdbhnd.Model.Domain;
using BasketballTournamentTask_cdbhnd.Model.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace BasketballTournamentTask_cdbhnd
{
    internal class Program
    {
        public static DbContext Dbc { get; set; } = new();

        private static Dictionary<string, Team> allTeams = new();

        private static Dictionary<string, Dictionary<string, GroupEntry>> allGroups = new();

        static void Main(string[] args)
        {
            Dbc = new DbContext();

            //TODO: AnalyzeTeams()

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

            foreach (string teamCode in allTeams.Keys)
            {
                ExhibitionGamesToElo(teamCode, 30);
            }

            //TODO: SimulateGroupStage()

            Dictionary<string, List<(int, int)>> roundsSchedule = new Dictionary<string, List<(int, int)>>
            {
                { "I kolo", [(0, 1),(2, 3)] },
                { "II kolo", [(1, 2),(3, 0)] },
                { "III kolo", [(1, 3),(0, 2)] }
            };

            Console.WriteLine("  ***************************************************");
            Console.WriteLine(" *    OLYMPICS BASKETBALL TOURNAMENT SIMULATOR     *");
            Console.WriteLine("**************************************************");
            Console.WriteLine("      .,::OOO::,.     .,ooOOOoo,.     .,::OOO::,.\r\n    .:'         `:. .8'         `8. .:'         `:.\r\n    :\"           \": 8\"           \"8 :\"           \":\r\n    :,        .,:::\"\"::,.     .,:o8OO::,.        ,:__  o\\\r\n     :,,    .:' ,:   8oo`:. .:'oo8   :,,`:.    ,,:  W    \\O\r\n      `^OOoo:\"O^'     `^88oo:\"8^'     `^O\":ooOO^'         |\\_\r\n            :,           ,: :,           ,:              /-\\\r\n             :,,       ,,:   :,,       ,,:               \\   \\\r\n              `^Oo,,,oO^'     `^OOoooOO^'");
            Console.WriteLine("\n");
            Console.WriteLine("                   - GROUP STAGE -");

            // 
            foreach (var round in roundsSchedule)
            {
                Console.WriteLine($"{round.Key}:");

                foreach (var group in allGroups)
                {
                    string groupLetter = group.Key;

                    Console.WriteLine($"\tGrupa {groupLetter}:");

                    foreach ((int, int) pair in round.Value)
                    {
                        GroupEntry ge1 = group.Value.ElementAt(pair.Item1).Value;
                        GroupEntry ge2 = group.Value.ElementAt(pair.Item2).Value;

                        GroupStageGame(ge1, ge2);

                    }
                }
                Console.WriteLine();
            }

            SortedDictionary<string, List<GroupEntry>> allGroupsList = new();

            Console.WriteLine("\n\n                    - GROUPS -");

            foreach (var group in allGroups)
            {
                Console.WriteLine($"\nGrupa {group.Key}:\t\t|Pts| W | L | FOR | AGT | -/+");
                Console.WriteLine("------------------------+---+---+---+-----+-----+-----");

                List<GroupEntry> groupList = allGroups[group.Key].Values.ToList();
                List<IGrouping<int, GroupEntry>> threeTeamTie = groupList.GroupBy(x => x.Points).Where(x => x.Count() > 2).ToList();

                if (threeTeamTie.Count == 1 && (threeTeamTie.FirstOrDefault()).Count() == 3)
                {
                    threeTeamTie.FirstOrDefault().All(x => x.IsInThreeTeamTie = true);

                    int threeTeamTiePointsAmount = threeTeamTie.FirstOrDefault().FirstOrDefault().Points;

                    List<GroupEntry> tttMembersList = groupList.Where(x => x.Points == threeTeamTie.FirstOrDefault().FirstOrDefault().Points).ToList();

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


                groupList.Sort(new GroupTeamComparer());

                allGroupsList.Add(group.Key, groupList);
                

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

            List<GroupEntry> bigTable = new List<GroupEntry>();

            for (int i = 0; i < 3; i++)
            {
                List<GroupEntry> sameRankSubTable = new List<GroupEntry>();

                foreach (List<GroupEntry> group in allGroupsList.Values)
                {
                    sameRankSubTable.Add(group[i]);
                }

                sameRankSubTable.Sort(new CrossGroupTeamComparer());
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
                
                if (i == bigTable.Count - 1)
                {
                    Console.WriteLine("------------------------+---+---+---+-----+-----+-----");
                }

                Console.WriteLine($"{i + 1}. {bigTable[i].Team.Name}{tabSeparator}| {bigTable[i].Points} | {bigTable[i].Wins} | {bigTable[i].Losses} | {bigTable[i].PointsScored} | {bigTable[i].PointsReceived} | {preSpaces}{prefix}{Math.Abs(bigTable[i].PointsDifference)}");
            }

            Console.WriteLine("\n\n                      - DRAW -");
            //TODO: SimulateDraw()

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
                List<GroupEntry> topHat;
                List<GroupEntry> bottomHat;

                List<Game> potentialPairs;

                string top1GroupLetter, bottom1GroupLetter, top2GroupLetter, bottom2GroupLetter;

                do
                {
                    topHat = new List<GroupEntry>(hats[i]);
                    bottomHat = new List<GroupEntry>(hats[hats.Count - i - 1]);
                    
                    potentialPairs = new List<Game>();
                    
                    Random rand = new Random();

                    int randomIndexTop = rand.Next(0, 2);
                    int randomIndexBottom = rand.Next(0, 2);

                    Team top1 = topHat[randomIndexTop].Team;
                    top1GroupLetter = topHat[randomIndexTop].Group;

                    Team bottom1 = bottomHat[randomIndexBottom].Team;
                    bottom1GroupLetter = bottomHat[randomIndexBottom].Group;

                    Game game1 = new Game(top1, bottom1);
                    potentialPairs.Add(game1);

                    topHat.RemoveAt(randomIndexTop);
                    bottomHat.RemoveAt(randomIndexBottom);

                    Team top2 = topHat[0].Team;
                    top2GroupLetter = topHat[0].Group;

                    Team bottom2 = bottomHat[0].Team;
                    bottom2GroupLetter = bottomHat[0].Group;

                    Game game2 = new Game(top2, bottom2);
                    potentialPairs.Add(game2);

                } while (top1GroupLetter == bottom1GroupLetter || top2GroupLetter == bottom2GroupLetter);

                quarterfinalGames.AddRange(potentialPairs);
            }

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


            //TODO: SimulateKnockoutStage()
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

            Console.WriteLine($"\n\n              {goldTeam.ISOCode}\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⡿⠿⠿⢿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣶⠀⢸⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⠿⠀⠸⢿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⠀⠀⠀{silverTeam.ISOCode}⠀⠀⠀⢸⣿⣿⣿⣶⣶⣶⣾⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⣤⣤⣤⣤⣤⣤⣤⣤⣤⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀\r\n⠀⣿⣿⣟⢉⣉⠉⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀{bronzeTeam.ISOCode}⠀⠀⠀⠀\r\n⠀⣿⣿⣿⣿⠟⢀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣶⣶⣶⣶⣶⣶⣶⣶⠀\r\n⠀⣿⣿⣟⠁⠐⠛⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣉⣉⡉⢹⣿⣿⠀\r\n⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣉⡀⢸⣿⣿⠀\r\n⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣉⣉⣁⣼⣿⣿⠀\r\n⠀⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠀");



        }



        private static double FibaRankingToElo(int fibaRanking)
        {
            return 4200 / (fibaRanking + 4) + 950;
        }

        private static void ExhibitionGamesToElo(string teamCode, int adjustment)
        {
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

        private static void GroupStageGame(GroupEntry team1, GroupEntry team2)
        {
            int overtimeCounter = 0;
            string overtimeText = "";

            string team1Name = team1.Team.Name;
            string team2Name = team2.Team.Name;

            Func<double, double> Formula = (double eloRating) => { return eloRating / 80 + 25; };

            int team1Score = (int)(Math.Round(Formula(team1.Team.ELORating) * RandomFromNormalDistribution(2.0, 0.15)));
            int team2Score = (int)(Math.Round(Formula(team2.Team.ELORating) * RandomFromNormalDistribution(2.0, 0.15)));

            while (team1Score == team2Score)
            {
                team1Score += (int)(Math.Round(Formula(team1.Team.ELORating) / 8 * RandomFromNormalDistribution(2.0, 0.35)));
                team2Score += (int)(Math.Round(Formula(team2.Team.ELORating) / 8 * RandomFromNormalDistribution(2.0, 0.35)));
                overtimeCounter++;
            }

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

            team1.HeadToHead.Add(team2.Team.ISOCode, team1H2H);
            team2.HeadToHead.Add(team1.Team.ISOCode, team2H2H);

            team1.PointsScored += team1Score;
            team1.PointsReceived += team2Score;
            team1H2H.PointsScored = team1Score;
            team1H2H.PointsReceived = team2Score;

            team2.PointsScored += team2Score;
            team2.PointsReceived += team1Score;
            team2H2H.PointsScored = team2Score;
            team2H2H.PointsReceived = team1Score;

            if (team1Score > team2Score)
            {
                team1.Wins++;
                team2.Losses++;

                team1H2H.Wins++;
                team2H2H.Losses++;
            }
            else if (team2Score > team1Score)
            {
                team2.Wins++;
                team1.Losses++;

                team2H2H.Wins++;
                team1H2H.Losses++;
            }

            double team1Elo = team1.Team.ELORating;
            double team2Elo = team2.Team.ELORating;

            double team1expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (team2Elo - team1Elo) / 400.0));
            double team2expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (team1Elo - team2Elo) / 400.0));

            int team1ActualOutcome = (team1Score > team2Score) ? 1 : 0;
            int team2ActualOutcome = (team2Score > team1Score) ? 1 : 0;

            int adjustment = 40;

            team1Elo = team1Elo + adjustment * (team1ActualOutcome - team1expectedOutcome);
            team2Elo = team2Elo + adjustment * (team2ActualOutcome - team2expectedOutcome);

            team1.Team.ELORating = team1Elo;
            team2.Team.ELORating = team2Elo;

        }


        private static (Team, Team) KnockoutStageGame(Game game)
        {
            Team team1 = game.Team1;
            Team team2 = game.Team2;

            int overtimeCounter = 0;
            string overtimeText = "";

            string team1Name = team1.Name;
            string team2Name = team2.Name;

            Func<double, double> Formula = (double eloRating) => { return eloRating / 80 + 25; };

            int team1Score = (int)(Math.Round(Formula(team1.ELORating) * RandomFromNormalDistribution(1.8, 0.12)));
            int team2Score = (int)(Math.Round(Formula(team2.ELORating) * RandomFromNormalDistribution(1.8, 0.12)));

            while (team1Score == team2Score)
            {
                team1Score += (int)(Math.Round(Formula(team1.ELORating) / 8 * RandomFromNormalDistribution(1.8, 0.30)));
                team2Score += (int)(Math.Round(Formula(team2.ELORating) / 8 * RandomFromNormalDistribution(1.8, 0.30)));
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

            double team1Elo = team1.ELORating;
            double team2Elo = team2.ELORating;

            double team1expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (team2Elo - team1Elo) / 400.0));
            double team2expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (team1Elo - team2Elo) / 400.0));

            int team1ActualOutcome = (team1Score > team2Score) ? 1 : 0;
            int team2ActualOutcome = (team2Score > team1Score) ? 1 : 0;

            int adjustment = 20;

            team1Elo = team1Elo + adjustment * (team1ActualOutcome - team1expectedOutcome);
            team2Elo = team2Elo + adjustment * (team2ActualOutcome - team2expectedOutcome);

            team1.ELORating = team1Elo;
            team2.ELORating = team2Elo;

            if (team1Score > team2Score)
            {
                return (team1, team2);
            }
            else
            {
                return (team2, team1);
            }

        }


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
