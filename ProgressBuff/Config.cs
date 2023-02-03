﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace ProgressBuff
{
    public class Scheme
    {
        public string SchemeName = "默认方案";
        public List<string> SkipProgressDetection = new List<string>();
        public List<string> SkipRemoteDetection = new List<string>();
        public Dictionary<string, List<int>> ProgressBuff = new Dictionary<string, List<int>>();
    }
    public class Config
    {
        public string UseScheme = "默认方案";
        public bool Enabled = true;
        public bool Broadcast = true;
        public bool WriteLog = true;
        public bool ClearBuff = true;
        public List<Scheme> Schemes = new List<Scheme>();



        public static Config Read(string Path)//给定文件进行读
        {
            if (!File.Exists(Path)) return new Config();
            using (var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            { return Read(fs); }
        }
        public static Config Read(Stream stream)//给定流文件进行读取
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                if (ConfigR != null)
                    ConfigR(cf);
                return cf;
            }
        }
        public void Write(string Path)//给定路径进行写
        {
            using (var fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write))
            { Write(fs); }
        }
        public void Write(Stream stream)//给定流文件写
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            { sw.Write(str); }
        }
        public static Action<Config> ConfigR;//定义为常量
    }
}