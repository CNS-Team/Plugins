using System;
using System.Collections.Generic;
using TShockAPI;

namespace PlaceholderAPI;

public class PlaceholderManager
{
    private Dictionary<string, string> placeholders;

    public PlaceholderManager()
    {
        this.placeholders = new Dictionary<string, string>();
    }

    public string GetText(string text, TSPlayer player)
    {
        Hooks.OnGetText(this.placeholders, player);
        foreach (var key in this.placeholders.Keys)
        {
            text = text.Replace(key, this.placeholders[key]);
        }

        return text;
    }

    public void Register(string key)
    {
        if (!this.placeholders.ContainsKey(key))
        {
            this.placeholders.Add(key, "");
        }
        else
        {
            Console.WriteLine("[PlaceholderAPI] 占位符 " + key + " 注册冲突");
        }
    }

    public void Deregister(string key)
    {
        if (this.placeholders.ContainsKey(key))
        {
            this.placeholders.Remove(key);
        }
    }
}