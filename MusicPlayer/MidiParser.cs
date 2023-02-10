using System.Runtime.InteropServices;

namespace MusicPlayer;

public static class MidiParser
{
    private delegate void ReadMidiCallback(IntPtr mem, IntPtr nul);
    [DllImport("midiParser.dll", EntryPoint = "readMidi")]

    private static extern void ReadMidi(string filename, ReadMidiCallback callback, IntPtr nul);
    [DllImport("midiParser.dll", EntryPoint = "getUInt")]
    private static extern uint GetUInt(IntPtr mem, int index);
    [DllImport("midiParser.dll", EntryPoint = "getUShort")]
    private static extern ushort GetUShort(IntPtr mem, int index);
    [DllImport("midiParser.dll", EntryPoint = "getByte")]
    private static extern byte GetByte(IntPtr mem, int index);

    public static Midi ParseMidi(string filename)
    {
        var res = new Midi();

        void Callback(IntPtr mem, IntPtr nul)
        {
            var size = GetUInt(mem, 0);
            res.notes = new Note[size];
            for (var i = 0; i < size; ++i)
            {
                res.notes[i].time = GetUInt(mem, (12 * i) + 4);
                res.notes[i].lasting = GetUShort(mem, (12 * i) + 8);
                res.notes[i].instrument = GetByte(mem, (12 * i) + 10);
                res.notes[i].pitch = GetByte(mem, (12 * i) + 11);
                res.notes[i].velocity = GetByte(mem, (12 * i) + 12);
            }
        }

        ReadMidi(filename, Callback, IntPtr.Zero);
        return res;
    }
}