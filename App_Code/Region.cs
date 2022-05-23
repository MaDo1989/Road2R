using System;
using System.Collections.Generic;
using System.Data.SqlClient;


/// <summary>
/// Summary description for Region
/// </summary>
public class Region
{
    public Region(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public Region(int id)
    {
        Id = id;
    }
    public Region(){}

    public int Id { get; set; }
    public string Name { get; set; }
    DbService dbs;

    public List<Region> GetAllRegions()
    {
        List<Region> regions = new List<Region>();
        Region region;
        string query = "exec spGetAllRegions";

        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);

            while (sdr.Read())
            {
                region = new Region(Convert.ToInt32(sdr["Id"]), sdr["RegionName"].ToString());
                regions.Add(region);
            }

            return regions;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbs.CloseConnection();
        }


    }

}