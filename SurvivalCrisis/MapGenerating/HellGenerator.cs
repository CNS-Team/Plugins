using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis.MapGenerating
{
    public class HellGenerator : Generator
    {
        private readonly Random rand;
        public HellGenerator() : base(SurvivalCrisis.Regions.Hell)
        {
            this.rand = new Random();
        }

        public override void Generate()
        {
            SurvivalCrisis.DebugMessage("生成地狱...");
            this.Coverage.Fill(TileID.Ash);
            this.AddChest(ChestLevel.V3, 30);
            this.AddChest(ChestLevel.V4, 40);
            this.AddChest(ChestLevel.V6, 50);
            SurvivalCrisis.DebugMessage("地狱生成成功");
        }

        private void AddChest(ChestLevel chest, int count)
        {
            const int brickWidth = 4;
            const int width = 14; // 箱子正方形宽度
            var placed = new List<Point>
            {
                new Point(30000, 30000)
            };
            for (var i = 0; i < count; i++)
            {
                var x = 0;
                var y = 0;
                bool canPlaceChest;
                do
                {
                    x = this.rand.Next(0, this.Coverage.Width - width);
                    y = this.rand.Next(0, this.Coverage.Height - width);
                    var xMin = placed.Min(v => Math.Abs(x - v.X));
                    var yMin = placed.Min(v => Math.Abs(y - v.Y));
                    canPlaceChest = xMin >= width || yMin >= width;// && Coverage[x + 0, y + 1].active() && Coverage[x + 1, y + 1].active();
                }
                while (!canPlaceChest);
                placed.Add(new Point(x, y));
                for (var j = x; j < x + width; j++)
                {
                    for (var k = y; k < y + width; k++)
                    {
                        this.Coverage[j, k].active(false);
                        this.Coverage[j, k].liquid = 255;
                        this.Coverage[j, k].liquidType(Tile.Liquid_Lava);
                    }
                }
                for (var j = x + ((width - (brickWidth * 2) - 2) / 2); j < x + width - ((width - (brickWidth * 2) - 2) / 2); j++)
                {
                    for (var k = y + ((width - (brickWidth * 2) - 2) / 2); k < y + width - ((width - (brickWidth * 2) - 2) / 2); k++)
                    {
                        this.Coverage[j, k].type = TileID.LihzahrdBrick;
                        this.Coverage[j, k].active(true);
                    }
                }
                for (var j = x + ((width - 2) / 2); j < x + width - ((width - 2) / 2); j++)
                {
                    for (var k = y + ((width - 2) / 2); k < y + width - ((width - 2) / 2); k++)
                    {
                        this.Coverage[j, k].active(false);
                        this.Coverage[j, k].liquid = 0;
                    }
                }
                chest.PlaceChest(x + ((width - 2) / 2) + this.Coverage.X, y + ((width - 2) / 2) + 1 + this.Coverage.Y);
                for (var j = x + ((width - 2) / 2); j < x + width - ((width - 2) / 2); j++)
                {
                    for (var k = y + ((width - 2) / 2); k < y + width - ((width - 2) / 2); k++)
                    {
                        this.Coverage[j, k].liquid = 255;
                    }
                }
            }
        }
    }
}