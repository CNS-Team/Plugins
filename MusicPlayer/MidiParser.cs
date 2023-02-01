using System.Runtime.InteropServices;

namespace MusicPlayer;

public static class MidiParser
{
    private delegate void ReadMidiCallback(IntPtr mem, ref Midi midi);
    [DllImport("midiParser.dll")]
    private static extern void ReadMidi(string filename, ReadMidiCallback callback, ref Midi midi);
    [DllImport("midiParser.dll")]
    private static extern uint GetUInt(IntPtr mem, int index);
    [DllImport("midiParser.dll")]
    private static extern ushort GetUShort(IntPtr mem, int index);
    [DllImport("midiParser.dll")]
    private static extern byte GetByte(IntPtr mem, int index);
    
    private static void Callback(IntPtr mem, ref Midi res)
    {
        uint size = GetUInt(mem, 0);
        res.notes = new Note[size];
        for (int i = 0; i < size; ++i)
        {
            res.notes[i].time = GetUInt(mem, 12 * i + 4);
            res.notes[i].lasting = GetUShort(mem, 12 * i + 8);
            res.notes[i].instrument = GetByte(mem, 12 * i + 10);
            res.notes[i].pitch = GetByte(mem, 12 * i + 11);
            res.notes[i].velocity = GetByte(mem, 12 * i + 12);
        }
    }

    public static Midi ParseMidi(string filename)
    {
        Midi res = new Midi();
        ReadMidi(filename, Callback, ref res);
        return res;
    }
}