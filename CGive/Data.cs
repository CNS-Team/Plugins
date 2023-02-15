using System;
using TShockAPI;
using TShockAPI.DB;

namespace CGive;

public class Data
{
	public static void Init()
	{
		DbExt.Query(TShock.DB, "create table if not exists CGive(executer text,cmd text,who text,id int(32))", Array.Empty<object>());
		Command("create table if not exists Given(name text,id int(32))");
	}

	public static void Command(string cmd)
	{
		DbExt.Query(TShock.DB, cmd, Array.Empty<object>());
	}
}
