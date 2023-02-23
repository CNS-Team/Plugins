using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis.MapGenerating
{
    public class SurfaceGenerator : Generator
    {
        private readonly Random rand;
        private IslandsGenerator.TileBlockData hall;
        private IslandsGenerator.TileBlockData npcHouse;
        private readonly int[] snowMinX;
        private readonly int[] snowMaxX;
        private readonly int snowTop;
        private readonly int snowBottom;
        private readonly int snowOriginLeft;
        private readonly int snowOriginRight;
        private readonly int waterLine;
        private readonly int lavaLine;
        private readonly int maxTunnels;
        private int numTunnels;
        private readonly int[] tunnelX;
        public SurfaceGenerator() : base(SurvivalCrisis.Regions.Surface)
        {
            this.rand = new Random();
            this.snowMinX = new int[this.Coverage.Height];
            this.snowMaxX = new int[this.Coverage.Height];
            this.snowTop = 0;
            this.snowBottom = 0;
            this.snowOriginLeft = 0;
            this.snowOriginRight = this.Coverage.Width;
            this.lavaLine = this.Coverage.Height * 3 / 4;
            this.waterLine = 0;
            this.maxTunnels = 10;
            this.tunnelX = new int[this.maxTunnels];
            this.numTunnels = 0;
        }

        public override void Generate()
        {
            SurvivalCrisis.DebugMessage("生成地表...");
            try
            {
                this.npcHouse = IslandsGenerator.TileBlockData.FromFile(Path.Combine(SurvivalCrisis.IslandsPath, "NPCHouse.sec"));
                this.hall = IslandsGenerator.TileBlockData.FromFile(Path.Combine(SurvivalCrisis.IslandsPath, "Hall.sec"));
                this.GenerateSnowySurface();
                this.GenerateHall();
                this.AddTrees();
            }
            catch (Exception e)
            {
                TSPlayer.All.SendErrorMessage(e.ToString());
                TSPlayer.Server.SendErrorMessage(e.ToString());
            }
            SurvivalCrisis.DebugMessage("地表生成成功");
        }

        private void AddTrees()
        {
            var count = 0;
            var maxCount = 8000;
            var settings = new WorldGen.GrowTreeSettings
            {
                TreeHeightMax = 50,
                TreeHeightMin = 10,
                GroundTest = x => true,
                WallTest = x => true,
                SaplingTileType = TileID.Saplings,
                TreeTileType = TileID.Trees
            };
            for (var x = 0; x < this.Coverage.Width - 1; x++)
            {
                for (var y = 20; y <= 100; y++)
                {
                    var canPlantTree =
                            this.Coverage[x, y].wall == 0 && this.Coverage[x + 1, y].wall == 0 &&
                            !this.Coverage[x, y].active() && !this.Coverage[x + 1, y].active() &&
                            this.Coverage[x, y].liquid == 0 && this.Coverage[x + 1, y].liquid == 0 &&
                            this.Coverage[x, y + 1].active() && this.Coverage[x + 1, y + 1].active() &&
                            this.Coverage[x, y + 1].type == TileID.SnowBlock && this.Coverage[x + 1, y + 1].type == TileID.SnowBlock;

                    if (canPlantTree && this.rand.NextDouble() < 0.15)
                    {
                        WorldGen.GrowTreeWithSettings(this.Coverage.X + x, this.Coverage.Y + y + 1, settings);
                        count++;
                    }
                    if (count > maxCount)
                    {
                        return;
                    }
                }
            }
        }

        private void Tunnels()
        {
            for (var i = 0; i < (int) (this.Coverage.Width * 0.0015); i++)
            {
                if (this.numTunnels >= this.maxTunnels - 1)
                {
                    break;
                }
                var array = new int[10];
                var array2 = new int[10];
                var num861 = this.rand.Next(450, this.Coverage.Width - 450);
                while ((double) num861 > (double) this.Coverage.Width * 0.4 && (double) num861 < (double) this.Coverage.Width * 0.6)
                {
                    num861 = this.rand.Next(450, this.Coverage.Width - 450);
                }
                var num862 = 0;
                bool flag58;
                do
                {
                    flag58 = false;
                    for (var idx = 0; idx < 10; idx++)
                    {
                        for (num861 %= this.Coverage.Width; !this.Coverage[num861, num862].active(); num862++)
                        {
                        }
                        if (this.Coverage[num861, num862].type == 53)
                        {
                            flag58 = true;
                        }
                        array[idx] = num861;
                        array2[idx] = num862 - this.rand.Next(11, 16);
                        num861 += this.rand.Next(5, 11);
                    }
                }
                while (flag58);
                this.tunnelX[this.numTunnels] = array[5];
                this.numTunnels++;
                for (var num864 = 0; num864 < 10; num864++)
                {
                    this.TileRunner(array[num864], array2[num864], this.rand.Next(5, 8), this.rand.Next(6, 9), 0, addTile: true, -2f, -0.3f);
                    this.TileRunner(array[num864], array2[num864], this.rand.Next(5, 8), this.rand.Next(6, 9), 0, addTile: true, 2f, -0.3f);
                }
            }
        }

        private void GenerateHall()
        {
            var npcs = new[] { NPCID.Nurse, NPCID.Guide, NPCID.GoblinTinkerer, NPCID.ArmsDealer, NPCID.Merchant };
            var point = this.Coverage.Center;
            point.Y = 20;
            while (!this.Coverage[point].active() && this.Coverage[point].wall == WallID.None)
            {
                point.Y++;
            }
            point.Y -= 5;
            SurvivalCrisis.Regions.Hall = this.Coverage.SubSection(point.X - (this.hall.Width / 2), point.Y - this.hall.Height, this.hall.Width + (npcs.Length * (2 + this.npcHouse.Width)), this.hall.Height);
            SurvivalCrisis.Regions.Hall.TurnToAir();
            this.hall.AffixTo(SurvivalCrisis.Regions.Hall);

            var raw = point;

            point.X += (this.hall.Width / 2) + 2;
            point.Y -= (this.hall.Height / 2) + (this.npcHouse.Height / 2);


            for (var i = 0; i < npcs.Length; i++)
            {
                var section = this.Coverage.SubSection(point.X, point.Y, this.npcHouse.Width, this.npcHouse.Height);
                this.npcHouse.AffixTo(section);
                var idx = NPC.NewNPC(new EntitySource_DebugCommand(), (point.X + (this.npcHouse.Width / 2) + this.Coverage.X) * 16, (point.Y + 2 + this.Coverage.Y) * 16, npcs[i]);
                TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", idx);
                point.X += 2 + this.npcHouse.Width;
            }

            raw.Y -= this.hall.Height;
        }

        private void GenerateSnowySurface()
        {
            // Tunnels();
            //for (int i = 0; i < 10; i++)
            //{
            //	var x = rand.Next(Coverage.Width);
            //	var y = rand.Next(35, 60);
            //	TileRunner(x, y, rand.Next(5, 10), rand.Next(50, 200), TileID.SnowBlock, true, rand.Next(250, 400) / 100f, killTile: false);
            //}
            //for (int i = 0; i < 60; i++)
            //{
            //	var x = rand.Next(Coverage.Width);
            //	var y = rand.Next(45, 70);
            //	TileRunner(x, y, rand.Next(5, 8), rand.Next(20, 80), TileID.SnowBlock, true, rand.Next(250, 400) / 100f, killTile: false);
            //}
            {
                float k = 85;
                float kAcceleration = 0;
                var accelT = 0;
                float kVelocity = 0;
                double m = 0;
                for (var x = 0; x < this.Coverage.Width; x++)
                {
                    if (accelT > 0)
                    {
                        kVelocity += kAcceleration;
                        accelT--;
                    }
                    else
                    {
                        kVelocity *= 0.75f;
                        if (Math.Abs(kVelocity) < 0.2f)
                        {
                            m += 0.200;
                            if (this.rand.NextDouble() < 0.25 + m)
                            {
                                var a = this.rand.Next(-10, 10) / 10f;
                                a = a * a * a * 16;
                                accelT = this.rand.Next(15, 45);
                                kAcceleration = a / accelT;
                                m = 0;
                            }
                        }
                        else
                        {
                            m = 0;
                        }
                    }
                    while (k + kVelocity < 70 || k + kVelocity >= 90)
                    {
                        kVelocity *= -0.25f;
                        kAcceleration *= Math.Sign(kVelocity);
                    }
                    k += kVelocity;
                    for (var y = (int) k; y < 90; y++)
                    {
                        this.Coverage.PlaceTileAt(new Point(x, y), TileID.SnowBlock);
                    }
                }
            }
            for (var k = 90; k < this.Coverage.Height; k++)
            {
                for (var j = 0; j < this.Coverage.Width; j++)
                {
                    this.Coverage.PlaceTileAt(new Point(j, k), TileID.SnowBlock);
                }
            }
            for (var i = 0; i < 10; i++)
            {
                var x = this.rand.Next(this.Coverage.Width);
                var y = this.rand.Next(50, this.Coverage.Height);
                this.TileRunner(x, y, this.rand.Next(5, 15), this.rand.Next(400, 1300), TileID.SnowBlock, true, speedY: this.rand.Next(250, 400) / 100f, killTile: true);
            }
            for (var i = 0; i < 5; i++)
            {
                var x = this.rand.Next(this.Coverage.Width);
                var y = this.rand.Next(50, this.Coverage.Height);
                this.TileRunner(x, y, this.rand.Next(15, 20), this.rand.Next(750, 1300), TileID.SnowBlock, true, speedY: this.rand.Next(350, 450) / 100f, killTile: true);
            }
            for (var i = 0; i < 60; i++)
            {
                var x = this.rand.Next(this.Coverage.Width);
                var y = this.rand.Next(90, this.Coverage.Height);
                this.TileRunner(x, y, this.rand.Next(15, 40), this.rand.Next(10, 30), TileID.SnowBlock, true, speedY: this.rand.Next(250, 400) / 100f, killTile: true);
            }
            for (var i = 0; i < 100; i++)
            {
                var x = this.rand.Next(this.Coverage.Width);
                var y = this.rand.Next(105, this.Coverage.Height);
                this.TileRunner(x, y, this.rand.Next(10, 20), this.rand.Next(5, 40), TileID.SnowBlock, true, speedY: this.rand.Next(250, 400) / 100f, killTile: true);
                if (this.rand.NextDouble() < 0.3 + (0.6 * (y - 100) / (this.Coverage.Height - 100)))
                {
                    var chest = ChestLevel.V1;
                    if (this.Coverage.Height - y < 80)
                    {
                        var a = this.rand.NextDouble();
                        if (a > 0.95)
                        {
                            chest = ChestLevel.V3;
                        }
                        else if (a > 0.7)
                        {
                            chest = ChestLevel.V2;
                        }
                    }
                    else
                    {
                        if (this.rand.NextDouble() > 0.9)
                        {
                            chest = ChestLevel.V2;
                        }
                    }
                    x += 5;
                    y += 3;
                    this.Coverage.PlaceTileAt(new Point(x + 0, y + 1), TileID.SnowBlock);
                    this.Coverage.PlaceTileAt(new Point(x + 1, y + 1), TileID.SnowBlock);
                    chest.PlaceChest(x + this.Coverage.X, y + this.Coverage.Y);
                }
            }
            for (var i = 0; i < this.Coverage.Width; i++)
            {
                //for (int j = 0; j < 2; j++)
                //{
                //	if (!Coverage[i, j].active())
                //	{
                //		Coverage[i, j].liquid = 1;
                //		Coverage[i, j].liquidType(Tile.Liquid_Water);
                //	}
                //}
                if (!this.Coverage[i, this.Coverage.Height - 2].active())
                {
                    this.Coverage[i, this.Coverage.Height - 1].active(false);
                }
            }
            for (var k = 90; k < this.Coverage.Height; k++)
            {
                for (var j = 0; j < this.Coverage.Width; j++)
                {
                    if (!this.Coverage[j, k].active() || this.Coverage[j, k].type == TileID.Heart || this.Coverage[j, k].type == TileID.Containers)
                    {
                        this.Coverage.PlaceWallAt(new Point(j, k), WallID.SnowWallUnsafe);
                    }
                }
            }
            {
                var x = this.Coverage.Width * 1 / 10;
                var mountCount = this.rand.Next(3, 6);
                for (var i = 0; i < mountCount; i++)
                {
                    var heights = new List<int>();
                    for (; x < this.Coverage.Width * 9 / 10; x++)
                    {
                        for (var y = 60; y < 90; y++)
                        {
                            if (this.Coverage[x, y].active())
                            {
                                if (heights.Count == 0)
                                {
                                    if (this.rand.NextDouble() < 0.5f)
                                    {
                                        heights.Add(y);
                                    }
                                }
                                else
                                {
                                    heights.Add(y);
                                    break;
                                }
                            }
                        }
                        if (heights.Count > 50)
                        {
                            var average = heights.Average();
                            var σ2 = heights.Average(value => (value - average) * (value - average));

                            if (σ2 < 0.07)
                            {
                                SurvivalCrisis.DebugMessage($"({x - 49 + this.Coverage.X}, {heights[0] + this.Coverage.Y})");
                                const float q = 0.4f;
                                var width = this.MakeMount(x - 49, heights[0], ((this.rand.NextFloat() * 1f) + 2.0f) * q * q, 5 * q);
                                x += width;
                                break;
                            }
                            heights.Clear();
                        }
                    }
                }
            }
            {

                var t = 0;
                var maxLakeCount = this.rand.Next(3, 6);
                var x = 0;
                var y = 0;
                var size = this.rand.Next(400, 900);
                var failAttemptCount = 0;
                while (t < maxLakeCount)
                {
                    x = this.rand.Next(x, this.Coverage.Width);
                    y = 0;
                    while (!this.Coverage[x, y].active() && y < this.Coverage.Height)
                    {
                        y++;
                    }
                    if (y == this.Coverage.Height)
                    {
                        continue;
                    }
                    if (this.TryGenerateLake(x, y, size))
                    {
                        size = this.rand.Next(400, 900);
                        failAttemptCount = 0;
                        t++;
                    }
                    else
                    {
                        failAttemptCount++;
                        if (failAttemptCount >= 20)
                        {
                            failAttemptCount = 0;
                            t++;
                        }
                    }
                }
                // Commands.HandleCommand(TSPlayer.Server, "/settle");
            }
            for (var k = 90; k < this.Coverage.Height; k++)
            {
                for (var j = 0; j < this.Coverage.Width; j++)
                {
                    if (!this.Coverage[j, k].active() && this.Coverage[j, k].wall == WallID.None)
                    {
                        this.Coverage[j, k].wall = WallID.SnowWallUnsafe;
                    }
                }
            }
        }

        /// <summary>
        /// (x, y)为山脚(左)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="slope">只是用来大致衡量有多陡罢了</param>
        private int MakeMount(int x, int y, float slope, float initialVel = 3.85f)
        {
            var highest = 10000000;
            x += this.Coverage.X;
            y += this.Coverage.Y;
            var i = x;
            float j = y;
            var jVelocity = -initialVel;
            while (j <= y)
            {
                j += jVelocity * (0.6f + (this.rand.NextFloat() * 0.8f));
                jVelocity += slope * ((this.rand.NextFloat() * 0.05f) + 0.0283f);
                for (var k = (int) Math.Ceiling(j); !Main.tile[i, k].active() && Main.tile[i, k].wall == 0; k++)
                {
                    Main.tile[i, k].active(true);
                    if (i > x)
                    {
                        Main.tile[i - 1, k].slope(0);
                    }
                    Main.tile[i, k].slope(0);
                    Main.tile[i, k].type = TileID.SnowBlock;
                }
                if (jVelocity < -0.002)
                {
                    Main.tile[i, (int) Math.Ceiling(j)].slope(Tile.Type_SlopeDownRight);
                }
                else if (jVelocity > 0.002f)
                {
                    // Main.tile[i, (int)Math.Ceiling(j)].slope(Tile.Type_SlopeUpLeft);
                }
                if (j < highest)
                {
                    highest = (int) Math.Ceiling(j);
                }
                i++;
            }
            var m = ((i + x) / 2) - this.Coverage.X;
            var a = this.rand.Next(x + ((i - x) / 3), i - ((i - x) / 3)) - this.Coverage.X;
            var b = this.rand.Next(highest + ((y - highest) * 4 / 5), highest + ((y - highest) * 6 / 7)) - this.Coverage.Y;
            var dir = Math.Sign(a - m);
            void ClearCircle(Point point, float radius)
            {
                if (point.X - radius < 0 || point.X + radius >= this.Coverage.Width)
                {
                    TSPlayer.All.SendErrorMessage($"point: ({point.X}, {point.Y}) radius: {radius} {this.Coverage.Width}");
                    return;
                }
                for (var i = 0; i <= radius; i++)
                {
                    for (var j = 0; j <= radius; j++)
                    {
                        if ((i * i) + (j * j) <= radius * radius)
                        {
                            this.Coverage[point.X + i, point.Y + j].active(false);
                            this.Coverage[point.X + i, point.Y + j].wall = WallID.SnowWallUnsafe;
                            this.Coverage[point.X - i, point.Y + j].active(false);
                            this.Coverage[point.X - i, point.Y + j].wall = WallID.SnowWallUnsafe;
                            this.Coverage[point.X + i, point.Y - j].active(false);
                            this.Coverage[point.X + i, point.Y - j].wall = WallID.SnowWallUnsafe;
                            this.Coverage[point.X - i, point.Y - j].active(false);
                            this.Coverage[point.X - i, point.Y - j].wall = WallID.SnowWallUnsafe;
                        }
                    }
                }
                bool removeWall(int x, int y)
                {
                    (int X, int Y) = (x, y);
                    var u = this.Coverage[X, Y - 1].wall == WallID.None && !this.Coverage[X, Y - 1].active();
                    var d = this.Coverage[X, Y + 1].wall == WallID.None && !this.Coverage[X, Y + 1].active();
                    return
                        !this.Coverage[X, Y].active() && (u || d);
                }
                for (var i = (int) Math.Ceiling(radius); i >= 0; i--)
                {
                    for (var j = (int) Math.Ceiling(radius); j >= 0; j--)
                    {
                        if ((i * i) + (j * j) <= radius * radius)
                        {
                            if (removeWall(point.X + i, point.Y + j))
                            {
                                this.Coverage[point.X + i, point.Y + j].wall = WallID.None;
                            }
                            if (removeWall(point.X - i, point.Y + j))
                            {
                                this.Coverage[point.X - i, point.Y + j].wall = WallID.None;
                            }
                            if (removeWall(point.X + i, point.Y - j))
                            {
                                this.Coverage[point.X + i, point.Y - j].wall = WallID.None;
                            }
                            if (removeWall(point.X - i, point.Y - j))
                            {
                                this.Coverage[point.X - i, point.Y - j].wall = WallID.None;
                            }
                        }
                    }
                }
            }
            double EI(Point point)
            {
                var radius = 4;
                if (point.X - radius < 0 || point.X + radius >= this.Coverage.Width)
                {
                    TSPlayer.All.SendErrorMessage($"point: ({point.X}, {point.Y}) radius: {radius} {this.Coverage.Width}");
                    return 1;
                }
                var c = 0;
                for (var i = 0; i < radius; i++)
                {
                    for (var j = 0; j < radius; j++)
                    {
                        if ((i * i) + (j * j) <= radius * radius)
                        {
                            if (!this.Coverage[point.X + i, point.Y + j].active())
                            {
                                c++;
                            }
                            if (!this.Coverage[point.X - i, point.Y + j].active())
                            {
                                c++;
                            }
                            if (!this.Coverage[point.X + i, point.Y - j].active())
                            {
                                c++;
                            }
                            if (!this.Coverage[point.X - i, point.Y - j].active())
                            {
                                c++;
                            }
                        }
                    }
                }
                return c / Math.PI * radius * radius;
            }
            float aU = a;
            float bU = b;
            while (EI(new Point((int) (aU + (dir * 2 * 5.5f)), (int) bU)) < 0.66)
            {
                var current = new Point((int) aU, (int) bU);
                ClearCircle(current, (this.rand.NextFloat() * 1.5f) + 4.25f);
                aU += (0.75f + (this.rand.NextFloat() * 1.5f)) * dir;
                if (this.rand.NextDouble() < 0.3)
                {
                    bU += 1;
                }
            }
            for (var z = 0; z < 2; z++)
            {
                aU += dir * 5.5f;
                var current = new Point((int) aU, (int) bU);
                ClearCircle(current, (this.rand.NextFloat() * 1.5f) + 4.55f);
                aU += (0.75f + (this.rand.NextFloat() * 1.5f)) * dir;
                if (this.rand.NextDouble() < 0.3)
                {
                    bU += 1;
                }
            }
            float aD = a;
            float bD = b;
            while (bD < 155)
            {
                var current = new Point((int) aD, (int) bD);
                ClearCircle(current, (this.rand.NextFloat() * 1.5f) + 4.25f);
                aD += (0.55f + (this.rand.NextFloat() * 1.5f)) * dir;
                bD += (this.rand.NextFloat() * 3f) + 3f;
            }
            this.TileRunner((int) aD, (int) bD, this.rand.Next(9, 16), this.rand.Next(200, 800), TileID.SnowBlock, true, speedY: this.rand.Next(350, 450) / 100f, killTile: true);
            // TileRunner(a, b, rand.Next(15, 20), rand.Next(10, 20), TileID.SnowBlock, true, speedX: rand.Next(150, 200) / 100f, killTile: true);

            return i - x;
        }
        private bool TryGenerateLake(int x, int y, int size)
        {
            bool check(Point p)
            {
                return p.X < 3 || p.Y < 2 || p.X + 3 >= this.Coverage.Width || p.Y + 2 >= this.Coverage.Height
                    ? false
                    : this.Coverage[p.X - 1, p.Y + 0].active() &&
                    this.Coverage[p.X - 2, p.Y + 0].active() &&
                    this.Coverage[p.X + 1, p.Y + 0].active() &&
                    this.Coverage[p.X + 2, p.Y + 0].active() &&
                    this.Coverage[p.X + 0, p.Y + 1].active() &&
                    this.Coverage[p.X + 0, p.Y + 2].active();
            }

            var current = new Point(x, y);
            var queue = new Queue<Point>(size);
            var tags = new Dictionary<Point, bool>
            {
                { current, true }
            };
            queue.Enqueue(current);
            while (queue.Count > 0 && tags.Count < size)
            {
                current = queue.Dequeue();
                var pNearby = new List<Point>()
                {
                    new Point(current.X, current.Y + 1),
                    new Point(current.X - 1, current.Y),
                    new Point(current.X + 1, current.Y),
                };
                for (var j = 0; j < 4; j++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        if (this.rand.NextDouble() < 0.65 - (i * i / 66.0) - (j * 0.3))
                        {
                            pNearby.Add(new Point(current.X - i, current.Y + j));
                        }
                        if (this.rand.NextDouble() < 0.65 - (i * i / 66.0) - (j * 0.3))
                        {
                            pNearby.Add(new Point(current.X - i, current.Y + j));
                        }
                        if (this.rand.NextDouble() < 0.4 - (j * 0.09) + (size / 1600.0))
                        {
                            continue;
                        }
                        break;
                    }
                }
                for (var i = 0; i < pNearby.Count; i++)
                {
                    if (check(pNearby[i]) && !tags.ContainsKey(pNearby[i]))
                    {
                        tags.Add(pNearby[i], true);
                        queue.Enqueue(pNearby[i]);
                    }
                }
            }
            if (tags.Count >= size)
            {
                SurvivalCrisis.DebugMessage($"lake: ({x + this.Coverage.X}, {y + this.Coverage.Y}) size: {size}");
                foreach (var p in tags.Keys)
                {
                    this.Coverage[p.X, p.Y].active(false);
                    this.Coverage[p.X, p.Y].liquid = 255;
                    this.Coverage[p.X, p.Y].liquidType(Tile.Liquid_Water);
                }
                foreach (var p in tags.Keys)
                {
                    if (!this.Coverage[p.X, p.Y].active() && this.Coverage[p.X, p.Y].liquid == 255)
                    {
                        if (p.Y > 2 && this.Coverage[p.X, p.Y - 1].active() && !this.Coverage[p.X, p.Y - 2].active())
                        {
                            this.Coverage[p.X, p.Y].active(false);
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private void TileRunnerWeak(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool killTile = false, bool noYChange = false, bool overRide = true, int ignoreTileType = -1)
        {
            var mudWall = false;
            var beachDistance = int.MaxValue;

            var num = strength;
            float num2 = steps;
            var vector = default(Vector2);
            vector.X = i;
            vector.Y = j;
            var vector2 = default(Vector2);
            vector2.X = (float) this.rand.Next(-10, 11) * 0.1f;
            vector2.Y = (float) this.rand.Next(-10, 11) * 0.1f;
            if (speedX != 0f || speedY != 0f)
            {
                vector2.X = speedX;
                vector2.Y = speedY;
            }
            var flag = type == 368;
            var flag2 = type == 367;
            var lava = false;
            while (num > 0.0 && num2 > 0f)
            {
                if (vector.Y < 0f && num2 > 0f && type == 59)
                {
                    num2 = 0f;
                }
                num = strength * (double) (num2 / (float) steps);
                num2 -= 1f;
                var num3 = (int) ((double) vector.X - (num * 0.5));
                var num4 = (int) ((double) vector.X + (num * 0.5));
                var num5 = (int) ((double) vector.Y - (num * 0.5));
                var num6 = (int) ((double) vector.Y + (num * 0.5));
                if (num3 < 1)
                {
                    num3 = 1;
                }
                if (num4 > this.Coverage.Width - 1)
                {
                    num4 = this.Coverage.Width - 1;
                }
                if (num5 < 1)
                {
                    num5 = 1;
                }
                if (num6 > this.Coverage.Height - 1)
                {
                    num6 = this.Coverage.Height - 1;
                }
                for (var k = num3; k < num4; k++)
                {
                    if (k < beachDistance + 50 || k >= this.Coverage.Width - beachDistance - 50)
                    {
                        lava = false;
                    }
                    for (var l = num5; l < num6; l++)
                    {
                        if ((false && l < this.Coverage.Height - 300 && type == 57) || (ignoreTileType >= 0 && this.Coverage[k, l].active() && this.Coverage[k, l].type == ignoreTileType) || !((double) (Math.Abs((float) k - vector.X) + Math.Abs((float) l - vector.Y)) < strength * 0.5 * (1.0 + ((double) this.rand.Next(-10, 11) * 0.015))))
                        {
                            continue;
                        }
                        if (mudWall && (double) l > Main.worldSurface && this.Coverage[k, l - 1].wall != 2 && l < this.Coverage.Height - 210 - this.rand.Next(3) && (double) (Math.Abs((float) k - vector.X) + Math.Abs((float) l - vector.Y)) < strength * 0.45 * (1.0 + ((double) this.rand.Next(-10, 11) * 0.01)))
                        {
                            if (l > this.lavaLine - this.rand.Next(0, 4) - 50)
                            {
                                if (this.Coverage[k, l - 1].wall != 64 && this.Coverage[k, l + 1].wall != 64 && this.Coverage[k - 1, l].wall != 64 && this.Coverage[k + 1, l].wall != 64)
                                {
                                    this.PlaceWall(k, l, 15, mute: true);
                                }
                            }
                            else if (this.Coverage[k, l - 1].wall != 15 && this.Coverage[k, l + 1].wall != 15 && this.Coverage[k - 1, l].wall != 15 && this.Coverage[k + 1, l].wall != 15)
                            {
                                this.PlaceWall(k, l, 64, mute: true);
                            }
                        }
                        if (type < 0)
                        {
                            if (this.Coverage[k, l].type == 53)
                            {
                                continue;
                            }
                            if (type == -2 && this.Coverage[k, l].active() && (l < this.waterLine || l > this.lavaLine))
                            {
                                this.Coverage[k, l].liquid = byte.MaxValue;
                                this.Coverage[k, l].lava(lava);
                                if (l > this.lavaLine)
                                {
                                    this.Coverage[k, l].lava(lava: true);
                                }
                            }
                            this.Coverage[k, l].active(active: false);
                            continue;
                        }
                        if (flag && (double) (Math.Abs((float) k - vector.X) + Math.Abs((float) l - vector.Y)) < strength * 0.3 * (1.0 + ((double) this.rand.Next(-10, 11) * 0.01)))
                        {
                            this.PlaceWall(k, l, 180, mute: true);
                        }
                        if (flag2 && (double) (Math.Abs((float) k - vector.X) + Math.Abs((float) l - vector.Y)) < strength * 0.3 * (1.0 + ((double) this.rand.Next(-10, 11) * 0.01)))
                        {
                            this.PlaceWall(k, l, 178, mute: true);
                        }
                        if (overRide || !this.Coverage[k, l].active())
                        {
                            var tile = this.Coverage[k, l];
                            var flag3 = false;
                            flag3 = Main.tileStone[type] && tile.type != 1;
                            if (!TileID.Sets.CanBeClearedDuringGeneration[tile.type])
                            {
                                flag3 = true;
                            }
                            switch (tile.type)
                            {
                                case 53:
                                    if (type == 59 && /*UndergroundDesertLocation.Contains(k, l)*/false)
                                    {
                                        flag3 = true;
                                    }
                                    if (type == 40)
                                    {
                                        flag3 = true;
                                    }
                                    if ((double) l < Main.worldSurface && type != 59)
                                    {
                                        flag3 = true;
                                    }
                                    break;
                                case 45:
                                case 147:
                                case 189:
                                case 190:
                                case 196:
                                case 460:
                                    flag3 = true;
                                    break;
                                case 396:
                                case 397:
                                    flag3 = !TileID.Sets.Ore[type];
                                    break;
                                case 1:
                                    if (type == 59 && (double) l < Main.worldSurface + (double) this.rand.Next(-50, 50))
                                    {
                                        flag3 = true;
                                    }
                                    break;
                                case 367:
                                case 368:
                                    if (type == 59)
                                    {
                                        flag3 = true;
                                    }
                                    break;
                            }
                            if (!flag3)
                            {
                                tile.type = (ushort) type;
                            }
                        }
                        if (addTile)
                        {
                            this.Coverage[k, l].active(active: true);
                            this.Coverage[k, l].liquid = 0;
                            this.Coverage[k, l].lava(lava: false);
                            if (killTile)
                            {
                                this.Coverage[k, l].active(false);
                            }
                        }
                        if (noYChange && (double) l < Main.worldSurface && type != 59)
                        {
                            this.Coverage[k, l].wall = 2;
                        }
                        if (type == 59 && l > this.waterLine && this.Coverage[k, l].liquid > 0)
                        {
                            this.Coverage[k, l].lava(lava: false);
                            this.Coverage[k, l].liquid = 0;
                        }
                    }
                }
                vector += vector2;
                if ((!false || this.rand.Next(3) != 0) && num > 50.0)
                {
                    vector += vector2;
                    num2 -= 1f;
                    vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                    vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                    if (num > 100.0)
                    {
                        vector += vector2;
                        num2 -= 1f;
                        vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                        vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                        if (num > 150.0)
                        {
                            vector += vector2;
                            num2 -= 1f;
                            vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                            vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                            if (num > 200.0)
                            {
                                vector += vector2;
                                num2 -= 1f;
                                vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                if (num > 250.0)
                                {
                                    vector += vector2;
                                    num2 -= 1f;
                                    vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                    vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                    if (num > 300.0)
                                    {
                                        vector += vector2;
                                        num2 -= 1f;
                                        vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                        vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                        if (num > 400.0)
                                        {
                                            vector += vector2;
                                            num2 -= 1f;
                                            vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                            vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                            if (num > 500.0)
                                            {
                                                vector += vector2;
                                                num2 -= 1f;
                                                vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                if (num > 600.0)
                                                {
                                                    vector += vector2;
                                                    num2 -= 1f;
                                                    vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                    vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                    if (num > 700.0)
                                                    {
                                                        vector += vector2;
                                                        num2 -= 1f;
                                                        vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                        vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                        if (num > 800.0)
                                                        {
                                                            vector += vector2;
                                                            num2 -= 1f;
                                                            vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                            vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                            if (num > 900.0)
                                                            {
                                                                vector += vector2;
                                                                num2 -= 1f;
                                                                vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                                vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                if (false)
                {
                    vector2.X += (float) this.rand.Next(-10, 11) * 0.25f;
                }
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (!noYChange)
                {
                    vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                else if (type != 59 && num < 3.0)
                {
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                if (type == 59 && !noYChange)
                {
                    if ((double) vector2.Y > 0.5)
                    {
                        vector2.Y = 0.5f;
                    }
                    if ((double) vector2.Y < -0.5)
                    {
                        vector2.Y = -0.5f;
                    }
                    if ((double) vector.Y < Main.rockLayer + 100.0)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector.Y > (float) (this.Coverage.Height - 300))
                    {
                        vector2.Y = -1f;
                    }
                }
            }
        }
        private void TileRunner(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool killTile = false, bool noYChange = false, bool overRide = true, int ignoreTileType = -1)
        {
            var mudWall = false;
            var beachDistance = int.MaxValue;

            var num = strength;
            float num2 = steps;
            var vector = default(Vector2);
            vector.X = i;
            vector.Y = j;
            var vector2 = default(Vector2);
            vector2.X = (float) this.rand.Next(-10, 11) * 0.1f;
            vector2.Y = (float) this.rand.Next(-10, 11) * 0.1f;
            if (speedX != 0f || speedY != 0f)
            {
                vector2.X = speedX;
                vector2.Y = speedY;
            }
            var flag = type == 368;
            var flag2 = type == 367;
            var lava = false;
            while (num > 0.0 && num2 > 0f)
            {
                if (vector.Y < 0f && num2 > 0f && type == 59)
                {
                    num2 = 0f;
                }
                num = strength;// * (double)(num2 / (float)steps);
                num2 -= 1f;
                var num3 = (int) ((double) vector.X - (num * 0.5));
                var num4 = (int) ((double) vector.X + (num * 0.5));
                var num5 = (int) ((double) vector.Y - (num * 0.5));
                var num6 = (int) ((double) vector.Y + (num * 0.5));
                if (num3 < 1)
                {
                    num3 = 1;
                }
                if (num4 > this.Coverage.Width - 1)
                {
                    num4 = this.Coverage.Width - 1;
                }
                if (num5 < 1)
                {
                    num5 = 1;
                }
                if (num6 > this.Coverage.Height - 1)
                {
                    num6 = this.Coverage.Height - 1;
                }
                for (var k = num3; k < num4; k++)
                {
                    if (k < beachDistance + 50 || k >= this.Coverage.Width - beachDistance - 50)
                    {
                        lava = false;
                    }
                    for (var l = num5; l < num6; l++)
                    {
                        if ((false && l < this.Coverage.Height - 300 && type == 57) || (ignoreTileType >= 0 && this.Coverage[k, l].active() && this.Coverage[k, l].type == ignoreTileType) || !((double) (Math.Abs((float) k - vector.X) + Math.Abs((float) l - vector.Y)) < strength * 0.5 * (1.0 + ((double) this.rand.Next(-10, 11) * 0.015))))
                        {
                            continue;
                        }
                        if (mudWall && (double) l > Main.worldSurface && this.Coverage[k, l - 1].wall != 2 && l < this.Coverage.Height - 210 - this.rand.Next(3) && (double) (Math.Abs((float) k - vector.X) + Math.Abs((float) l - vector.Y)) < strength * 0.45 * (1.0 + ((double) this.rand.Next(-10, 11) * 0.01)))
                        {
                            if (l > this.lavaLine - this.rand.Next(0, 4) - 50)
                            {
                                if (this.Coverage[k, l - 1].wall != 64 && this.Coverage[k, l + 1].wall != 64 && this.Coverage[k - 1, l].wall != 64 && this.Coverage[k + 1, l].wall != 64)
                                {
                                    this.PlaceWall(k, l, 15, mute: true);
                                }
                            }
                            else if (this.Coverage[k, l - 1].wall != 15 && this.Coverage[k, l + 1].wall != 15 && this.Coverage[k - 1, l].wall != 15 && this.Coverage[k + 1, l].wall != 15)
                            {
                                this.PlaceWall(k, l, 64, mute: true);
                            }
                        }
                        if (type < 0)
                        {
                            if (this.Coverage[k, l].type == 53)
                            {
                                continue;
                            }
                            if (type == -2 && this.Coverage[k, l].active() && (l < this.waterLine || l > this.lavaLine))
                            {
                                this.Coverage[k, l].liquid = byte.MaxValue;
                                this.Coverage[k, l].lava(lava);
                                if (l > this.lavaLine)
                                {
                                    this.Coverage[k, l].lava(lava: true);
                                }
                            }
                            this.Coverage[k, l].active(active: false);
                            continue;
                        }
                        if (flag && (double) (Math.Abs((float) k - vector.X) + Math.Abs((float) l - vector.Y)) < strength * 0.3 * (1.0 + ((double) this.rand.Next(-10, 11) * 0.01)))
                        {
                            this.PlaceWall(k, l, 180, mute: true);
                        }
                        if (flag2 && (double) (Math.Abs((float) k - vector.X) + Math.Abs((float) l - vector.Y)) < strength * 0.3 * (1.0 + ((double) this.rand.Next(-10, 11) * 0.01)))
                        {
                            this.PlaceWall(k, l, 178, mute: true);
                        }
                        if (overRide || !this.Coverage[k, l].active())
                        {
                            var tile = this.Coverage[k, l];
                            var flag3 = false;
                            flag3 = Main.tileStone[type] && tile.type != 1;
                            if (!TileID.Sets.CanBeClearedDuringGeneration[tile.type])
                            {
                                flag3 = true;
                            }
                            switch (tile.type)
                            {
                                case 53:
                                    if (type == 59 && /*UndergroundDesertLocation.Contains(k, l)*/false)
                                    {
                                        flag3 = true;
                                    }
                                    if (type == 40)
                                    {
                                        flag3 = true;
                                    }
                                    if ((double) l < Main.worldSurface && type != 59)
                                    {
                                        flag3 = true;
                                    }
                                    break;
                                case 45:
                                case 147:
                                case 189:
                                case 190:
                                case 196:
                                case 460:
                                    flag3 = true;
                                    break;
                                case 396:
                                case 397:
                                    flag3 = !TileID.Sets.Ore[type];
                                    break;
                                case 1:
                                    if (type == 59 && (double) l < Main.worldSurface + (double) this.rand.Next(-50, 50))
                                    {
                                        flag3 = true;
                                    }
                                    break;
                                case 367:
                                case 368:
                                    if (type == 59)
                                    {
                                        flag3 = true;
                                    }
                                    break;
                            }
                            if (!flag3)
                            {
                                tile.type = (ushort) type;
                            }
                        }
                        if (addTile)
                        {
                            this.Coverage[k, l].active(active: true);
                            this.Coverage[k, l].liquid = 0;
                            this.Coverage[k, l].lava(lava: false);
                            if (killTile)
                            {
                                this.Coverage[k, l].active(false);
                            }
                        }
                        if (noYChange && (double) l < Main.worldSurface && type != 59)
                        {
                            this.Coverage[k, l].wall = 2;
                        }
                        if (type == 59 && l > this.waterLine && this.Coverage[k, l].liquid > 0)
                        {
                            this.Coverage[k, l].lava(lava: false);
                            this.Coverage[k, l].liquid = 0;
                        }
                    }
                }
                vector += vector2;
                if ((!false || this.rand.Next(3) != 0) && num > 50.0)
                {
                    vector += vector2;
                    num2 -= 1f;
                    vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                    vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                    if (num > 100.0)
                    {
                        vector += vector2;
                        num2 -= 1f;
                        vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                        vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                        if (num > 150.0)
                        {
                            vector += vector2;
                            num2 -= 1f;
                            vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                            vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                            if (num > 200.0)
                            {
                                vector += vector2;
                                num2 -= 1f;
                                vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                if (num > 250.0)
                                {
                                    vector += vector2;
                                    num2 -= 1f;
                                    vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                    vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                    if (num > 300.0)
                                    {
                                        vector += vector2;
                                        num2 -= 1f;
                                        vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                        vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                        if (num > 400.0)
                                        {
                                            vector += vector2;
                                            num2 -= 1f;
                                            vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                            vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                            if (num > 500.0)
                                            {
                                                vector += vector2;
                                                num2 -= 1f;
                                                vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                if (num > 600.0)
                                                {
                                                    vector += vector2;
                                                    num2 -= 1f;
                                                    vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                    vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                    if (num > 700.0)
                                                    {
                                                        vector += vector2;
                                                        num2 -= 1f;
                                                        vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                        vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                        if (num > 800.0)
                                                        {
                                                            vector += vector2;
                                                            num2 -= 1f;
                                                            vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                            vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                            if (num > 900.0)
                                                            {
                                                                vector += vector2;
                                                                num2 -= 1f;
                                                                vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                                                                vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                vector2.X += (float) this.rand.Next(-10, 11) * 0.05f;
                if (false)
                {
                    vector2.X += (float) this.rand.Next(-10, 11) * 0.25f;
                }
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (!noYChange)
                {
                    vector2.Y += (float) this.rand.Next(-10, 11) * 0.05f;
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                else if (type != 59 && num < 3.0)
                {
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                if (type == 59 && !noYChange)
                {
                    if ((double) vector2.Y > 0.5)
                    {
                        vector2.Y = 0.5f;
                    }
                    if ((double) vector2.Y < -0.5)
                    {
                        vector2.Y = -0.5f;
                    }
                    if ((double) vector.Y < Main.rockLayer + 100.0)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector.Y > (float) (this.Coverage.Height - 300))
                    {
                        vector2.Y = -1f;
                    }
                }
            }
        }

        private void PlaceWall(int i, int j, ushort type, bool mute)
        {
            this.Coverage.PlaceWallAt(new Point(i, j), type);
        }
    }
}