using LazyUtils;
using LinqToDB.Mapping;

namespace PChrome.Core;

public class Money : PlayerConfigBase<Money>
{
    [Column]
    public int money { get; set; }
}