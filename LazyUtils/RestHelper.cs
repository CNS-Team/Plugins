using Newtonsoft.Json.Linq;
using Rests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazyUtils.Commands;
using TShockAPI;
using MonoMod.Utils;

namespace LazyUtils;

public static class RestHelper
{
    private static RestCommandD ParseCommand(MethodInfo method)
    {
        var @params = method.GetParameters();
        var n = @params.Length;
        var parsers = new CommandParser.Parser[n - 1];
        var names = new string[n - 1];
        var errors = new RestObject[n - 1];
        for (var i = 1; i < n; ++i)
        {
            var parser = CommandParser.GetParser(@params[i].ParameterType);
            parsers[i - 1] = parser;
            names[i - 1] = @params[i].Name!;
            errors[i - 1] = new RestObject("400") { Error = $"Bad input for parameter: {CommandParser.GetFriendlyName(@params[i].ParameterType)} {names[i - 1]}" };
        }

        var @delegate = method;

        return arg =>
        {
            var args = new object[n];
            args[0] = arg;
            for (var i = 1; i < n; ++i)
                if (!parsers[i - 1](arg.Parameters[names[i - 1]], out args[i]))
                    return errors[i - 1];
            return @delegate.Invoke(null, args);
        };
    }

    internal static void Register(Type type, string name, LazyPlugin plugin)
    {
        foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
            RestCommandD parser = ParseCommand(method);
            if (parser == null) continue;
            TShock.RestApi.Register(new SecureRestCommand($"/{name}/{method.Name}", parser,
                method.GetCustomAttributes<Permission>().Select(p => p.Name)
                    .Concat(method.GetCustomAttributes<PermissionsAttribute>().Select(p => p.perm)).ToArray()));
            Console.WriteLine($"[{plugin.Name}] rest endpoint registered: /{name}/{method.Name}");
        }
    }
}