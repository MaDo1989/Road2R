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
        return reader.IsDBNull(index) ? (int?)null : reader.GetInt32(index);
    }

    public static DateTime? GetNullableDateTime(this SqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);
        return reader.IsDBNull(index) ? (DateTime?)null : reader.GetDateTime(index);
    }

    public static bool? GetNullableBool(this SqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);
        return reader.IsDBNull(index) ? (bool?)null : reader.GetBoolean(index);
    }
}