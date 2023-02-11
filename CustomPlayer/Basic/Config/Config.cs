using Newtonsoft.Json;
using System.Diagnostics;
using TShockAPI;

namespace VBY.Basic.Config;

public class MainRoot
{
    public Command Commands = new();
}
public class Command
{
    public CommandInfo Use = new();
    public CommandInfo Admin = new();
    public TShockAPI.Command[] GetCommands(CommandDelegate use, CommandDelegate admin)
    {
        return new TShockAPI.Command[] { this.Use.GetCommand(use), this.Admin.GetCommand(admin) };
    }
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
        this.Permissions = permissions;
        this.Names = names;
    }
    public TShockAPI.Command GetCommand(CommandDelegate cmd)
    {
        return new TShockAPI.Command(this.Permissions, cmd, this.Names);
    }
}
public class MainConfig<T> where T : MainRoot, new()
{
    public string ConfigDirectory;
    public string FileName;
    public virtual string ConfigPath => Path.Combine(this.ConfigDirectory, this.FileName);
    public T Root = new();
    public bool Normal;
    public string StateString = "正常";
    public string ErrorString = "无";
    public virtual string ConfigString
    {
        get
        {
            var type = this.GetType();
            var defaultStream = type.Assembly.GetManifestResourceStream(type.Namespace + "." + this.FileName);
            if (defaultStream is null) { throw new Exception($"{type.Namespace}.{this.FileName} resource no find"); }
            using StreamReader reader = new(defaultStream);
            return reader.ReadToEnd();
        }
    }
    public MainConfig(string configDirectory, string fileName = "config.json")
    {
        this.ConfigDirectory = configDirectory;
        this.FileName = fileName;
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }

        if (File.Exists(this.ConfigPath))
        {
            this.Read(readKey: true);
        }
        else
        {
            this.WriteAndRead(readKey: true);
        }
    }
    public virtual bool Read(TSPlayer? ply = null, bool readKey = false, bool log = false, T? obj = default)
    {
        this.Normal = true;
        try
        {
            this.Root = JsonConvert.DeserializeObject<T>(File.ReadAllText(this.ConfigPath)) ?? new T();
        }
        catch (FileNotFoundException ex)
        {
            this.StateString = this.FileName + "未找到";
            this.Normal = false;
            this.ErrorString = ex.ToString();
        }
        catch (JsonReaderException ex)
        {
            this.StateString = $"读取{this.FileName}错误";
            this.Normal = false;
            this.ErrorString = ex.ToString();
        }
        catch (Exception ex)
        {
            this.StateString = $"未知错误";
            this.Normal = false;
            this.ErrorString = ex.ToString();
        }
        if (!this.Normal)
        {
            this.LogAndOut(this.ErrorString, ply, log, TraceLevel.Error);
            var newRoot = obj ?? JsonConvert.DeserializeObject<T>(this.ConfigString);
            if (newRoot is null)
            {
                throw new Exception("配置读取错误");
            }

            this.Root = newRoot;
            if (readKey) { Console.WriteLine("请按任意键继续..."); Console.ReadKey(); };
        }
        return this.Normal;
    }
    public virtual bool Write(TSPlayer? ply = null, bool readKey = false, bool log = false)
    {
        this.Normal = true;
        try
        {
            var writeString = Environment.OSVersion.Platform != PlatformID.Win32NT ? this.ConfigString.Replace("\r\n", "\n") : this.ConfigString;
            File.WriteAllText(this.ConfigPath, writeString);
        }
        catch (Exception ex)
        {
            this.StateString = "写入默认配置文件出错";
            this.ErrorString = ex.ToString();
            this.Normal = false;
        }
        if (!this.Normal)
        {
            this.LogAndOut(this.ErrorString, ply, log, TraceLevel.Error);
            if (readKey)
            {
                Console.ReadKey();
            }
        }
        return this.Normal;
    }
    public virtual void WriteAndRead(TSPlayer? ply = null, bool readKey = false)
    {
        if (this.Write(ply, readKey))
        {
            this.Read(ply, readKey);
        }
    }
    public virtual void LogAndOut(string message, TSPlayer? ply = null, bool log = false, TraceLevel level = TraceLevel.Info)
    {
        if (ply == null)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    Utils.WriteColorLine(message);
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
        if (log)
        {
            TShock.Log.Write(message, level);
        }
    }
}