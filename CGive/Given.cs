using System;
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
		QueryResult val = DbExt.QueryReader(dB, $"select * from Given where name='{Name}' and id={id}", Array.Empty<object>());
		try
		{
			result = val.Read();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return result;
	}
}
