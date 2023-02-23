using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using JObject = Newtonsoft.Json.Linq.JObject;

namespace SurvivalCrisis
{
    public class CrisisConfig
    {
        public struct Point4
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public static implicit operator TileSection(in Point4 value) => new TileSection(value.Left, value.Top, value.Right - value.Left, value.Bottom - value.Top);

            public override string ToString()
            {
                var X = this.Left;
                var Y = this.Top;
                var Width = this.Right - this.Left;
                var Height = this.Bottom - this.Top;
                return $"({X}, {Y}) Width({Width}) Height({Height})";
            }
        }
        public Point4 Hall
        {
            get;
            set;
        }
        public Point4 Lobby
        {
            get;
            set;
        }
        public Point4 WaitingZone
        {
            get;
            set;
        }
        public Point4 GamingZone
        {
            get;
            set;
        }

        public Point4 Islands
        {
            get;
            set;
        }
        public Point4 Surface
        {
            get;
            set;
        }
        public Point4 Underground
        {
            get;
            set;
        }
        public Point4 Cave
        {
            get;
            set;
        }
        public Point4 Spheres
        {
            get;
            set;
        }
        public Point4 Maze
        {
            get;
            set;
        }
        public Point4 CaveEx
        {
            get;
            set;
        }
        public Point4 Hell
        {
            get;
            set;
        }

        public Point[] SpheresLarge
        {
            get;
            set;
        }

        public static CrisisConfig LoadFile(string path)
        {
            var text = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<CrisisConfig>(text);
        }
        public void Save(string path)
        {
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, text);
        }


        public void ShowTo(TSPlayer player)
        {
            player.SendMessage($"{nameof(this.Hall)}: {this.Hall}", Color.Blue);
            player.SendMessage($"{nameof(this.WaitingZone)}: {this.WaitingZone}", Color.Blue);
            player.SendMessage($"{nameof(this.GamingZone)}: {this.GamingZone}", Color.Blue);

            player.SendMessage($"{nameof(this.Islands)}: {this.Islands}", Color.Blue);
            player.SendMessage($"{nameof(this.Surface)}: {this.Surface}", Color.Blue);
            // player.SendMessage($"{nameof(Underground)}: {Underground}", Color.Blue);
            player.SendMessage($"{nameof(this.Cave)}: {this.Cave}", Color.Blue);
            player.SendMessage($"{nameof(this.Spheres)}: {this.Spheres}", Color.Blue);
            player.SendMessage($"{nameof(this.Maze)}: {this.Maze}", Color.Blue);
            player.SendMessage($"{nameof(this.CaveEx)}: {this.CaveEx}", Color.Blue);
            player.SendMessage($"{nameof(this.Hell)}: {this.Hell}", Color.Blue);
        }
    }
}