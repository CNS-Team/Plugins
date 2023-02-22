using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace SurvivalCrisis.MapGenerating
{
    using TileBlockData = IslandsGenerator.TileBlockData;

    public class SpheresGenerator : Generator
    {
        private readonly Random rand;
        private TileBlockData light;
        private List<TileBlockData> spheresLarge;
        private List<TileBlockData> spheres;
        private List<TileBlockData> hungerIslands;
        private readonly List<TileSection> placed;
        public SpheresGenerator() : base(SurvivalCrisis.Regions.Spheres)
        {
            this.rand = new Random();
            this.placed = new List<TileSection>(40);
            this.LoadSpheres();
        }

        public override void Generate()
        {
            SurvivalCrisis.DebugMessage("生成中空层...");
            this.PlaceSpheresLarge();
            this.PlaceHungerIslands();
            this.PlaceSpheres();
            this.PlaceLights();
            for (var i = 0; i < this.Coverage.Width; i++)
            {
                for (var j = 0; j < this.Coverage.Height; j++)
                {
                    if (this.Coverage[i, j].wall == WallID.None)
                    {
                        this.Coverage[i, j].wall = WallID.StarsWallpaper;
                    }
                }
            }
            SurvivalCrisis.DebugMessage("中空层生成成功");
        }

        private void PlaceSpheresLarge()
        {
            // 球的直径
            const double d = 205.0;
            //int h = (int)Math.Ceiling((spheresLarge.Count * 4 - 3 + 1) * d / Coverage.Width);
            //int g = (int)Math.Floor((Coverage.Width - 2 * d) / 4 * d);
            //List<Point> points = new List<Point>(h * g);
            //Point current = new Point((int)(d * 3 / 2), (int)(d * 3 / 2));
            //for (int i = 0; i < h; i++)
            //{
            //	current.X = (int)(d * 3 / 2);
            //	for (int j = 0; j < g; j++)
            //	{
            //		var p = current;
            //		p.X += rand.Next(-20, 20);
            //		p.Y += rand.Next(-10, 10);
            //		points.Add(p);
            //		current.X += (int)(4 * d);
            //	}
            //	current.Y += (int)(2 * d);
            //}
            var points = (Point[]) SurvivalCrisis.Instance.Config.SpheresLarge.Clone();
            this.rand.Shuffle(points);

            for (var i = 0; i < this.spheresLarge.Count; i++)
            {
                var sphereLarge = this.spheresLarge[i];
                var p = points[i];
                // var section = Coverage.SubSection((int)(p.X - d / 2), (int)(p.Y - d / 2), (int)Math.Ceiling(d), (int)Math.Ceiling(d));
                var section = new TileSection(p.X, p.Y, sphereLarge.Width, sphereLarge.Height);
                sphereLarge.AffixTo(section);
                ChestLevel.V1.Generate(section);
                ChestLevel.V2.Generate(section);
                ChestLevel.V3.Generate(section);
                ChestLevel.V4.Generate(section);
                this.placed.Add(section);
            }
        }
        private void PlaceHungerIslands()
        {
            foreach (var island in this.hungerIslands)
            {
                bool canPlace;
                TileSection section;
                do
                {
                    var x = this.rand.Next(this.Coverage.Left, this.Coverage.Right);
                    var y = this.rand.Next(this.Coverage.Top, this.Coverage.Bottom);
                    section = new TileSection(x, y, island.Width, island.Height);
                    canPlace = this.Coverage.InRange(section) && this.placed.Count(v => v.Intersect(section)) == 0;
                }
                while (!canPlace);
                island.AffixTo(section);
                this.placed.Add(section);
            }
        }
        private void PlaceSpheres()
        {
            var maxSpheres = this.rand.Next(100, 200);
            for (var i = 0; i < maxSpheres; i++)
            {
                var sphere = this.rand.Next(this.spheres);
                bool canPlace;
                TileSection section;
                do
                {
                    var x = this.rand.Next(this.Coverage.Left, this.Coverage.Right);
                    var y = this.rand.Next(this.Coverage.Top, this.Coverage.Bottom);
                    section = new TileSection(x, y, sphere.Width, sphere.Height);
                    canPlace = this.Coverage.InRange(section) && this.placed.Count(v => v.Intersect(section)) == 0;
                }
                while (!canPlace);
                sphere.AffixTo(section);
                if (sphere.Identifier.Contains("V1"))
                {
                    ChestLevel.V1.Generate(section);
                }
                if (sphere.Identifier.Contains("V2"))
                {
                    ChestLevel.V2.Generate(section);
                }
                if (sphere.Identifier.Contains("V3"))
                {
                    ChestLevel.V3.Generate(section);
                }
                if (sphere.Identifier.Contains("V4"))
                {
                    ChestLevel.V4.Generate(section);
                }
                this.placed.Add(section);
            }
        }
        private void PlaceLights()
        {
            var max = this.rand.Next(250, 350);
            for (var i = 0; i < max; i++)
            {
                bool canPlace;
                TileSection section;
                do
                {
                    var x = this.rand.Next(this.Coverage.Left, this.Coverage.Right);
                    var y = this.rand.Next(this.Coverage.Top, this.Coverage.Bottom);
                    section = new TileSection(x, y, this.light.Width, this.light.Height);
                    canPlace = this.Coverage.InRange(section) && this.placed.Count(v => v.Intersect(section)) == 0;
                }
                while (!canPlace);
                this.light.AffixTo(section);
                this.placed.Add(section);
            }
        }

        private void LoadSpheres()
        {
            var directory = new DirectoryInfo(SurvivalCrisis.SpheresPath);
            {
                this.spheresLarge = new List<TileBlockData>();
                var files = directory.GetFiles("*Large.sec");
                foreach (var file in files)
                {
                    try
                    {
                        this.spheresLarge.Add(TileBlockData.FromFile(file.FullName));
                    }
                    catch (Exception e)
                    {
                        SurvivalCrisis.DebugMessage($"文件{file.Name}读取失败:");
                        SurvivalCrisis.DebugMessage($"{e}");
                    }
                }
            }
            {
                this.spheres = new List<TileBlockData>();
                var files = directory.GetFiles("*.sec");
                foreach (var file in files)
                {
                    if (file.Name.Contains("Light") || file.Name.Contains("Large") || file.Name.Contains("Hunger"))
                    {
                        continue;
                    }
                    try
                    {
                        this.spheres.Add(TileBlockData.FromFile(file.FullName));
                    }
                    catch (Exception e)
                    {
                        SurvivalCrisis.DebugMessage($"文件{file.Name}读取失败:");
                        SurvivalCrisis.DebugMessage($"{e}");
                    }
                }
            }
            {
                this.hungerIslands = new List<TileBlockData>();
                var files = directory.GetFiles("Hunger*.sec");
                foreach (var file in files)
                {
                    try
                    {
                        this.hungerIslands.Add(TileBlockData.FromFile(file.FullName));
                    }
                    catch (Exception e)
                    {
                        SurvivalCrisis.DebugMessage($"文件{file.Name}读取失败:");
                        SurvivalCrisis.DebugMessage($"{e}");
                    }
                }
            }
            this.light = TileBlockData.FromFile(directory.GetFiles("Light.sec").First().FullName);
        }
    }
}