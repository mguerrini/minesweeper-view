using Minesweeper.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Services
{
    public interface IMinesweeperServices
    {
        GameData GetGame(string userId, string gameId);
        IList<GameData> GetGameListByUserId(string userId);

        GameData NewGame(string userId, int rowCount, int colCount, int mines);
        GameData RevealCell(string userId, string gameId, int row, int col);
        GameData MarkCell(string userId, string gameId, int row, int col, CellMarkType mark);

        void DeleteGame(string userId, string gameId);
        void DeleteGamesByUser(string userId);
    }
}
