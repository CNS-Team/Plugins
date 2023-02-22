using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace SurvivalCrisis
{
    using DataSet = Dictionary<int, PlayerData>;
    public class DataManager
    {
        private DataSet ExistedDatas;
        [JsonIgnore]
        public List<PlayerData> SortedDatas { get; }
        [JsonIgnore]
        public string SavePath { get; }

        public DataManager(string savePath)
        {
            this.SavePath = savePath;
            this.SortedDatas = new List<PlayerData>();
            this.LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (File.Exists(this.SavePath))
            {
                var text = File.ReadAllText(this.SavePath);
                this.ExistedDatas = JsonConvert.DeserializeObject<DataSet>(text);
            }
            else
            {
                this.ExistedDatas = new DataSet();
                this.Save();
            }
            this.SortedDatas.AddRange(this.ExistedDatas.Values);
            this.UpdateRank();
        }

        public void Save()
        {
            var text = JsonConvert.SerializeObject(this.ExistedDatas, Formatting.Indented);
            File.WriteAllText(this.SavePath, text);
        }

        public GamePlayer GetPlayer(int index)
        {
            var player = TShock.Players[index];
            var data = this.GetPlayerData(player.Account.ID);
            data.Name = player.Account.Name;
            return new GamePlayer(index, data);
        }
        public PlayerData GetPlayerData(int userID)
        {
            if (!this.ExistedDatas.TryGetValue(userID, out var data))
            {
                data = new PlayerData(userID);
                this.ExistedDatas.Add(userID, data);
                this.UpdateRank();
            }
            return data;
        }

        public List<PlayerData> GetSurvivorRank()
        {
            var list = this.ExistedDatas.Where(p => p.Value.SurvivorDatas.TotalScore != 0).Select(p => p.Value);
            list = list.OrderBy(data => -data.SurvivorDatas.TotalScore);
            return list.ToList();
        }

        public List<PlayerData> GetTraitorRank()
        {
            var list = this.ExistedDatas.Where(p => p.Value.TraitorDatas.TotalScore != 0).Select(p => p.Value);
            list = list.OrderBy(data => -data.TraitorDatas.TotalScore);
            return list.ToList();
        }

        public void UpdateRank()
        {
            this.SortedDatas.Sort((left, right) =>
            {
                var subtraction = right.Score - left.Score;
                return subtraction == 0 ? right.UserID - left.UserID : subtraction;
            });
        }
    }
}