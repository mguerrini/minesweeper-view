using Newtonsoft.Json;
using System;

namespace Minesweeper.Shared
{
    [Serializable]
    public class BoardData {
        [JsonProperty(PropertyName = "row_count")]
        public int RowCount { get; set; }
        [JsonProperty(PropertyName = "col_count")]
        public int ColCount { get; set; }
        [JsonProperty(PropertyName = "mines_count")]
        public int MinesCount { get; set; }
        [JsonProperty(PropertyName = "cells")]
        public CellData[][] Cells { get; set; }

        public BoardData()
        {

        }
	}
}