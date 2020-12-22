using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Shared;
using RestSharp;

namespace Minesweeper.Services
{
    public class MinesweeperServices : IMinesweeperServices
    {
        public MinesweeperServices(MinesweeperServicesConfiguration config)
        {
            this.Configuration = config;
        }

        private RestClient Client { get; set; }

        private MinesweeperServicesConfiguration Configuration { get; set; }



        public IList<GameData> GetGameListByUserId(string userId)
        {
            this.CreateClient();

            string resource = "minesweeper/users/{user_id}/games";
            var req = new RestRequest(resource, Method.GET);
            req.AddParameter("user_id", userId, ParameterType.UrlSegment);

            IRestResponse res = this.Client.Get(req);
            if (res.IsSuccessful)
            {
                var output = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<GameData>>(res.Content);
                return output;
            }
            else
            {
                ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(res.Content);
                throw new Exception(err.Message);
            }
        }


        public GameData GetGame(string userId, string gameId)
        {
            this.CreateClient();

            string resource = "minesweeper/users/{user_id}/games/{game_id}/show";
            var req = new RestRequest(resource, Method.GET);
            req.AddParameter("user_id", userId, ParameterType.UrlSegment);
            req.AddParameter("game_id", gameId, ParameterType.UrlSegment);

            IRestResponse res = this.Client.Get(req);
            if (res.IsSuccessful)
            {
                var output = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(res.Content);
                return output;
            }
            else
            {
                ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(res.Content);
                throw new Exception(err.Message);
            }
        }

        public GameData NewGame(string userId, int rowCount, int colCount, int mines)
        {
            this.CreateClient();

            string resource = "minesweeper/users/{user_id}/games";
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(new NewGameRequest() { Rows = rowCount, Cols = colCount, Mines = mines });

            var req = new RestRequest(resource, Method.POST, DataFormat.Json);
            req.AddParameter("user_id", userId, ParameterType.UrlSegment);
            req.AddJsonBody(body);

            IRestResponse res = this.Client.Post(req);

            if (res.IsSuccessful)
            {
                var output = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(res.Content);
                return output;
            }
            else
            {
                ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(res.Content);
                throw new Exception(err.Message);
            }
        }

        public GameData MarkCell(string userId, string gameId, int row, int col, CellMarkType mark)
        {
            this.CreateClient();

            string resource = "minesweeper/users/{user_id}/games/{game_id}/mark";
            var req = new RestRequest(resource, Method.PUT, DataFormat.Json);
            req.AddParameter("user_id", userId, ParameterType.UrlSegment);
            req.AddParameter("game_id", gameId, ParameterType.UrlSegment);

            var args = new MarkCellRequest() { Col = col, Row = row };
            args.Flag = mark == CellMarkType.CellMarkType_Flag;
            args.None = mark == CellMarkType.CellMarkType_None;
            args.Question = mark == CellMarkType.CellMarkType_Question;
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(args);

            req.AddJsonBody(body);


            IRestResponse res = this.Client.Put(req);
            if (res.IsSuccessful)
            {
                var output = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(res.Content);
                return output;
            }
            else
            {
                ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(res.Content);
                throw new Exception(err.Message);
            }
        }

        public GameData RevealCell(string userId, string gameId, int row, int col)
        {
            this.CreateClient();

            string resource = "minesweeper/users/{user_id}/games/{game_id}/reveal";
            var req = new RestRequest(resource, Method.PUT, DataFormat.Json);
            req.AddParameter("user_id", userId, ParameterType.UrlSegment);
            req.AddParameter("game_id", gameId, ParameterType.UrlSegment);

            var args = new BaseRequest() { Col = col, Row = row };
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(args);
            req.AddJsonBody(body);


            IRestResponse res = this.Client.Put(req);
            if (res.IsSuccessful)
            {
                var output = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(res.Content);
                return output;
            }
            else
            {
                ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(res.Content);
                throw new Exception(err.Message);
            }

        }

        public void DeleteGame(string userId, string gameId)
        {
            this.CreateClient();

            string resource = "minesweeper/users/{user_id}/games/{game_id}";
            var req = new RestRequest(resource, Method.DELETE);
            req.AddParameter("user_id", userId, ParameterType.UrlSegment);
            req.AddParameter("game_id", gameId, ParameterType.UrlSegment);


            IRestResponse res = this.Client.Delete(req);
            if (!res.IsSuccessful)
            {
                ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(res.Content);
                throw new Exception(err.Message);
            }

        }

        public void DeleteGamesByUser(string userId)
        {
            this.CreateClient();

            string resource = "minesweeper/users/{user_id}/games";
            var req = new RestRequest(resource, Method.DELETE);
            req.AddParameter("user_id", userId, ParameterType.UrlSegment);


            IRestResponse res = this.Client.Delete(req);
            if (!res.IsSuccessful)

            {
                ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(res.Content);
                throw new Exception(err.Message);
            }
        }


        private void CreateClient()
        {
            if (this.Client == null)
            {
                this.Client = new RestClient(this.Configuration.BaseUrl);
            }
        }
    }
}
