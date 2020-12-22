using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Services
{
    [Serializable]
    public class NewGameRequest
    {
        [JsonProperty(PropertyName = "rows")]
        public int Rows;

        [JsonProperty(PropertyName = "columns")]
        public int Cols;

        [JsonProperty(PropertyName = "mines")]
        public int Mines;
    }

    [Serializable]
    public class BaseRequest
    {
        [JsonProperty(PropertyName = "row")]
        public int Row;

        [JsonProperty(PropertyName = "col")]
        public int Col;
    }

    [Serializable]
    public class MarkCellRequest: BaseRequest
    {
        [JsonProperty(PropertyName = "none")]
        public bool None;

        [JsonProperty(PropertyName = "flag")]
        public bool Flag;

        [JsonProperty(PropertyName = "question")]
        public bool Question;
    }


    [Serializable]
    public class ErrorResponse
    {
        [JsonProperty(PropertyName = "code")]
        public int Code;

        [JsonProperty(PropertyName = "status")]
        public string Status;

        [JsonProperty(PropertyName = "error")]
        public string Error;

        [JsonProperty(PropertyName = "message")]
        public string Message;
    }
}
