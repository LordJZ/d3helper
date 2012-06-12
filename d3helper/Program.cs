using System;
using d3helper.Bots;
using d3helper.Classes;

namespace d3helper
{
    class Program
    {
        static Bot Bot;
        static Bot Stats;

        static void Main(string[] args)
        {
            Logger.Init();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            //Bot = new RoyalCryptsBot();
            //Bot.Start();

            Stats = new StatsBot();
            Stats.Start();

            Bot = new MiraEamonBot();
            Bot.SetPlayer(new DemonHunter());
            Bot.Start();
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Logger.Close();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Add(e.ExceptionObject.ToString());
        }
    }
}
