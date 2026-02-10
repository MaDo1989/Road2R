using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;
using System.Data;


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

    public Dictionary<string, List<DBstracture>> GetStractureFromDB(List<string> tablesName)
    {
        // Create a cache key based on the tables requested
        string cacheKey = "DBStructure_" + string.Join("_", tablesName.OrderBy(t => t));

        ObjectCache cache = MemoryCache.Default;

        var cached = cache[cacheKey] as Dictionary<string, List<DBstracture>>;
        if (cached != null)
        {
            return cached;
        }

        // Not in cache - fetch from DB
        DBservice_Gilad db = new DBservice_Gilad();
        var listDBStracture = db.GetDBstractures(tablesName);
        var result = GroupByTable(listDBStracture);

        // Cache for 3 days
        CacheItemPolicy policy = new CacheItemPolicy();
        policy.AbsoluteExpiration = DateTimeOffset.Now.AddDays(3);

        cache.Set(cacheKey, result, policy);

        return result;
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

    public DataTable ExecQuery(Execution exec)
    {
        if (exec.SqlQuery == null || string.IsNullOrEmpty(exec.SqlQuery))
        {
            throw new ArgumentNullException("the sql param is empty or null");
        }
        else
        {
            DBservice_Gilad dbs = new DBservice_Gilad();
            DataTable results =  dbs.ExecuteQueryByString(exec.SqlQuery);
            if (results!= null && results.Rows.Count > 0)
            {
                int indication = Execution.InsertExec(exec);
                return results;
            }

        }
        return null;
            
    }
}