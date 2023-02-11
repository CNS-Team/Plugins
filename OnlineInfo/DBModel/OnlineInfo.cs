using LinqToDB.Mapping;

namespace OnlineInfo.DBModel;

[Table("OnlineInfo")]
internal class OnlineInfo
{
    [Column(IsPrimaryKey = true)]
    public int ServerId { get; set; }

    [Column(DataType = LinqToDB.DataType.VarChar, Length = 256)]
    public string? ServerName { get; set; }

    [Column(DataType = LinqToDB.DataType.Text)]
    public string? Players { get; set; }

    [Column]
    public int PlayerCount { get; set; }

    public override string ToString()
    {
        return
            $"ID: {this.ServerId}\n" +
            $"名称: {this.ServerName}\n" +
            $"人数: {this.PlayerCount}\n" +
            $"玩家: {this.Players}";
    }
}