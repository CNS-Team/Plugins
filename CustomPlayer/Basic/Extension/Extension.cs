using System.Data;

namespace VBY.Basic.Extension
{
    public static class VBYDbExt
    {
        public static int GetInt32(this IDataReader reader, string name)
        {
            return reader.GetInt32(reader.GetOrdinal(name));
        }

        public static string GetString(this IDataReader reader, string name)
        {
            return reader.GetString(reader.GetOrdinal(name));
        }

        public static DateTime GetDateTime(this IDataReader reader, string name)
        {
            return reader.GetDateTime(reader.GetOrdinal(name));
        }

        public static void ForEach(this IDataReader args, Action<IDataReader> action)
        {
            while (args.Read())
            {
                action(args);
            }
        }
        public static void DoForEach(this IDataReader args, Action<IDataReader> action)
        {
            action(args); 
            while (args.Read())
            {
                action(args);
            }
        }
    }
    public static class VBYExt
    {
        public static void RemoveRange<T>(this List<T> list, IEnumerable<T> collection)
        {
            foreach (var i in collection) { list.Remove(i); }
        }
        public static T? Find<T>(this T[] array, Predicate<T> predicate)
        {
            foreach (var t in array)
            {
                if (predicate(t))
                {
                    return t;
                }
            }
            return default;
        }
    }
}
namespace System
{
    public static class StringExt
    {
        public static string LastWord(this string str)
        {
            for (var i = str.Length - 1; i >= 0; i--)
            {
                if (char.IsUpper(str[i]))
                {
                    return str.Substring(i);
                }
            }
            return str;
        }
    }
}