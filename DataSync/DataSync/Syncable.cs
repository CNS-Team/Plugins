using System;
using System.Data;
using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.DB;

namespace DataSync
{
	// Token: 0x02000005 RID: 5
	public abstract class Syncable<T> : ISyncable where T : IEquatable<T>
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12
		// (set) Token: 0x0600000D RID: 13
		protected abstract T t { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000021F8 File Offset: 0x000003F8
		protected virtual int interval
		{
			get
			{
				return 600;
			}
		}

		// Token: 0x17000006 RID: 6
		private T this[string key]
		{
			get
			{
				T result;
				using (QueryResult queryResult = DbExt.QueryReader(Syncable<T>.db, "select * from synctable where `key`=@0", new object[]
				{
					key
				}))
				{
					bool flag = queryResult.Read();
					if (flag)
					{
						result = JsonConvert.DeserializeObject<T>(queryResult.Get<string>("value"));
					}
					else
					{
						result = default(T);
					}
				}
				return result;
			}
			set
			{
				string text = JsonConvert.SerializeObject(value);
				using (QueryResult queryResult = DbExt.QueryReader(Syncable<T>.db, "select * from synctable where `key`=@0", new object[]
				{
					key
				}))
				{
					bool flag = queryResult.Read();
					if (flag)
					{
						DbExt.Query(Syncable<T>.db, "update synctable set `value`=@1 where `key`=@0", new object[]
						{
							key,
							text
						});
					}
					else
					{
						DbExt.Query(Syncable<T>.db, "insert into synctable (`key`, `value`) values (@0, @1)", new object[]
						{
							key,
							text
						});
					}
				}
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002310 File Offset: 0x00000510
		protected virtual T Merge(T t1, T t2)
		{
			IComparable<T> comparable = t1 as IComparable<T>;
			bool flag = comparable != null;
			if (flag)
			{
				return (comparable.CompareTo(t2) > 0) ? t1 : t2;
			}
			throw new NotImplementedException();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000234C File Offset: 0x0000054C
		public void CheckSync(string key)
		{
			int num = this._count + 1;
			this._count = num;
			bool flag = num == this.interval;
			if (flag)
			{
				T t = this[key] = (this.t = this.Merge(this.t, this[key]));
				this._count = 0;
			}
		}

		// Token: 0x04000005 RID: 5
		private static readonly IDbConnection db = TShock.DB;

		// Token: 0x04000006 RID: 6
		private int _count = 0;
	}
}
