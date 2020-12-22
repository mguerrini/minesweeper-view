using System;

namespace Minesweeper.Shared
{
    [Serializable]
    public enum CellType : int
    {
        CellType_Mine,
        CellType_Number,
        CellType_Empty,
        CellType_Unknown
    }
}