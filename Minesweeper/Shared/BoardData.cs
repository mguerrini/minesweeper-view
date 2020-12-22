using Newtonsoft.Json;
using System;

namespace Minesweeper.Shared
{
    [Serializable]
    public class BoardData {
        [JsonProperty(PropertyName = "row_count")]
        public int RowCount;
        [JsonProperty(PropertyName = "col_count")]
        public int ColCount;
        [JsonProperty(PropertyName = "mines_count")]
        public int MinesCount;
        [JsonProperty(PropertyName = "cells")]
        public CellData[][] Cells;

        public BoardData()
        {

        }
	}
}