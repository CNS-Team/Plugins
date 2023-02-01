using LazyUtils;
using Terraria;
using Terraria.ID;

namespace MusicPlayer;

public struct Midi
{
    public Note[] notes;

    public void PlayFor(int index)
    {
        foreach (var note in notes)
        {
            var pitch = (note.pitch - 60) / 12f;
            if (pitch is > 1f or < -1f) continue;
            TimingUtils.Delayed((int)note.time + 60, () =>
            {
                NetMessage.SendData(MessageID.InstrumentSound, index,
                    number: index, number2: pitch);
            });
        }
    }
}