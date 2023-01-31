using TShockAPI;
using TShockAPI.DB;

namespace Chrome.EntryCondition;

public static class DB
{
    public static string QueryGrade(string 玩家名)
    {
        var 职业 = "";
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名))
        {
            if (表.Read())
            {
                职业 = 表.Get<string>("职业");
            }
        }
        return 职业;
    }
}
