using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DBstracture
/// </summary>
public class DBstracture
{
    string tableName;
    string columnName;
    string dataType;
    public DBstracture()
    {
        
    }

    public DBstracture(string tableName, string columnName, string dataType)
    {
        TableName = tableName;
        ColumnName = columnName;
        DataType = dataType;
    }

    public string TableName
    {
        get
        {
            return tableName;
        }

        set
        {
            tableName = value;
        }
    }

    public string ColumnName
    {
        get
        {
            return columnName;
        }

        set
        {
            columnName = value;
        }
    }

    public string DataType
    {
        get
        {
            return dataType;
        }

        set
        {
            dataType = value;
        }
    }

    public Dictionary<string,List<DBstracture>> GetStractureFromDB(List<string> tablesName)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        var listDBStracture = db.GetDBstractures(tablesName);
        return GroupByTable(listDBStracture);
    }

    private Dictionary<string, List<DBstracture>> GroupByTable(List<DBstracture> list)
    {
        return list
            .GroupBy(x => x.TableName)
            .ToDictionary(
                g => g.Key,
                g => g.ToList()
            );
    }
}