using System.Data;

namespace VBY.Basic.Extension
{
    public static class VBYDbExt
    {
        public static int GetInt32(this IDataReader reader, string name) => reader.GetInt32(reader.GetOrdinal(name));
        public static string GetString(this IDataReader reader, string name) => reader.GetString(reader.GetOrdinal(name));
        public static DateTime GetDateTime(this IDataReader reader, string name) => reader.GetDateTime(reader.GetOrdinal(name));
        public static void ForEach(this IDataReader args, Action<IDataReader> action) { while (args.Read()) action(args); }
        public static void DoForEach(this IDataReader args,Action<IDataReader> action) { action(args); while (args.Read()) action(args); }
    }
    public static class VBYExt
    {
        public static void RemoveRange<T>(this List<T> list, IEnumerable<T> collection)
        {
            foreach (T i in collection) { list.Remove(i); }
        }
        public static T? Find<T>(this T[] array, Predicate<T> predicate)
        {
            foreach(T t in array) 
            { 
                if (predicate(t)) return t;
            }
            return default;
        }
    }
}
namespace TShockAPI
{
    public static class VBYExt
    {
        public static string StrPermissions(this Group group) => group.Permissions;
    }
}