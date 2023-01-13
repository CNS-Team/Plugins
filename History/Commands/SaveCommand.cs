using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using static MonoMod.InlineRT.MonoModRule;
using static System.Net.Mime.MediaTypeNames;

namespace History.Commands
{
    public class SaveCommand : HCommand
    {
        private readonly Action[] actions;

        public SaveCommand(Action[] actions)
        : base(null)
        {
            this.actions = actions;
        }

        public override void Execute()
        {
            foreach (var a in this.actions)
            {
                History.Database.Query("INSERT INTO History(Time, Account, Action, XY, Data, Style, Paint, WorldID, Text, Alternate, Random, Direction) VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11)",
                    a.time, a.account, a.action, (a.x << 16) + a.y, a.data, a.style, a.paint, Main.worldID, a.text, a.alt, a.random, a.direction ? 1 : -1);
            }
        }
    }
}