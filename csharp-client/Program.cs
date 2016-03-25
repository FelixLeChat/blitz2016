using System;
using Coveo.Bot;
using Coveo.Core;

namespace Coveo
{
    internal class MyBot
    {
        /**
         * @param args args[0] Private key
         * @param args args[1] [training|arena]
         * @param args args[2] Game Id
         */

        private static void Main(string[] args)
        {
            SimpleBotRunner runner;

            if (args.Length < 2) {
                Console.WriteLine("Usage: myBot.exe key training|arena gameId");
                Console.WriteLine("gameId is optionnal when in training mode");
                Console.ReadKey();
                return;
            }

            // Link to private server - Still need to figure out how to start matches
            const string serverUrl = "http://ec2-52-207-237-66.compute-1.amazonaws.com/";
            var gameId = args.Length == 3 ? args[2] : null;

            runner = new SimpleBotRunner(
                new ApiToolkit(serverUrl, args[0], args[1] == "training", gameId),
                new TestBot());
            
            runner.Run();

            Console.Read();
        }
    }
}