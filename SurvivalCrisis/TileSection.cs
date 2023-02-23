using Microsoft.Xna.Framework;
// using OTAPI.Tile;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis
{
    /// <summary>
    /// 表示一个方块区
    /// </summary>
    public readonly struct TileSection
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public int Left => this.X;
        public int Right => this.X + this.Width;
        public int Top => this.Y;
        public int Bottom => this.Y + this.Height;

        public Point LeftTop => new Point(this.Left, this.Top);
        public Point LeftBottom => new Point(this.Left, this.Bottom);
        public Point RightTop => new Point(this.Right, this.Top);
        public Point RightBottom => new Point(this.Right, this.Bottom);

        public Point CenterBottom => new Point(this.CenterX, this.Bottom);

        public int CenterX => this.X + (this.Width / 2);
        public int CenterY => this.Y + (this.Height / 2);

        public Point Center => new Point(this.CenterX, this.CenterY);

        public int Size => this.Width * this.Height;

        public ITile this[int x, int y]
        {
            get
            {
#if DEBUG
                System.Diagnostics.Debug.Assert(0 <= x && x < this.Width, "x out of range");
                System.Diagnostics.Debug.Assert(0 <= y && y < this.Height, "y out of range");
#endif
                return Main.tile[this.X + x, this.Y + y];
            }
            set
            {
#if DEBUG
                System.Diagnostics.Debug.Assert(0 <= x && x < this.Width, "x out of range");
                System.Diagnostics.Debug.Assert(0 <= y && y < this.Height, "y out of range");
#endif
                Main.tile[this.X + x, this.Y + y] = value;
            }
        }
        public ITile this[Point point]
        {
            get => this[point.X, point.Y];
            set => this[point.X, point.Y] = value;
        }

        public TileSection(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clover_s">[0] x1 [1] y1 [2] x2 [3] y2</param>
        public TileSection(int[] clover_s) : this(clover_s[0], clover_s[1], clover_s[2] - clover_s[0], clover_s[3] - clover_s[1])
        {

        }

        public TileSection(Point point) : this(point.X, point.Y, 1, 1)
        {

        }

        public int CountPlayers(bool ignoreWatcher = false)
        {
            var count = 0;
            foreach (var player in TShock.Players)
            {
                if (player?.Active ?? false)
                {
                    if (this.InRange(player) && (!ignoreWatcher || SurvivalCrisis.Instance.Players[player.Index].Identity != PlayerIdentity.Watcher))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void UpdateToPlayer(int who = -1)
        {
            Replenisher.UpdateSection(this.X, this.Y, this.X + this.Width, this.Y + this.Height, who);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">这是物块坐标系里的坐标</param>
        /// <returns></returns>
        public bool InRange(Point point)
        {
            var Dx = point.X - this.X;
            var Dy = point.Y - this.Y;
            return
                0 <= Dx && Dx < this.Width &&
                0 <= Dy && Dy < this.Height;
        }
        public bool InRange(int x, int y)
        {
            return this.InRange(new Point(x, y));
        }
        public bool InRange(in TileSection sec)
        {
            return
                this.Left <= sec.Left && sec.Right <= this.Right &&
                this.Top <= sec.Top && sec.Bottom <= this.Bottom;
        }
        public bool InRange(Rectangle rect)
        {
            var point = rect.Center;
            var Dx = point.X - this.X;
            var Dy = point.Y - this.Y;
            return
                rect.Width / 2 <= Dx && Dx <= this.Width - (rect.Width / 2) &&
                rect.Height / 2 <= Dy && Dy <= this.Height - (rect.Height / 2);
        }
        public bool InRange(Entity entity)
        {
            var rect = entity.Hitbox;
            rect.X /= 16;
            rect.Y /= 16;
            rect.Width /= 16;
            rect.Height /= 16;
            return this.InRange(rect);
        }
        public bool InRange(Chest chest)
        {
            return this.InRange(chest.x, chest.y);
        }
        public bool InRange(TSPlayer player)
        {
            return this.InRange(player.TPlayer);
        }
        public bool InRange(GamePlayer player)
        {
            return this.InRange(player.TPlayer);
        }

        public bool Intersect(in TileSection sec)
        {
            return
                Math.Abs(this.CenterX - sec.CenterX) < (this.Width / 2) + (sec.Width / 2) &&
                Math.Abs(this.CenterY - sec.CenterY) < (this.Height / 2) + (sec.Height / 2);
        }

        /// <summary>
        /// 填充一层
        /// </summary>
        /// <param name="height">填充层的高度(0到Height)</param>
        /// <param name="type">物块ID</param>
        public void FillLine(int height, ushort type)
        {
            Replenisher.SpecialLine(this.X, this.Y + height, this.Width, type);
        }

        public void PlaceTileAt(int i, int j, ushort type, bool netUpdate = false)
        {
            this.PlaceTileAt(new Point(i, j), type, netUpdate);
        }
        public void PlaceTileAt(Point point, ushort type, bool netUpdate = false)
        {
            point.X += this.X;
            point.Y += this.Y;
            if (netUpdate)
            {
                Replenisher.PlaceTileAndUpdate(point, type);
            }
            else
            {
                WorldGen.PlaceTile(point.X, point.Y, type);
            }
        }
        public void PlaceWallAt(int i, int j, ushort type, bool netUpdate = false)
        {
            this.PlaceWallAt(new Point(i, j), type, netUpdate);
        }
        public void PlaceWallAt(Point point, ushort type, bool netUpdate = false)
        {
            point.X += this.X;
            point.Y += this.Y;
            if (netUpdate)
            {
                Replenisher.PlaceWallAndUpdate(point, type);
            }
            else
            {
                WorldGen.PlaceWall(point.X, point.Y, type);
            }
        }

        public void TryRemoveTile(int i, int j)
        {
            if (i < 0 || j < 0 || i >= this.Width || j >= this.Height)
            {
                return;
            }
            this[i, j].active(false);
        }

        public void TryPlaceWall(int i, int j, ushort type)
        {
            if (i < 0 || j < 0 || i >= this.Width || j >= this.Height)
            {
                return;
            }
            this[i, j].wall = type;
        }


        /*
		public void LoadFile(string name)
		{
			string path = Path.Combine(Environment.CurrentDirectory, "hunger", "maps", name);
			using var stream = new BufferedStream(new GZipStream(File.Open(path, FileMode.Open), CompressionMode.Decompress), 1048576);
			using var reader = new BinaryReader(stream);
			int width = reader.ReadInt32();
			int height = reader.ReadInt32();
#if DEBUG
			System.Diagnostics.Debug.Assert(width == Width, $"width({width}) != Width({Width})");
			System.Diagnostics.Debug.Assert(height == Height, $"height({height}) != Height({Height})");
#endif
			for (int i = X; i < X + width; i++)
			{
				for (int j = Y; j < Y + height; j++)
				{
					Main.tile[i, j] = reader.ReadTile();
					Main.tile[i, j].skipLiquid(true);
				}
			}
		}
		public void SaveFile(string name)
		{
			string path = Path.Combine(Environment.CurrentDirectory, "hunger", "maps", name);

			using (var stream = new GZipStream(File.OpenWrite(path), CompressionMode.Compress))
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write(Width);
				writer.Write(Height);
				for (int i = 0; i < Width; i++)
				{
					for (int j = 0; j < Height; j++)
					{
						writer.Write(this[i, j]);
					}
				}
			}
		}
		*/
        /// <summary>
        /// 检测是否可以容纳Entity
        /// </summary>
        /// <param name="point">相对Section左上角的方块坐标</param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CanPlaceEntity(Point point, Entity entity)
        {
            var x = (int) Math.Ceiling(entity.width / 16.0);
            var y = (int) Math.Ceiling(entity.height / 16.0);
            if (point.X + x > this.Width)
            {
                return false;
            }
            if (point.Y + y > this.Height)
            {
                return false;
            }
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    if (this[point.X + i, point.Y + j].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public TileSection SubSection(int x, int y, int width, int height)
        {
            return new TileSection(this.X + x, this.Y + y, width, height);
        }
        public TileSection SubSection(Rectangle rect)
        {
            return this.SubSection(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void KillTiles(int x, int y, int width, int height)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(0 <= x && x < this.Width, "x out of range");
            System.Diagnostics.Debug.Assert(0 <= y && y < this.Height, "y out of range");
            System.Diagnostics.Debug.Assert(x + width <= this.Width, "x + width > Width");
            System.Diagnostics.Debug.Assert(this.Y + height <= this.Height, "y + height > Height");
#endif
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    WorldGen.KillTile(this.X + x + i, this.Y + y + j);
                }
            }
        }
        public void KillTiles(Rectangle rect)
        {
            this.KillTiles(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void KillAllTile()
        {
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    WorldGen.KillTile(this.X + i, this.Y + j);
                }
            }
        }
        public void TurnToAir()
        {
            for (var i = 0; i < Main.chest.Length; i++)
            {
                var chest = Main.chest[i];
                if (chest != null && this.InRange(chest))
                {
                    Main.chest[i] = null;
                }
            }
            for (var i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && this.InRange(npc))
                {
                    npc.type = 0;
                    npc.active = false;
                }
            }
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this[i, j].active(false);
                    this[i, j].wall = WallID.None;
                    this[i, j].liquid = 0;
                    this[i, j].color(0);
                    this[i, j].wallColor(0);
                }
            }
        }

        public void Fill(ushort tileType, ushort wallType = WallID.None)
        {
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this[i, j].type = tileType;
                    this[i, j].active(true);
                    this[i, j].slope(0);
                    this[i, j].wall = wallType;
                }
            }
        }

        public static bool operator ==(in TileSection left, in TileSection right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y &&
                left.Width == right.Width &&
                left.Height == right.Height;
        }
        public static bool operator !=(in TileSection left, in TileSection right) => !(left == right);



        public Point LeftJoint => new Point(this.Left, this.CenterY);
        public Point RightJoint => new Point(this.Right, this.CenterY);
        public Point RandomJoint(Random rad)
        {
            return (rad.NextDouble() < 0.5) ? this.LeftJoint : this.RightJoint;
        }

        public string ToString2()
        {
            return $"({this.X}, {this.Y}) Width({this.Width}) Height({this.Height})";
        }
        public override string ToString()
        {
            return $"{{ Left: {this.Left}, Right: {this.Right}, Top: {this.Top}, Bottom: {this.Bottom} }}";
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }
    }
}