using Newtonsoft.Json;
using System;

namespace Minesweeper.Shared
{
    [Serializable]
    public enum GameStatusType : int {
        GameStatus_Created,
        GameStatus_Playing,
        GameStatus_Lost,
        GameStatus_Won
    }
}