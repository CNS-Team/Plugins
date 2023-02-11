namespace System;

public static class StringExt
{
    public static string LastWord(this string str)
    {
        for (var i = str.Length - 1; i >= 0; i--)
        {
            if (char.IsUpper(str[i]))
            {
                return str[i..];
            }
        }
        return str;
    }
}