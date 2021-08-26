﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using NPCConsoleTesting.Models;
using Microsoft.Extensions.Configuration;
using NPCConsoleTesting.DB_Connection;
using System.IO;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace NPCConsoleTesting
{
    class Program
    {
        static void Main()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("App Starting");

            var host = Host.CreateDefaultBuilder()
                //.ConfigureAppConfiguration(app =>
                //{
                //    app.AddJsonFile("appsettings.json");
                //})
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IGreetingService, GreetingService>();
                    services.AddTransient<Helper>();
                })
                .UseSerilog()
                .Build();

            //var svc = ActivatorUtilities.CreateInstance<GreetingService>(host.Services);
            //svc.Run();

            var svc2 = ActivatorUtilities.CreateInstance<Helper>(host.Services);
            string test = svc2.GetConnectionString();
            
            DBConnection();

            void DBConnection()
            {
                using (SqlConnection connection = new SqlConnection(test))
                {
                    connection.Open();

                    string sql = @"SELECT * FROM npcs WHERE id = 58";

                    var query = connection.Query<CharacterModel>(sql);
                    Console.WriteLine("test");
                    Console.ReadLine();
                }
            }






            Console.WriteLine($"How many are battling?");
            int numberBattling = int.Parse(Console.ReadLine());
            Console.WriteLine($"1 = Random, 2 = Custom");
            int randomOrCustom = int.Parse(Console.ReadLine());

            Build build = new();
            List<ICombatant> combatants = new();

            for (int i = 0; i < numberBattling; i++)
            {
                combatants.Add(randomOrCustom == 2 ? Build.BuildCombatantViaConsole() : build.BuildCombatantRandomly());
            }

            //do a round
            //RoundResults roundResults = Combat.CombatRound(combatants);
            //roundResults.roundLog.ForEach(i => Console.WriteLine(i));
            //Console.ReadLine();

            //do a whole fight
            List<string> wholeFightLog = new() {" ", "Here's what happened:"};
            bool downToOne = false;
            int roundNumber = 0;

            while (!downToOne)
            {
                RoundResults roundResults = CombatRound.DoACombatRound(combatants);

                roundNumber++;
                wholeFightLog.Add($"------Round {roundNumber}------");

                //TODO: ensure there is not a shorter way to do this. No luck briefly with Join, Concat
                //add roundLog to wholeFightLog
                foreach (string log in roundResults.roundLog)
                {
                    wholeFightLog.Add(log);
                }

                //TODO: clean this up, likely using LINQ
                //check if we're down to one
                int numberOfSurvivors = 0;
                foreach (ICombatant ch in roundResults.combatants)
                {
                    if (ch.HP > 0)
                    {
                        numberOfSurvivors++;
                    }
                }
                if (numberOfSurvivors == 1)
                {
                    //the fight has ended
                    downToOne = true;

                    List<string> winner = combatants.Where(x => x.HP > 0).Select(x => x.Name).ToList();
                    wholeFightLog.Add($"{winner[0]} won.");

                    wholeFightLog.ForEach(i => Console.WriteLine(i));
                    Console.ReadLine();
                }

                //lol
                if (numberOfSurvivors < 1)
                {
                    Console.WriteLine("lol");
                    break;
                }

                //update combatants list with returned
                combatants = roundResults.combatants;
            }
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("")}")
                .AddEnvironmentVariables();
        }
    }
}
