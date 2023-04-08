using TShockAPI;
using TShockAPI.DB;

namespace CGive;

public class CGive
{
    public string Executer { get; set; }

    public string cmd { get; set; }

    public string who { get; set; }

    public int id { get; set; }

    public bool Execute()
    {
        if (who == "-1")
        {
            Save();
            List<TSPlayer> list = TSPlayer.FindByNameOrID(Executer);
            if (list.Count > 0 || Executer.ToLower() == "server")
            {
                TSPlayer[] players = TShock.Players;
                TSPlayer[] array = players;
                TSPlayer[] array2 = array;
                foreach (TSPlayer tSPlayer in array2)
                {
                    if (tSPlayer != null && tSPlayer.Active)
                    {
                        if (Executer.ToLower() == "server")
                        {
                            Commands.HandleCommand(TSPlayer.Server, cmd.Replace("name", tSPlayer.Name));
                        }
                        else
                        {
                            Commands.HandleCommand(list[0], cmd.Replace("name", tSPlayer.Name));
                        }
                        Given given = new Given();
                        given.Name = tSPlayer.Name;
                        given.id = id;
                        given.Save();
                    }
                }
                return true;
            }
            return false;
        }
        List<TSPlayer> list2 = TSPlayer.FindByNameOrID(who);
        if (list2.Count > 0)
        {
            List<TSPlayer> list3 = TSPlayer.FindByNameOrID(Executer);
            if (list3.Count > 0 || Executer.ToLower() == "server")
            {
                if (Executer.ToLower() == "server")
                {
                    Commands.HandleCommand(TSPlayer.Server, cmd.Replace("name", who));
                }
                else
                {
                    Commands.HandleCommand(list3[0], cmd.Replace("name", who));
                }
                return true;
            }
            return false;
        }
        return false;
    }

    public static List<CGive> GetCGive(string who)
    {
        List<CGive> list = new List<CGive>();
        using (QueryResult queryResult = TShock.DB.QueryReader("select executer,cmd,who,id from CGive where who='" + who + "' or who=='-1'"))
        {
            while (queryResult.Read())
            {
                list.Add(new CGive
                {
                    Executer = queryResult.Reader.GetString(0),
                    cmd = queryResult.Reader.GetString(1),
                    who = who,
                    id = queryResult.Reader.GetInt32(3)
                });
            }
        }
        return list;
    }

    public static IEnumerable<CGive> GetCGive()
    {
        using QueryResult re = TShock.DB.QueryReader("select executer,cmd,who,id from CGive");
        while (re.Read())
        {
            yield return new CGive
            {
                Executer = re.Reader.GetString(0),
                cmd = re.Reader.GetString(1),
                who = re.Reader.GetString(2),
                id = re.Reader.GetInt32(3)
            };
        }
    }

    public void Save()
    {
        int num = 0;
        foreach (CGive item in GetCGive())
        {
            if (num < item.id)
            {
                num = item.id;
            }
        }
        num++;
        Data.Command($"insert into CGive(executer,cmd,who,id)values('{Executer}','{cmd}','{who}',{num})");
    }

    public void Del()
    {
        Data.Command($"delete from CGive where id={id}");
    }
}
