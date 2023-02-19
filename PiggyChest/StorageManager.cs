using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiggyChest;

public class StorageManager
{
    public string StoragePath { get; }
    public StorageManager(Config config)
    {
        this.StoragePath = config.StoragePath;
        this.StoragePath.EnsureDirectoryExists();
    }
    public List<ItemInfo> GetBankItems(int userID, string chestName)
    {
        var userPath = Path.Combine(this.StoragePath, userID.ToString());
        userPath.EnsureDirectoryExists();
        var chestPath = Path.Combine(userPath, chestName + ".txt");
        if (!File.Exists(chestPath))
        {
            var list = new List<ItemInfo>();
            this.SaveBankItems(userID, chestName, list);
            return list;
        }
        else
        {
            var text = File.ReadAllText(chestPath);
            return ItemInfo.ParseList(text);
        }
    }
    public void SaveBankItems(int userID, string chestName, List<ItemInfo> items)
    {
        var userPath = Path.Combine(this.StoragePath, userID.ToString());
        userPath.EnsureDirectoryExists();
        var chestPath = Path.Combine(userPath, chestName + ".txt");
        File.WriteAllText(chestPath, items.ToText());
    }
}
