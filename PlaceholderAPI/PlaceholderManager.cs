using System;
using System.Collections.Generic;
using TShockAPI;

namespace PlaceholderAPI;

public class PlaceholderManager
{
	private Dictionary<string, string> placeholders;

	public PlaceholderManager()
	{
		placeholders = new Dictionary<string, string>();
	}

	public string GetText(string text, TSPlayer player)
	{
		Hooks.OnGetText(placeholders, player);
		foreach (string key in placeholders.Keys)
		{
			text = text.Replace(key, placeholders[key]);
		}
		return text;
	}

	public void Register(string key)
	{
		if (!placeholders.ContainsKey(key))
		{
			placeholders.Add(key, "");
		}
		else
		{
			Console.WriteLine("[PlaceholderAPI] 占位符 " + key + " 注册冲突");
		}
	}

	public void Deregister(string key)
	{
		if (placeholders.ContainsKey(key))
		{
			placeholders.Remove(key);
		}
	}
}
