using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis.MapGenerating
{
    public partial class CaveGenerator : Generator
    {
        protected Random rand;
        protected ushort oreTypeEvil;
        protected ushort oreTypeH3;
        protected ushort aBrick;
        protected BiomeType biome;
        public CaveGenerator(TileSection? range = null) : base(range ?? SurvivalCrisis.Regions.Cave)
        {
            this.rand = new Random();
            this.oreTypeEvil = this.rand.Next(2) == 0 ? TileID.Demonite : TileID.Crimtane;
            this.oreTypeH3 = this.rand.Next(2) == 0 ? TileID.Titanium : TileID.Adamantite;
            this.aBrick = this.rand.Next(4) switch
            {
                0 => TileID.SolarBrick,
                1 => TileID.VortexBrick,
                2 => TileID.NebulaBrick,
                3 => TileID.StardustBrick
            };
            this.biome = BiomeType.Icy;
        }

        public override void Generate()
        {
            SurvivalCrisis.DebugMessage("生成洞穴...");
            this.GenerateCave(0.05);
            this.AddLifeCrystal(300);
            this.AddChest(ChestLevel.V1, 40);
            this.AddChest(ChestLevel.V2, 180);
            this.AddChest(ChestLevel.V3, 50);
            this.AddChest(ChestLevel.V7, 10);
            SurvivalCrisis.DebugMessage("洞穴生成成功");
        }


        protected ushort GetTileType(double f, BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.DeepCave:
                    if (f < 0.39)
                    {
                        return this.oreTypeH3;
                    }

                    if (f < 0.45)
                    {
                        return TileID.Stone;
                    }

                    if (f < 0.475)
                    {
                        return TileID.LunarBrick;
                    }

                    if (f < 0.49)
                    {
                        return this.aBrick;
                    }

                    break;
                case BiomeType.Icy:
                    if (f < 0.335)
                    {
                        return this.oreTypeEvil;
                    }

                    if (f < 0.37)
                    {
                        return TileID.SmoothSandstone;
                    }

                    if (f < 0.4)
                    {
                        return TileID.BreakableIce;
                    }

                    if (f < 0.43)
                    {
                        return TileID.IceBlock;
                    }

                    if (f < 0.49)
                    {
                        return TileID.SnowBlock;
                    }

                    break;
                case BiomeType.Meteorite:
                    if (f < 0.4)
                    {
                        return TileID.PumpkinBlock;
                    }

                    if (f < 0.45)
                    {
                        return TileID.Stone;
                    }

                    if (f < 0.475)
                    {
                        return TileID.TinPlating;
                    }

                    if (f < 0.49)
                    {
                        return TileID.MeteoriteBrick;
                    }

                    break;
                case BiomeType.Jungle:
                    if (f < 0.35)
                    {
                        return TileID.PumpkinBlock;
                    }

                    if (f < 0.45)
                    {
                        return TileID.Mudstone;
                    }

                    if (f < 0.475)
                    {
                        return TileID.Mud;
                    }

                    if (f < 0.49)
                    {
                        return TileID.JungleThorns;
                    }

                    break;
                case BiomeType.Building:
                    if (f < 0.43)
                    {
                        return TileID.Gold;
                    }

                    if (f < 0.47)
                    {
                        return TileID.Stone;
                    }

                    if (f < 0.49)
                    {
                        return TileID.DynastyWood;
                    }

                    break;
            }
            return TileID.HellstoneBrick;
        }

        protected ushort GetWallType(BiomeType biome)
        {
            return biome switch
            {
                BiomeType.Icy => WallID.SnowWallUnsafe,
                BiomeType.Meteorite => WallID.CopperPipeWallpaper,
                BiomeType.Jungle => WallID.Jungle,
                BiomeType.Building => WallID.LivingWood,
                _ => 0,
            };
        }
        protected void FixTop(double delta)
        {
            var perlinNoise = new PerlinNoise();
            for (var i = this.Coverage.Left; i < this.Coverage.Right; i++)
            {
                double X = delta * i, Y = delta * this.Coverage.Top;
                var v = (int) (perlinNoise.Value(X, Y) * 12);
                for (var j = this.Coverage.Top - 1; j > this.Coverage.Top - v; j--)
                {
                    if (Main.tile[i, j].active())
                    {
                        break;
                    }

                    Main.tile[i, j].active(false);
                    Main.tile[i, j].wall = 0;
                }
            }
        }
        protected void GenerateCave(double delta)
        {
            var perlinNoise = new PerlinNoise();
            for (var i = 0; i < this.Coverage.Width; i++)
            {
                for (var j = 0; j < this.Coverage.Height; j++)
                {
                    var X = delta * i;
                    var Y = delta * j;
                    var v = perlinNoise.Value(X, Y);
                    var biome = this.biome;
                    var tileType = this.GetTileType(v, biome);
                    var wallType = this.GetWallType(biome);
                    if (tileType != TileID.HellstoneBrick)
                    {
                        this.Coverage.PlaceTileAt(i, j, tileType);
                        if (tileType == TileID.Crimtane || tileType == TileID.Demonite)
                        {
                            this.Coverage[i, j].color(PaintID.WhitePaint);
                        }
                    }
                    this.Coverage.PlaceWallAt(i, j, wallType);
                }
            }
            //FixTop(range, delta);
        }
        protected virtual void AddChest(ChestLevel chest, int count)
        {
            for (var i = 0; i < count; i++)
            {
                int x;
                int y;
                bool canPlaceChest;
                do
                {
                    x = this.rand.Next(0, this.Coverage.Width - 1);
                    y = this.rand.Next(1, this.Coverage.Height - 1);
                    canPlaceChest =
                        !this.Coverage[x, y - 1].active() && !this.Coverage[x + 1, y - 1].active() &&
                        !this.Coverage[x, y - 0].active() && !this.Coverage[x + 1, y - 0].active() &&
                        this.Coverage[x, y + 1].active() && this.Coverage[x + 1, y + 1].active();
                }
                while (!canPlaceChest);
                this.Coverage[x + 0, y + 1].type = TileID.SnowBrick;
                this.Coverage[x + 1, y + 1].type = TileID.SnowBrick;
                this.Coverage[x + 0, y + 1].active(true);
                this.Coverage[x + 1, y + 1].active(true);
                this.Coverage[x + 0, y + 1].slope(0);
                this.Coverage[x + 1, y + 1].slope(0);
                chest.PlaceChest(x + this.Coverage.X, y + this.Coverage.Y);
            }
        }
        protected void AddLifeCrystal(int count)
        {
            for (var i = 0; i < count; i++)
            {
                int x;
                int y;
                bool canPlaceCrystal;
                do
                {
                    x = this.rand.Next(0, this.Coverage.Width - 1);
                    y = this.rand.Next(1, this.Coverage.Height - 1);
                    canPlaceCrystal =
                        !this.Coverage[x, y - 1].active() && !this.Coverage[x + 1, y - 1].active() &&
                        !this.Coverage[x, y - 0].active() && !this.Coverage[x + 1, y - 0].active() &&
                        this.Coverage[x, y + 1].active() && this.Coverage[x + 1, y + 1].active();
                }
                while (!canPlaceCrystal);
                this.Coverage[x + 0, y + 1].type = TileID.SnowBrick;
                this.Coverage[x + 1, y + 1].type = TileID.SnowBrick;
                this.Coverage[x + 0, y + 1].active(true);
                this.Coverage[x + 1, y + 1].active(true);
                this.Coverage[x + 0, y + 1].slope(0);
                this.Coverage[x + 1, y + 1].slope(0);
                WorldGen.AddLifeCrystal(x + this.Coverage.X, y + this.Coverage.Y);
            }
        }
    }
}