using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Shared;

namespace Minesweeper.Services
{
    public class MinesweeperServicesMock : IMinesweeperServices
    {
        private GameData ActiveGame { get; set; }

        private List<GameData> Games { get; set; }

        public void DeleteGame(string userId, string gameId)
        {
        }

        public void DeleteGamesByUser(string userId)
        {
        }

        public GameData GetGame(string userId, string gameId)
        {
            return this.ActiveGame;
        }

        public IList<GameData> GetGameListByUserId(string userId)
        {
            return this.Games;
        }

        public GameData MarkCell(string userId, string gameId, int row, int col, CellMarkType mark)
        {
            return this.ActiveGame;
        }

        public GameData NewGame(string userId, int rowCount, int colCount, int mines)
        {
            string text = File.ReadAllText(@"C:\LocalData\Personales\ChallengesLaborales\Deviget\Source\Client\Minesweeper\Minesweeper\_Resources\Game.txt");
            GameData data = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(text);

            this.ActiveGame = data;
            this.Games = new List<GameData>();
            this.Games.Add(data);
            this.Games.Add(data);
            this.Games.Add(data);
            this.Games.Add(data);

            return data;
        }

        public GameData RevealCell(string userId, string gameId, int row, int col)
        {
            return this.ActiveGame;
        }
    }
}
