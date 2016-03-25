using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Coveo.Core
{
    public class ApiToolkit
    {
        public const string TrainingUrl = "/api/training";
        public const string ArenaUrl = "/api/arena";

        private readonly string _uri;
        private readonly string _serverUrl;

        public string PlayUrl { get; private set; }
        public string ViewUrl { get; private set; }
        public string BotKey { get; }

        public GameState GameState { get; private set; }
        public bool Errored { get; set; }

        public ApiToolkit(string serverUrl,
            string key,
            bool trainingMode,
            string gameId,
            uint turns = 4000,
            string map = null)
        {
            BotKey = key;
            _uri = serverUrl + (trainingMode ? TrainingUrl : ArenaUrl);
            _uri += "?key=" + key;
            if (trainingMode) {
                _uri += "&turns=" + turns;
                if (map != null) {
                    _uri += "&map=" + map;
                }
            } else {
                _uri += "&gameId=" + gameId;
            }

            Errored = false;
        }

        //initializes a new game, its syncronised
        public void CreateGame()
        {
            WebRequest client = WebRequest.CreateHttp(_uri);
            client.Method = "POST";
            client.ContentType = "application/x-www-form-urlencoded";
            client.Timeout = 1000*60*60; // Because we don't want to timeout

            try {
                var result = new StreamReader(client.GetResponse().GetResponseStream()).ReadToEnd();
                GameState = Deserialize(result);
            } catch (WebException exception) {
                using (var reader = new StreamReader(exception.Response.GetResponseStream())) {
                    Errored = true;
                    Console.WriteLine(exception.Message);
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
        }

        private GameState Deserialize(string json)
        {
            var response = JsonConvert.DeserializeObject<GameResponse>(json);

            /*var byteArray = Encoding.UTF8.GetBytes(json);
            var stream = new MemoryStream(byteArray);

            var ser = new DataContractJsonSerializer(typeof(GameResponse));
            var gameResponse = (GameResponse) ser.ReadObject(stream);*/

            PlayUrl = response.PlayUrl;
            ViewUrl = response.ViewUrl;

            return new GameState() {
                MyHero = response.Hero,
                Heroes = response.Game.Heroes,
                CurrentTurn = response.Game.Turn,
                MaxTurns = response.Game.MaxTurns,
                Finished = response.Game.Finished,
                Board = CreateBoard(response.Game.Board.Size, response.Game.Board.Tiles)
            };
        }

        public void MoveHero(string direction)
        {
            var myParameters = "key=" + BotKey + "&dir=" + direction;

            using (var client = new WebClient()) {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                try {
                    var result = client.UploadString(PlayUrl, myParameters);
                    GameState = Deserialize(result);
                } catch (WebException exception) {
                    using (var reader = new StreamReader(exception.Response.GetResponseStream())) {
                        Errored = true;
                        Console.WriteLine(exception.Message);
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
            }
        }

        private Tile[][] CreateBoard(int size,
            string data)
        {
            var board = new Tile[size][];

            for (var i = 0; i < size; i++) {
                board[i] = new Tile[size];
            }

            int x = 0, y = 0;
            var charData = data.ToCharArray();

            for (var i = 0; i < charData.Length; i += 2) {
                switch (charData[i]) {
                    case '^':
                        board[x][y] = Tile.Spikes;
                        break;

                    case '#':
                        board[x][y] = Tile.ImpassableWood;
                        break;

                    case ' ':
                        board[x][y] = Tile.Free;
                        break;

                    case '@':
                        switch (charData[i + 1]) {
                            case '1':
                                board[x][y] = Tile.Hero1;
                                break;

                            case '2':
                                board[x][y] = Tile.Hero2;
                                break;

                            case '3':
                                board[x][y] = Tile.Hero3;
                                break;

                            case '4':
                                board[x][y] = Tile.Hero4;
                                break;
                        }
                        break;

                    case '[':
                        board[x][y] = Tile.Tavern;
                        break;

                    case '$':
                        switch (charData[i + 1]) {
                            case '-':
                                board[x][y] = Tile.GoldMineNeutral;
                                break;

                            case '1':
                                board[x][y] = Tile.GoldMine1;
                                break;

                            case '2':
                                board[x][y] = Tile.GoldMine2;
                                break;

                            case '3':
                                board[x][y] = Tile.GoldMine3;
                                break;

                            case '4':
                                board[x][y] = Tile.GoldMine4;
                                break;
                        }
                        break;
                }

                x++;
                if (x == size) {
                    x = 0;
                    y++;
                }
            }

            return board;
        }
    }
}