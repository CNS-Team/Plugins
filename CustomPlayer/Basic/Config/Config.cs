using System.Diagnostics;

using TShockAPI;

using Newtonsoft.Json;

using VBY.Basic.Command;

namespace VBY.Basic.Config;

public class MainRoot
{
    public Command Commands = new();
}
public class Command
{
    public CommandInfo Use = new();
    public CommandInfo Admin = new();
    public TShockAPI.Command[] GetCommands(CommandDelegate use, CommandDelegate admin) => new TShockAPI.Command[] { Use.GetCommand(use), Admin.GetCommand(admin) };
    public TShockAPI.Command[] GetCommands(SubCmdRoot use, SubCmdRoot admin) => new TShockAPI.Command[] { use.GetCommand(Use.Permissions, Use.Names), admin.GetCommand(Admin.Permissions, Admin.Names) };
}
public class CommandInfo
{
    public string Permissions;
    public string[] Names;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public CommandInfo() { }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public CommandInfo(string permissions, string[] names)
    {
        Permissions = permissions;
        Names = names;
    }
    public TShockAPI.Command GetCommand(CommandDelegate cmd) => new(Permissions, cmd, Names);
}
public class MainConfig<T> where T : MainRoot, new()
{
    public string ConfigDirectory;
    public string FileName;
    public virtual string ConfigPath { get { return Path.Combine(ConfigDirectory, FileName); } }
    public T Root = new();
    public bool Normal;
    public string StateString = "正常";
    public string ErrorString = "无";
    public virtual string ConfigString 
    { 
        get 
        {
            Type type = GetType();
            var defaultStream = type.Assembly.GetManifestResourceStream(type.Namespace + "." + FileName);
            if (defaultStream is null) { throw new Exception($"{type.Namespace}.{FileName} resource no find"); }
            using StreamReader reader = new(defaultStream);
            return reader.ReadToEnd();
        } 
    }
    public MainConfig(string configDirectory, string fileName = "config.json")
    {
        ConfigDirectory = configDirectory;
        FileName = fileName;
        if (!Directory.Exists(configDirectory)) Directory.CreateDirectory(configDirectory);
        if (File.Exists(ConfigPath)) Read(readKey: true); else WriteAndRead(readKey: true);
    }
    public virtual bool Read(TSPlayer? ply = null,bool readKey = false,bool log = false,T? obj = default)
    {
        Normal = true;
        try
        {
            Root = JsonConvert.DeserializeObject<T>(File.ReadAllText(ConfigPath)) ?? new T();
        }
        catch(FileNotFoundException ex)
        {
            StateString = FileName + "未找到";
            Normal = false;
            ErrorString = ex.ToString();
        }
        catch (JsonReaderException ex)
        {
            StateString = $"读取{FileName}错误";
            Normal = false;
            ErrorString = ex.ToString();
        }
        catch(Exception ex)
        {
            StateString = $"未知错误";
            Normal = false;
            ErrorString = ex.ToString();
        }
        if (!Normal)
        {
            LogAndOut(ErrorString, ply, log, TraceLevel.Error);
            var newRoot = obj ?? JsonConvert.DeserializeObject<T>(ConfigString);
            if (newRoot is null)
                throw new Exception("配置读取错误");
            Root = newRoot;
            if (readKey) { Console.WriteLine("请按任意键继续...");  Console.ReadKey(); };
        }
        return Normal;
    }
    public virtual bool Write(TSPlayer? ply = null, bool readKey = false, bool log = false)
    {
        Normal = true;
        try
        {
            string writeString;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) 
                writeString = ConfigString.Replace("\r\n", "\n");
            else 
                writeString = ConfigString;
            File.WriteAllText(ConfigPath, writeString);
        }
        catch(Exception ex)
        {
            StateString = "写入默认配置文件出错";
            ErrorString = ex.ToString();
            Normal = false;
        }
        if (!Normal)
        {
            LogAndOut(ErrorString, ply, log, TraceLevel.Error);
            if (readKey) Console.ReadKey();
        }
        return Normal;
    }
    public virtual void WriteAndRead(TSPlayer? ply = null, bool readKey = false)
    {
        if (Write(ply, readKey)) Read(ply, readKey);
    }
    public virtual void LogAndOut(string message, TSPlayer? ply = null, bool log = false, TraceLevel level = TraceLevel.Info)
    {
        if(ply == null)
        {
            switch (level) 
            {
                case TraceLevel.Error:
                    Utils.WriteColorLine(message, ConsoleColor.Red);
                    break;
                case TraceLevel.Warning:
                    Utils.WriteColorLine(message, ConsoleColor.DarkYellow);
                    break;
                case TraceLevel.Info:
                    Utils.WriteInfoLine(message);
                    break;
                default:
                    Console.WriteLine(message);
                    break;
            }

        }
        else
        {
            switch (level)
            {
                case TraceLevel.Error:
                    ply.SendErrorMessage(message);
                    break;
                case TraceLevel.Warning:
                    ply.SendWarningMessage(message);
                    break;
                case TraceLevel.Info:
                    ply.SendInfoMessage(message);
                    break;
                default:
                    ply.SendMessage(message, Microsoft.Xna.Framework.Color.White);
                    break;
            }
        }
        if (log) TShock.Log.Write(message, level);
    }
}
