using System;

namespace Minesweeper.Shared
{
    [Serializable]
    public enum CellMarkType : int
    {
        CellMarkType_None,
        CellMarkType_Question,
        CellMarkType_Flag
    }
}