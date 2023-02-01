using LazyUtils;
using LazyUtils.Commands;
using LinqToDB.Tools;
using TShockAPI;

namespace MusicPlayer;

[Command("playmidi")]
public static class Commands
{
    [Main]
    [RealPlayer]
    [Permissions("musicplayer.playother")]
    public static void Main(CommandArgs args, TSPlayer player, string fileName)
    {
        var midi = MidiParser.ParseMidi(fileName);
        midi.PlayFor(player.Index);
    }
    
    [Main]
    [RealPlayer]
    [Permissions("musicplayer.play")]
    public static void Main(CommandArgs args, string fileName)
    {
        Main(args, args.Player, fileName);
    }
}