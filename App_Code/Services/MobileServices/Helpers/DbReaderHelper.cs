using System;
using System.Data.SqlClient;

public static class DbReaderHelper
{
    public static string GetNullableString(this SqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);
        return reader.IsDBNull(index) ? null : reader.GetString(index);
    }

    public static int? GetNullableInt(this SqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(index))
            return null;

        return Convert.ToInt32(reader.GetValue(index));
    }

    public static DateTime? GetNullableDateTime(this SqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);
        return reader.IsDBNull(index) ? (DateTime?)null : reader.GetDateTime(index);
    }

    public static bool? GetNullableBool(this SqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(index))
            return null;

        object value = reader.GetValue(index);

        if (value is bool)
            return (bool)value;

        if (value is int)
            return (int)value == 1;

        if (value is short)
            return (short)value == 1;

        if (value is byte)
            return (byte)value == 1;

        string text = value.ToString();

        if (text == "1")
            return true;

        if (text == "0")
            return false;

        return Convert.ToBoolean(value);
    }
}