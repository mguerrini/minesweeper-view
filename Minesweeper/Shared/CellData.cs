using Newtonsoft.Json;
using System;

namespace Minesweeper.Shared
{
    [Serializable]
    public class CellData {

        [JsonProperty(PropertyName = "type")]
        public CellType Type;
        [JsonProperty(PropertyName = "row")]
        public int Row;
        [JsonProperty(PropertyName = "col")]
        public int Col;
        [JsonProperty(PropertyName = "is_revealed")]
        public bool IsRevealed;
        [JsonProperty(PropertyName = "mark")]
        public CellMarkType Mark;
        [JsonProperty(PropertyName = "number")]
        public int Number;

		public CellData(){

		}
	}
}