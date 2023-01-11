using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusTxtMgr.Utils
{
    public enum LogLevel
    {
        NONE,
        ERROR,
        WARNING,
        INFO,
        DEBUG,
    }

    internal static class Logger
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> level2Color = new Dictionary<LogLevel, ConsoleColor>()
        {
            { LogLevel.NONE,    ConsoleColor.White },
            { LogLevel.ERROR,   ConsoleColor.Red },
            { LogLevel.WARNING, ConsoleColor.Yellow },
            { LogLevel.INFO,    ConsoleColor.White },
            { LogLevel.DEBUG,   ConsoleColor.Green }
        };

        private static readonly object consoleWriteLock = new object();

        public static void Err(string msg) => Log(msg, LogLevel.ERROR);
        public static void Warn(string msg) => Log(msg, LogLevel.WARNING);
        public static void Info(string msg) => Log(msg, LogLevel.INFO);
        public static void Debug(string msg) => Log(msg, LogLevel.DEBUG);

        public static void Log(string msg, LogLevel level = LogLevel.DEBUG)
        {
            if (level > StatusTxtMgr.Settings.LogLevel)
                return;
            lock (consoleWriteLock)
            {
                Console.ForegroundColor = level2Color[level];
                Console.WriteLine($"[StatusTxtMgr] [{level:G}] {msg}");
                Console.ResetColor();
            }
        }
    }
}
