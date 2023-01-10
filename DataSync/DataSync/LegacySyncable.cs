using System;

namespace DataSync
{
	// Token: 0x02000003 RID: 3
	public sealed class LegacySyncable<T> : Syncable<T> where T : IEquatable<T>
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000003 RID: 3 RVA: 0x0000206D File Offset: 0x0000026D
		protected sealed override T t
		{
			get
			{
				return this.GetValue();
			}
			set
			{
				this.SetValue(value);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000207D File Offset: 0x0000027D
		protected sealed override int interval
		{
			get
			{
				return this.Interval;
			}
		}

		// Token: 0x04000001 RID: 1
		public Func<T> GetValue;

		// Token: 0x04000002 RID: 2
		public Action<T> SetValue;

		// Token: 0x04000003 RID: 3
		public int Interval;
	}
}
