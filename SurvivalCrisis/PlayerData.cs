using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SurvivalCrisis
{
    public partial class PlayerData
    {
        public string Name { get; set; }
        public int UserID { get; set; }
        [JsonIgnore]
        public int Score => this.SurvivorDatas.TotalScore + this.TraitorDatas.TotalScore;
        public int Coins
        {
            get;
            set;
        }

        public PlayerBoosts Boosts
        {
            get;
            set;
        }

        public List<int> UnlockedTitles
        {
            get;
            set;
        }
        public List<int> UnlockedPrefixs
        {
            get;
            set;
        }
        public int CurrentTitleID
        {
            get;
            set;
        }
        public int CurrentPrefixID
        {
            get;
            set;
        }

        public DataDetail SurvivorDatas { get; set; }
        public DataDetail TraitorDatas { get; set; }

        [JsonIgnore]
        public int GameCounts => this.SurvivorDatas.GameCounts + this.TraitorDatas.GameCounts;
        [JsonIgnore]
        public int WinCounts => this.SurvivorDatas.WinCounts + this.SurvivorDatas.WinCounts;
        [JsonIgnore]
        public int MaxSurvivalFrames => Math.Max(this.SurvivorDatas.MaxSurvivalFrames, this.TraitorDatas.MaxSurvivalFrames);

        public PlayerData(int userID)
        {
            this.UserID = userID;
            this.SurvivorDatas = new DataDetail();
            this.TraitorDatas = new DataDetail();
            this.Boosts = new PlayerBoosts();
            this.UnlockedPrefixs = new List<int>();
            this.UnlockedTitles = new List<int>();
        }
    }
}