﻿using BasketballTournamentTask_cdbhnd.Database;
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
        struct Game
        {
            public Team Team1 { get; set; }
            public Team Team2 { get; set; }
            public int? Team1Score { get; set; }
            public int? Team2Score { get; set; }

            public Game(Team team1, Team team2)
            {
                Team1 = team1;
                Team2 = team2;
            }

            public Game(Team team1, Team team2, int team1Score, int team2Score)
            {
                Team1 = team1;
                Team2 = team2;
                Team1Score = team1Score;
                Team2Score = team2Score;
            }
        }

        public static DbContext Dbc { get; set; } = new();

        private static Dictionary<string, Team> allTeams = new();

        static void Main(string[] args)
        {
            Dbc = new DbContext();

            //TODO: AnalyzeTeams()

            foreach (var groupDto in Dbc.GroupsDto)
            {
                foreach(TeamDto teamDto in groupDto.Value)
                {
                    Team team = new(teamDto.ISOCode, teamDto.Team, teamDto.FIBARanking, groupDto.Key);
                    allTeams.Add(teamDto.ISOCode, team);
                }
            }

            foreach (string teamCode in allTeams.Keys)
            {
                ExhibitionGamesToElo(teamCode, 30);
            }

            //TODO: SimulateGroupStage()

            SortedDictionary<string, List<(int, int)>> groupStages = new SortedDictionary<string, List<(int, int)>>
            {
                { "I kolo", [(0, 1),(2, 3)] },
                { "II kolo", [(1, 2),(3, 0)] },
                { "III kolo", [(1, 3),(0, 2)] }
            };
            
            foreach (var groupStage in groupStages)
            {
                Console.WriteLine($"Grupna faza - {groupStage.Key}:");

                foreach (var group in Dbc.GroupsDto)
                {
                    Console.WriteLine($"\tGrupa {group.Key}:");

                    foreach ((int, int) pair in groupStage.Value)
                    {
                        string team1Code = group.Value[pair.Item1].ISOCode;
                        string team2Code = group.Value[pair.Item2].ISOCode;

                        Team team1 = allTeams[team1Code];
                        Team team2 = allTeams[team2Code];

                        GroupStageGame(team1, team2);

                    }
                }
                Console.WriteLine();
            }

            SortedDictionary<string, List<Team>> groups = new();

            Console.WriteLine("\nKonačan plasman u grupama:\nW - pobede\nL - porazi\nPts - bodovi\nFOR - postignuti koševi\nAGT - primljeni koševi\n-/+ - koš razlika");
            foreach (var group in Dbc.GroupsDto)
            {
                Console.WriteLine($"\nGrupa {group.Key}:\t\t|Pts| W | L | FOR | AGT | -/+");
                Console.WriteLine("------------------------+---+---+---+-----+-----+-----");

                string[] teamCodesInGroup = allTeams.Values.Where(p => p.Group == group.Key).Select(x => x.ISOCode).ToArray();

                List<Team> teamsInGroup = allTeams.Values.Where(p => p.Group == group.Key).ToList();

                teamsInGroup.Sort(new GroupTeamComparer());

                groups.Add(group.Key, teamsInGroup);
                

                for (int i = 0; i < teamsInGroup.Count; i++)
                {
                    string preSpaces = Math.Abs(teamsInGroup[i].PointsDifference).ToString().Length == 1 ? "  " : Math.Abs(teamsInGroup[i].PointsDifference).ToString().Length == 2 ? " " : "";
                    char prefix = teamsInGroup[i].PointsDifference < 0 ? '-' : '+';
                    string tabSeparator = teamsInGroup[i].Name.Length > 16 ? "\t" : "\t\t";

                    Console.WriteLine($"{i + 1}. {teamsInGroup[i].Name}{tabSeparator}| {teamsInGroup[i].Points} | {teamsInGroup[i].Wins} | {teamsInGroup[i].Losses} | {teamsInGroup[i].PointsScored} | {teamsInGroup[i].PointsReceived} | {preSpaces}{prefix}{Math.Abs(teamsInGroup[i].PointsDifference)}");
                }
                Console.WriteLine("------------------------+---+---+---+-----+-----+-----");
            }

            List<Team> bigTable = new List<Team>();

            for (int i = 0; i < 3; i++)
            {
                List<Team> sameRankSubTable = new List<Team>();

                foreach (var group in groups.Values)
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
                string tabSeparator = bigTable[i].Name.Length > 16 ? "\t" : "\t\t";
                
                if (i == bigTable.Count - 1)
                {
                    Console.WriteLine("------------------------+---+---+---+-----+-----+-----");
                }

                Console.WriteLine($"{i + 1}. {bigTable[i].Name}{tabSeparator}| {bigTable[i].Points} | {bigTable[i].Wins} | {bigTable[i].Losses} | {bigTable[i].PointsScored} | {bigTable[i].PointsReceived} | {preSpaces}{prefix}{Math.Abs(bigTable[i].PointsDifference)}");
            }

            //TODO: SimulateDraw()

            List<List<Team>> hats = new List<List<Team>>();

            for (int i = 0; i < bigTable.Count - 1; i++)
            {

                int hatNo = i / 2;
                int placeInHat = i % 2;
                
                if (placeInHat == 0)
                {
                    hats.Add(new List<Team>());
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
                    Console.WriteLine($"\t\t{team.Name} ({team.Group})");
                }
            }

            List<Game> quarterfinalGames = new List<Game>();

            for (int i = 0; i < hats.Count / 2; i++)
            {
                List<Team> topHat;
                List<Team> bottomHat;

                List<Game> potentialPairs;

                do
                {
                    topHat = new List<Team>(hats[i]);
                    bottomHat = new List<Team>(hats[hats.Count - i - 1]);
                    
                    potentialPairs = new List<Game>();
                    
                    Random rand = new Random();

                    int randomIndexTop = rand.Next(0, 2);
                    int randomIndexBottom = rand.Next(0, 2);

                    Team top1 = topHat[randomIndexTop];
                    Team bottom1 = bottomHat[randomIndexBottom];
                    Game game1 = new Game(top1, bottom1);
                    potentialPairs.Add(game1);

                    topHat.RemoveAt(randomIndexTop);
                    bottomHat.RemoveAt(randomIndexBottom);

                    Team top2 = topHat[0];
                    Team bottom2 = bottomHat[0];
                    Game game2 = new Game(top2, bottom2);
                    potentialPairs.Add(game2);

                } while (potentialPairs[0].Team1.Group == potentialPairs[0].Team2.Group || potentialPairs[1].Team1.Group == potentialPairs[1].Team2.Group);

                quarterfinalGames.AddRange(potentialPairs);
            }

            Console.WriteLine("\nEliminaciona faza:");


            for (int i = 0; i < quarterfinalGames.Count; i++)
            {
                Console.WriteLine($"\t{quarterfinalGames[i].Team1.Name} - {quarterfinalGames[i].Team2.Name}");

                if (i != 0 && i % 2 == 1)
                {
                    Console.WriteLine();
                }
            }

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
            Console.WriteLine($"\t1. {goldTeam.Name}");
            Console.WriteLine($"\t2. {silverTeam.Name}");
            Console.WriteLine($"\t3. {bronzeTeam.Name}");



            Environment.Exit(0);
            
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

        private static void GroupStageGame(Team team1, Team team2)
        {
            int overtimeCounter = 0;
            string overtimeText = "";

            string team1Name = team1.Name;
            string team2Name = team2.Name;

            Func<double, double> Formula = (double eloRating) => { return eloRating / 80 + 25; };

            int team1Score = (int)(Math.Round(Formula(team1.ELORating) * RandomFromNormalDistribution(2.0, 0.15)));
            int team2Score = (int)(Math.Round(Formula(team2.ELORating) * RandomFromNormalDistribution(2.0, 0.15)));

            while (team1Score == team2Score)
            {
                team1Score += (int)(Math.Round(Formula(team1.ELORating) / 8 * RandomFromNormalDistribution(2.0, 0.35)));
                team2Score += (int)(Math.Round(Formula(team2.ELORating) / 8 * RandomFromNormalDistribution(2.0, 0.35)));
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
            team1.HeadToHead.Add(team2.ISOCode, team1H2H);

            HeadToHeadStats team2H2H = new HeadToHeadStats();
            team2.HeadToHead.Add(team1.ISOCode, team2H2H);

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

            double team1Elo = team1.ELORating;
            double team2Elo = team2.ELORating;

            double team1expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (team2Elo - team1Elo) / 400.0));
            double team2expectedOutcome = 1.0 / (1.0 + Math.Pow(10.0, (team1Elo - team2Elo) / 400.0));

            int team1ActualOutcome = (team1Score > team2Score) ? 1 : 0;
            int team2ActualOutcome = (team2Score > team1Score) ? 1 : 0;

            int adjustment = 40;

            team1Elo = team1Elo + adjustment * (team1ActualOutcome - team1expectedOutcome);
            team2Elo = team2Elo + adjustment * (team2ActualOutcome - team2expectedOutcome);

            team1.ELORating = team1Elo;
            team2.ELORating = team2Elo;

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
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }

        private static Dictionary<string, int> SumRows(Dictionary<string, Dictionary<string, int>> dictionary)
        {
            Dictionary<string, int> summedRows = new Dictionary<string, int>();

            foreach (var outerItem in dictionary)
            {
                int sum = 0;

                foreach (var innerItem in outerItem.Value)
                {
                    sum += innerItem.Value;
                }

                summedRows.Add(outerItem.Key, sum);
            }

            return summedRows;
        }

        static Dictionary<TKey, Dictionary<TSubKey, TValue>> DeepCopy<TKey, TSubKey, TValue>(Dictionary<TKey, Dictionary<TSubKey, TValue>> original)
        {
            var copy = new Dictionary<TKey, Dictionary<TSubKey, TValue>>();

            foreach (var kvp in original)
            {
                // For each key-value pair in the outer dictionary, create a new inner dictionary
                var innerCopy = new Dictionary<TSubKey, TValue>(kvp.Value);
                copy[kvp.Key] = innerCopy;
            }

            return copy;
        }

        private static Dictionary<string, Dictionary<string, int>> GetSubDictionary(Dictionary<string, Dictionary<string, int>> dictionary, string[] keys, bool keepKeys = true)
        {
            var subDictionary = DeepCopy(dictionary);

            foreach (var outerItem in subDictionary)
            {
                if (keys.Contains(outerItem.Key) && !keepKeys || !keys.Contains(outerItem.Key) && keepKeys)
                {
                    subDictionary.Remove(outerItem.Key);
                    continue;
                }

                foreach (var innerItem in outerItem.Value)
                {
                    if (keys.Contains(innerItem.Key) && !keepKeys || !keys.Contains(innerItem.Key) && keepKeys)
                    {
                        outerItem.Value.Remove(innerItem.Key);
                    }
                }

            }

            return subDictionary;
        }

        private static List<Team> TeamCodesToTeams(List<string> teamCodesSorted)
        {
            List<Team> teams = new List<Team>();

            foreach (var teamCode in teamCodesSorted)
            {
                teams.Add(allTeams.FirstOrDefault(x => x.Key == teamCode).Value);
            }

            return teams;
        }
    }
}
