using Newtonsoft.Json;
using System;

namespace Minesweeper.Shared
{
    [Serializable]
	public class GameData {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "start_time")]
        public DateTime StartTime { get; set; }
        [JsonProperty(PropertyName = "finish_time")]
        public DateTime FinishTime { get; set; }
        [JsonProperty(PropertyName = "status")]
        public GameStatusType Status { get; set; }
        [JsonProperty(PropertyName = "board")]
        public BoardData Board { get; set; }

        public GameData(){

		}
	}
}