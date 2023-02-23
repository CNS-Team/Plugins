using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis.MapGenerating
{
    public class MazeGenerator : Generator
    {
        private readonly Random rand;
        private readonly ushort tileType;
        private readonly ushort wallType;
        // 通道边长
        private const int m = 6;
        // 迷宫墙壁厚度
        private const int o = 2;
        private const int l = m + (o * 2);
        public MazeGenerator() : base(SurvivalCrisis.Regions.Maze)
        {
            this.rand = new Random();
            this.tileType = TileID.SnowBrick;
            this.wallType = WallID.SnowBrick;
        }
        public override void Generate()
        {
            SurvivalCrisis.DebugMessage("生成迷宫...");
            this.GenerateMaze();
            this.AddChests(ChestLevel.V2, 50);
            this.AddChests(ChestLevel.V3, 125);
            this.AddChests(ChestLevel.V4, 75);
            SurvivalCrisis.DebugMessage("迷宫生成成功");
        }

        private void AddChests(ChestLevel chest, int count)
        {
            var width = (int) Math.Ceiling(this.Coverage.Width / (double) l);
            var height = (int) Math.Ceiling(this.Coverage.Height / (double) l);
            for (var z = 0; z < count; z++)
            {
                int i;
                int j;
                bool canPlaceChest;
                var t = 0;
                do
                {
                    i = this.rand.Next(width - 1);
                    j = this.rand.Next(height - 1);
                    canPlaceChest =
                        this.Coverage[(i * l) + (l / 2) - 1, ((j + 1) * l) - o].active() &&
                        this.Coverage[(i * l) + (l / 2) - 0, ((j + 1) * l) - o].active() &&
                        !this.Coverage[(i * l) + (l / 2) - 1, ((j + 1) * l) - (1 + o)].active();
                }
                while (!canPlaceChest && ++t < 200);
                chest.PlaceChest((i * l) + (l / 2) - 1 + this.Coverage.X, ((j + 1) * l) - (1 + o) + this.Coverage.Y);
            }
        }

        private void GenerateMaze()
        {
            this.Coverage.Fill(this.tileType, this.wallType);
            var width = (int) Math.Ceiling(this.Coverage.Width / (double) l);
            var height = (int) Math.Ceiling(this.Coverage.Height / (double) l);
            var tags = new bool[width, height];

            void build(int i, int j)
            {
                var arrs = new Stack<(int a, int b)[]>(50);
                var IJs = new Stack<(int I, int J)>(50);
                var idxes = new Stack<byte>(50);

                var arr0 = new[] { (i + 1, j), (i - 1, j), (i, j + 1), (i, j - 1) };
                this.rand.Shuffle(arr0);

                arrs.Push(arr0);
                IJs.Push((i, j));
                idxes.Push(0);

                while (IJs.Count > 0)
                {
                    var (I, J) = IJs.Peek();
                    var idx = idxes.Peek();
                    var arr = arrs.Peek();
                    {
                        var (a, b) = arr[idx];
                        if (0 <= a && a < width && 0 <= b && b < height)
                        {
                            if (!tags[a, b])
                            {
                                tags[a, b] = true;
                                switch ((I - a, J - b))
                                {
                                    case (1, 0):
                                        for (var z = o; z < l - o; z++)
                                        {
                                            for (var w = 0; w < o; w++)
                                            {
                                                this.Coverage.TryRemoveTile((a * l) + l - (1 + w), (b * l) + z);
                                                this.Coverage.TryRemoveTile((a * l) + l + w, (b * l) + z);
                                                // Coverage.TryPlaceWall(v.a * l + (l - 0), v.b * l + z, WallID.Dirt);
                                            }
                                        }
                                        break;
                                    case (-1, 0):
                                        for (var z = o; z < l - o; z++)
                                        {
                                            for (var w = 0; w < o; w++)
                                            {
                                                this.Coverage.TryRemoveTile((a * l) + w, (b * l) + z);
                                                this.Coverage.TryRemoveTile((a * l) - (1 + w), (b * l) + z);
                                                // Coverage.TryPlaceWall(v.a * l - 1, v.b * l + z, WallID.Stone);
                                            }
                                        }
                                        break;
                                    case (0, 1):
                                        for (var z = o; z < l - o; z++)
                                        {
                                            for (var w = 0; w < o; w++)
                                            {
                                                this.Coverage.TryRemoveTile((a * l) + z, (b * l) + l - (1 + w));
                                                this.Coverage.TryRemoveTile((a * l) + z, (b * l) + l + w);
                                                // Coverage.TryPlaceWall(v.a * l + z, v.b * l + (l - 0), WallID.SnowWallUnsafe);
                                            }
                                        }
                                        break;
                                    case (0, -1):
                                        for (var z = o; z < l - o; z++)
                                        {
                                            for (var w = 0; w < o; w++)
                                            {
                                                this.Coverage.TryRemoveTile((a * l) + z, (b * l) + w);
                                                this.Coverage.TryRemoveTile((a * l) + z, (b * l) - (1 + w));
                                                // Coverage.TryPlaceWall(v.a * l + z, v.b * l - 1, WallID.SandstoneBrick);
                                            }
                                        }
                                        break;
                                }

                                //IJs.Push((I, J));
                                //arrs.Push(arr);
                                //idxes.Push(idx);

                                var arrNew = new[] { (a + 1, b), (a - 1, b), (a, b + 1), (a, b - 1) };
                                this.rand.Shuffle(arrNew);

                                arrs.Push(arrNew);
                                IJs.Push((a, b));
                                idxes.Push(0);

                                continue;
                            }
                        }
                    }
                    if (++idx == 4)
                    {
                        for (var z = o; z < l - o; z++)
                        {
                            for (var w = o; w < l - o; w++)
                            {
                                this.Coverage.TryRemoveTile((I * l) + z, (J * l) + w);
                            }
                        }
                        IJs.Pop();
                        arrs.Pop();
                        idxes.Pop();

                    }
                    else
                    {
                        idxes.Pop();
                        idxes.Push(idx);
                    }
                }
            }

            var oI = this.rand.Next(width);
            var oJ = this.rand.Next(height);
            tags[oI, oJ] = true;
            build(oI, oJ);
            const int c = 10;
            for (var i = 1; i <= c; i++)
            {
                oI = width / (c + 1) * i;
                for (var z = o; z < l - o; z++)
                {
                    for (var w = 0; w < o; w++)
                    {
                        this.Coverage.TryRemoveTile((oI * l) + z, w);
                        this.Coverage.TryRemoveTile((oI * l) + z, (height * l) - 1 - w);
                        // Coverage.TryPlaceWall(v.a * l + z, v.b * l + (l - 0), WallID.SnowWallUnsafe);
                    }
                }
            }
            //int cracks = 0;// rand.Next(width * height / 28);
            //for (int i = 0; i < cracks; i++)
            //{
            //	oI = rand.Next(1, width - 1);
            //	oJ = rand.Next(1, height - 1);
            //	if (rand.Next(2) == 0) // 往右
            //	{
            //		for (int z = o; z < l - o; z++)
            //		{
            //			for (int w = 0; w < o; w++)
            //			{
            //				Coverage.TryRemoveTile((oI + 1) * l - (1 + w), oJ * l + z);
            //				Coverage.TryRemoveTile((oI + 1) * l + w, oJ * l + z);
            //				// Coverage.TryPlaceWall(v.a * l + z, v.b * l + (l - 0), WallID.SnowWallUnsafe);
            //			}
            //		}
            //	}
            //	else // 往左
            //	{
            //		for (int z = o; z < l - o; z++)
            //		{
            //			for (int w = 0; w < o; w++)
            //			{
            //				Coverage.TryRemoveTile(oI * l + w, oJ * l + z);
            //				Coverage.TryRemoveTile(oI * l - (1 + w), oJ * l + z);
            //				// Coverage.TryPlaceWall(v.a * l + z, v.b * l + (l - 0), WallID.SnowWallUnsafe);
            //			}
            //		}
            //	}
            //}

            const int roomWidth = 8;
            var roomCount = width * height / (10 * roomWidth * roomWidth);
            var rooms = new List<TileSection>(roomCount + 1)
            {
                new TileSection(30000, 30000, 1, 1)
            };
            var pylon = IslandsGenerator.TileBlockData.FromFile(Path.Combine(SurvivalCrisis.IslandsPath, "Pylon.sec"));
            SurvivalCrisis.Instance.MazePylons = new List<Point>();
            for (var i = 0; i < roomCount; i++)
            {
                var canBuildRoom = false;
                int x;
                int y;
                var t = 0;
                do
                {
                    x = this.rand.Next(roomWidth, width - roomWidth);
                    y = this.rand.Next(roomWidth, height - roomWidth);
                    var xMin = rooms.Min(r => Math.Abs(r.X - x));
                    var yMin = rooms.Min(r => Math.Abs(r.Y - y));
                    canBuildRoom = xMin > roomWidth || yMin > roomWidth;
                    t++;
                }
                while (!canBuildRoom);
                SurvivalCrisis.DebugMessage($"maze room: ({i + 1}/{roomCount}), t: {t}");
                var a = roomWidth * l;
                rooms.Add(new TileSection(x, y, roomWidth, roomWidth));
                for (var j = (x * l) + o; j < (x * l) + a - o; j++)
                {
                    for (var k = (y * l) + o; k < (y * l) + a - o; k++)
                    {
                        this.Coverage.TryRemoveTile(j, k);
                    }
                }
                Main.tile[(x * l) + this.Coverage.X + ((a - 2) / 2) + 0, (y * l) + this.Coverage.Y + ((a - 2) / 2) + 2].type = TileID.EchoBlock;
                Main.tile[(x * l) + this.Coverage.X + ((a - 2) / 2) + 1, (y * l) + this.Coverage.Y + ((a - 2) / 2) + 2].type = TileID.EchoBlock;
                Main.tile[(x * l) + this.Coverage.X + ((a - 2) / 2) + 2, (y * l) + this.Coverage.Y + ((a - 2) / 2) + 2].type = TileID.EchoBlock;
                Main.tile[(x * l) + this.Coverage.X + ((a - 2) / 2) + 0, (y * l) + this.Coverage.Y + ((a - 2) / 2) + 2].active(true);
                Main.tile[(x * l) + this.Coverage.X + ((a - 2) / 2) + 1, (y * l) + this.Coverage.Y + ((a - 2) / 2) + 2].active(true);
                Main.tile[(x * l) + this.Coverage.X + ((a - 2) / 2) + 2, (y * l) + this.Coverage.Y + ((a - 2) / 2) + 2].active(true);
                // Terraria.GameContent.Tile_Entities.TETeleportationPylon.
                // Terraria.GameContent.Tile_Entities.TETeleportationPylon.Place(x + (a - 2) / 2 + 0, y + (a - 2) / 2 + 1);
                var sec = new TileSection((x * l) + this.Coverage.X + ((a - 2) / 2) + 0, (y * l) + this.Coverage.Y + ((a - 2) / 2) - 1, 3, 5);
                pylon.AffixTo(sec);
                SurvivalCrisis.Instance.MazePylons.Add(new Point((x * l) + this.Coverage.X + ((a - 2) / 2), (y * l) + this.Coverage.Y + ((a - 2) / 2) - 1));
                // Terraria.DataStructures.TileEntity.PlaceEntityNet(x * l + Coverage.X + (a - 2) / 2 + 0, y * l + Coverage.Y + (a - 2) / 2 + 1, Terraria.GameContent.Tile_Entities.TETeleportationPylon._myEntityID);
                // var info = new Terraria.GameContent.TeleportPylonInfo();
                // info.PositionInTiles = new Terraria.DataStructures.Point16(sec.X, sec.Y);
                // info.TypeOfPylon = Terraria.GameContent.TeleportPylonType.Victory;
                // Main.PylonSystem.Pylons.Add(info);
                // WorldGen.PlaceObject(x * l + Coverage.X +(a - 2) / 2 + 0, y * l + Coverage.Y + (a - 2) / 2 + 1, Terraria.GameContent.Tile_Entities.TETeleportationPylon._myEntityID, false, rand.Next(10));
            }
        }
    }
}