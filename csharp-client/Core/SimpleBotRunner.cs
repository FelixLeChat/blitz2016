using System;
using System.Diagnostics;
using System.Threading;

namespace Coveo.Core
{
    /// <summary>
    /// SimpleBotRunner
    ///
    /// Runs a ISimpleBot with error handling
    /// </summary>
    public class SimpleBotRunner
    {
        private readonly ISimpleBot _simpleBot;

        private readonly ApiToolkit _api;

        private readonly bool _showGame;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="api">The ApiToolit to use</param>
        /// <param name="simpleBot">The ISimplebot to run</param>
        /// <param name="showGame">Wetherwe want to open a game view</param>
        public SimpleBotRunner(ApiToolkit api, ISimpleBot simpleBot, bool showGame = true)
        {
            _simpleBot = simpleBot;
            _api = api;
            _showGame = showGame;
        }

        /// <summary>
        /// Starts the game and runs the bot
        /// </summary>
        public void Run()
        {
            // Bot's setup
            _simpleBot.Setup();

            // Connecting to the game
            _api.CreateGame();

            if (_api.Errored == false)
            {
                // Opens up a game view
                if (_showGame) {
                    new Thread(delegate() {
                        Process.Start(_api.ViewUrl);
                    }).Start();
                }

                // While the game is running, we ask the bot for his next move and
                // we send that to the server
                while (_api.GameState.Finished == false && _api.Errored == false)
                {
                    try
                    {
                        _api.MoveHero(_simpleBot.Move(_api.GameState));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            // Bot's shutdown step
            _simpleBot.Shutdown();
        }
    }
}