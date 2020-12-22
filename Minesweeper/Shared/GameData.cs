using Newtonsoft.Json;
using System;

namespace Minesweeper.Shared
{
    [Serializable]
	public class GameData {
        [JsonProperty(PropertyName = "id")]
        public string Id;
        [JsonProperty(PropertyName = "start_time")]
        public DateTime StartTime;
        [JsonProperty(PropertyName = "fisnish_time")]
        public DateTime FinishTime;
        [JsonProperty(PropertyName = "status")]
        public GameStatusType Status;
        [JsonProperty(PropertyName = "board")]
        public BoardData Board;

		public GameData(){

		}
	}
}