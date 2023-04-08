using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace CGive;

public class Given
{
    public string Name { get; set; }

    public int id { get; set; }

    public void Save()
    {
        Data.Command($"insert into Given(name,id)values('{Name}',{id})");
    }

    public void Del()
    {
        Data.Command($"delete from Given where name='{Name}' and id={id}");
    }

    public bool IsGiven()
    {
        bool result = false;
        IDbConnection dB = TShock.DB;
        using (QueryResult queryResult = dB.QueryReader($"select * from Given where name='{Name}' and id={id}"))
        {
            result = queryResult.Read();
        }
        return result;
    }
}
