using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RideSlimExt
/// </summary>
public class RideSlimExt : RideSlim
{

    string area;
    List<EscoretSlim> escorts;
    List<string> equipment;
    bool onlyEscort;
    int rideNum;


    public RideSlimExt()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public RideSlimExt(string patientName, string driverName, int driverId, string origin, string destination, DateTime pickUpTime, int id, string cellPhone) :
           base(patientName, driverName, driverId, origin, destination, pickUpTime, id, cellPhone)
    {
    }

    public string Area
    {
        get
        {
            return area;
        }

        set
        {
            area = value;
        }
    }

    public List<EscoretSlim> Escorts
    {
        get
        {
            return escorts;
        }

        set
        {
            escorts = value;
        }
    }

    public List<string> Equipment
    {
        get
        {
            return equipment;
        }

        set
        {
            equipment = value;
        }
    }

    public bool OnlyEscort
    {
        get
        {
            return onlyEscort;
        }

        set
        {
            onlyEscort = value;
        }
    }

    public int RideNum
    {
        get
        {
            return rideNum;
        }

        set
        {
            rideNum = value;
        }
    }







    public Object GetRidePatView(int daysAhead)
    {

        // get the basic ride data
        DbService db = new DbService();
        Dictionary<int, RideSlimExt> dic = new Dictionary<int, RideSlimExt>();

        //List<RideSlimExt> rides = new List<RideSlimExt>();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@daysAhead", daysAhead);
        // old sp before united
        // cmd.CommandText = "spGetBasicRidePatData";
        // the new sp
        cmd.CommandText = "spGetBasicDataUnityRide";
        //if (db.con.State == ConnectionState.Closed)
        //{
        //    con.Open();
        //}

        //command.Connection = con;
        try
        {
            SqlDataReader dr = db.GetDataReaderSP(cmd);
            while (dr.Read())
            {
                Id = Convert.ToInt32(dr["ridepatnum"]);
                Destination = dr["destination"].ToString();
                Origin = dr["origin"].ToString();
                PickUpTime = Convert.ToDateTime(dr["PickUpTime"].ToString());

                bool isAnonymous = true;
                if (dr["IsAnonymous"] == System.DBNull.Value)
                    isAnonymous = false;
                else
                    isAnonymous = Convert.ToBoolean(dr["IsAnonymous"]);
                PatientName = dr["patient"].ToString();
                if (isAnonymous || PatientName.Contains("אנונימי") || PatientName.Contains("Anonymous"))
                    PatientName = "אנונימי";

                CellPhone = dr["CellPhone"].ToString();
                RideSlimExt rse = new RideSlimExt(PatientName, "", 0, Origin, Destination, PickUpTime, Id, CellPhone);
                rse.Area = dr["area"].ToString();
                rse.OnlyEscort = Convert.ToBoolean(dr["onlyEscort"]);
                dic.Add(Id, rse);

                int ridepatnum = Convert.ToInt32(dr["ridepatnum"]);
                dic[ridepatnum].escorts = new List<EscoretSlim>();
                int amoutOfEscorts = Convert.ToInt32(dr["AmountOfEscorts"]);
                for (int i = 0; i < amoutOfEscorts; i++)
                {
                    dic[ridepatnum].escorts.Add(new EscoretSlim());
                }
            }
            dr.Close();




            // get the escorts
            //cmd.CommandText = "spGetEscortsForRides";
            //dr = db.GetDataReaderSP(cmd);
            //while (dr.Read())
            //{
            //    int ridepatnum = Convert.ToInt32(dr["ridepatnum"]);
            //    if (dic[ridepatnum].escorts == null)
            //    {
            //        dic[ridepatnum].escorts = new List<EscoretSlim>();
            //    }
            //    string displayName = dr["DisplayName"].ToString();
            //    bool isAnonymous = true;
            //    if (dr["IsAnonymous"] == System.DBNull.Value)
            //        isAnonymous = false;
            //    else
            //        isAnonymous = Convert.ToBoolean(dr["IsAnonymous"]);

            //    if (isAnonymous != true && !displayName.Contains("אנונימי") && !displayName.Contains("Anonymus"))
            //    {
            //        string cellphone = dr["CellPhone"].ToString();
            //        dic[ridepatnum].escorts.Add(new EscoretSlim(displayName, cellphone, 0));
            //    }
            //    else
            //        dic[ridepatnum].escorts.Add(new EscoretSlim("אנונימי", "", 0));
            //}
            //dr.Close();


            // get the equipment
            cmd.CommandText = "spGetEquipmentPerPatient";
            dr = db.GetDataReaderSP(cmd);
            while (dr.Read())
            {
                string patient = dr["patient"].ToString();
                string equipment = dr["Name"].ToString();
                foreach (KeyValuePair<int, RideSlimExt> kv in dic)
                {
                    if (kv.Value.PatientName == patient)
                    {
                        if (kv.Value.equipment == null)
                            kv.Value.equipment = new List<string>();
                        kv.Value.equipment.Add(equipment);
                    }
                }
            }

            db.CloseConnection();


            List<RideSlimExt> l = dic.Values.ToList<RideSlimExt>();

            Object o = RideSlimExtToObject(l);

            return o;
        }
        catch (Exception ex)
        {
            DBservice_Gilad.WriteToErrorFile("in_mobileService GetRidePatViewSlim GetRidePatView spGetBasicDataUnityRide", ex.Message + ex.ToString());
            throw new Exception("Error in GetRidePatView " + ex.Message);
        }
    }

    private Object RideSlimExtToObject(List<RideSlimExt> RideList)
    {

        List<Object> listObjs = new List<Object>();
        int escortCounter = 0;

        foreach (RideSlimExt r in RideList)
        {
            try
            {
                List<Object> escorts = new List<Object>();
                List<Object> equipment = new List<object>();
                if (r.Escorts != null)
                    foreach (EscoretSlim e in r.Escorts)
                    {
                        bool isAnonymous = false;
                        if (e.DisplayName == "אנונימי") isAnonymous = true;
                        escorts.Add(new
                        {
                            DisplayName = e.DisplayName,
                            CellPhone = e.CellPhone,
                            IsAnonymous = isAnonymous,
                            Id = escortCounter
                        });
                        escortCounter++;
                    }
                if (r.equipment != null)
                    equipment = r.equipment.ToList<Object>();
                bool isAnonymousP = false;
                if (r.PatientName == "אנונימי")
                    isAnonymousP = true;

                listObjs.Add(new
                {
                    Escorts = escorts,
                    Pat = new
                    {
                        DisplayName = r.PatientName,
                        Equipment = equipment,
                        CellPhone = r.CellPhone,
                        IsAnonymous = isAnonymousP
                    },
                    RidePatNum = r.Id,
                    Origin = new
                    {
                        Name = r.Origin
                    },
                    Destination = new
                    {
                        Name = r.Destination
                    },
                    Date = r.PickUpTime,
                    Area = r.Area,
                    OnlyEscorts = r.OnlyEscort
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        return listObjs;
    }

    private Object RideSlimExtToObjectWithRideId(List<RideSlimExt> RideList)
    {

        List<Object> listObjs = new List<Object>();
        int escortCounter = 0;

        foreach (RideSlimExt r in RideList)
        {
            try
            {
                List<Object> escorts = new List<Object>();
                List<Object> equipment = new List<object>();
                if (r.Escorts != null)
                    foreach (EscoretSlim e in r.Escorts)
                    {
                        bool isAnonymous = false;
                        if (e.DisplayName == "אנונימי") isAnonymous = true;
                        escorts.Add(new
                        {
                            DisplayName = e.DisplayName,
                            CellPhone = e.CellPhone,
                            IsAnonymous = isAnonymous,
                            Id = escortCounter
                        });
                        escortCounter++;
                    }
                if (r.equipment != null)
                    equipment = r.equipment.ToList<Object>();
                bool isAnonymousP = false;
                if (r.PatientName == "אנונימי")
                    isAnonymousP = true;

                List<Object> rpList = new List<object>();

                rpList.Add(new
                {
                    Escorts = escorts,
                    Pat = new
                    {
                        DisplayName = r.PatientName,
                        Equipment = equipment,
                        CellPhone = r.CellPhone,
                        IsAnonymous = isAnonymousP
                    },
                    RidePatNum = r.Id,
                    Origin = new
                    {
                        Name = r.Origin
                    },
                    Destination = new
                    {
                        Name = r.Destination
                    },
                    Date = r.PickUpTime,
                    Area = r.Area,
                    OnlyEscorts = r.OnlyEscort
                });

                listObjs.Add(new
                {
                    Id = r.RideNum,
                    RidePats = rpList
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        return listObjs;
    }
    public Object GetFutureRides(int driverId)
    {
        try
        {
            // get the basic ride data
            DbService db = new DbService();
            Dictionary<int, RideSlimExt> dic = new Dictionary<int, RideSlimExt>();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@driverId", driverId);
            cmd.CommandText = "spGetFutureUnityRidesOfVolunteer";

            SqlDataReader dr = db.GetDataReaderSP(cmd);
            while (dr.Read())
            {
                Id = Convert.ToInt32(dr["ridepatnum"]);
                Destination = dr["destination"].ToString();
                Origin = dr["origin"].ToString();
                PickUpTime = Convert.ToDateTime(dr["PickUpTime"].ToString());
                bool isAnonymous = true;
                if (dr["IsAnonymous"] == System.DBNull.Value)
                    isAnonymous = false;
                else
                    isAnonymous = Convert.ToBoolean(dr["IsAnonymous"]);
                PatientName = dr["patient"].ToString();
                if (isAnonymous || PatientName.Contains("אנונימי") || PatientName.Contains("Anonymous"))
                {
                    PatientName = "אנונימי";
                }

                CellPhone = dr["CellPhone"].ToString();
                RideSlimExt rse = new RideSlimExt(PatientName, "", 0, Origin, Destination, PickUpTime, Id, CellPhone);
                rse.Area = dr["area"].ToString();
                rse.OnlyEscort = Convert.ToBoolean(dr["onlyEscort"]);
                rse.RideNum = Convert.ToInt32(dr["rideNum"]);
                dic.Add(Id, rse);
                dic[Id].escorts = new List<EscoretSlim>();
                int amountOfEscorts = Convert.ToInt32(dr["AmountOfEscorts"]);
                for (int i = 0; i < amountOfEscorts; i++)
                {
                    dic[Id].escorts.Add(new EscoretSlim("אנונימי", "", 0));
                }
            }

            dr.Close();

            //// get the escorts
            //cmd.CommandText = "spGetFutureEscortsForRidesOfVolunteer";
            //dr = db.GetDataReaderSP(cmd);
            //while (dr.Read())
            //{
            //    int ridepatnum = Convert.ToInt32(dr["ridepatnum"]);
            //    if (dic[ridepatnum].escorts == null)
            //    {
            //        dic[ridepatnum].escorts = new List<EscoretSlim>();
            //    }

            //    string displayName = dr["DisplayName"].ToString();
            //    bool isAnonymous = true;

            //    if (dr["IsAnonymous"] == System.DBNull.Value)
            //        isAnonymous = false;
            //    else
            //        isAnonymous = Convert.ToBoolean(dr["IsAnonymous"]);

            //    if (isAnonymous != true && !displayName.Contains("אנונימי") && !displayName.Contains("Anonymus"))
            //    {
            //        string cellphone = dr["CellPhone"].ToString();
            //        dic[ridepatnum].escorts.Add(new EscoretSlim(displayName, cellphone, 0));
            //    }
            //    else
            //        dic[ridepatnum].escorts.Add(new EscoretSlim("אנונימי", "", 0));
            //}
            //dr.Close();

            // get the equipment
            cmd.CommandText = "spGetFutureEquipmentPerPatientPerDriver_unityRide";
            dr = db.GetDataReaderSP(cmd);
            while (dr.Read())
            {
                string patient = dr["patient"].ToString();
                string equipment = dr["Name"].ToString();
                foreach (KeyValuePair<int, RideSlimExt> kv in dic)
                {
                    if (kv.Value.PatientName == patient)
                    {
                        if (kv.Value.equipment == null)
                            kv.Value.equipment = new List<string>();
                        kv.Value.equipment.Add(equipment);
                    }
                }
            }

            db.CloseConnection();


            List<RideSlimExt> l = dic.Values.ToList<RideSlimExt>();

            Object o = RideSlimExtToObjectWithRideId(l);

            return o;
        }
        catch (Exception ex)
        {

            DBservice_Gilad.WriteToErrorFile("in_mobileService getMyRidesSlim GetFutureRides spGetFutureUnityRidesOfVolunteer spGetFutureEquipmentPerPatientPerDriver_unityRide", ex.Message + ex.ToString());
            throw new Exception("Error in GetFutureRides " + ex.Message);
        }

       

    }

    public Object GetPastRides(int driverId)
    {
        try
        {
            // get the basic ride data
            DbService db = new DbService();
            Dictionary<int, RideSlimExt> dic = new Dictionary<int, RideSlimExt>();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@driverId", driverId);
            // old sp before unity
            //cmd.CommandText = "spGetPastRidesOfVolunteer";
            //New sp after unity
            cmd.CommandText = "spGetPastRideOfVol_unityRide";

            SqlDataReader dr = db.GetDataReaderSP(cmd);
            while (dr.Read())
            {
                Id = Convert.ToInt32(dr["ridepatnum"]);
                Destination = dr["destination"].ToString();
                Origin = dr["origin"].ToString();
                PickUpTime = Convert.ToDateTime(dr["PickUpTime"].ToString());
                bool isAnonymous = true;
                if (dr["IsAnonymous"] == System.DBNull.Value)
                    isAnonymous = false;
                else
                    isAnonymous = Convert.ToBoolean(dr["IsAnonymous"]);
                PatientName = dr["patient"].ToString();
                if (isAnonymous || PatientName.Contains("אנונימי") || PatientName.Contains("Anonymous"))
                    PatientName = "אנונימי";
                CellPhone = dr["CellPhone"].ToString();
                RideSlimExt rse = new RideSlimExt(PatientName, "", 0, Origin, Destination, PickUpTime, Id, CellPhone);
                if (dr["area"] == System.DBNull.Value)
                    rse.Area = "";
                else
                    rse.Area = dr["area"].ToString();
                rse.OnlyEscort = Convert.ToBoolean(dr["onlyEscort"]);
                rse.RideNum = Convert.ToInt32(dr["rideNum"]);

                dic.Add(Id, rse);
                dic[Id].escorts = new List<EscoretSlim>();

                int amountOfEscorts = Convert.ToInt32(dr["AmountOfEscorts"]);
                for (int i = 0; i < amountOfEscorts; i++)
                {
                    dic[Id].escorts.Add(new EscoretSlim("x", "0", 0));
                }
            }

            dr.Close();

            // get the escorts
            //cmd.CommandText = "spGetPastEscortsForRidesOfVolunteer";
            //dr = db.GetDataReaderSP(cmd);
            //while (dr.Read())
            //{
            //    try
            //    {
            //        int ridepatnum = Convert.ToInt32(dr["ridepatnum"]);
            //        if (dic[ridepatnum].escorts == null)
            //        {
            //            dic[ridepatnum].escorts = new List<EscoretSlim>();
            //        }

            //        string displayName = dr["DisplayName"].ToString();
            //        bool isAnonymous = true;
            //        if (dr["IsAnonymous"] == System.DBNull.Value)
            //            isAnonymous = false;
            //        else
            //            isAnonymous = Convert.ToBoolean(dr["IsAnonymous"]);

            //        if (isAnonymous != true && !displayName.Contains("אנונימי") && !displayName.Contains("Anonymus"))
            //        {
            //            string cellphone = dr["CellPhone"].ToString();
            //            dic[ridepatnum].escorts.Add(new EscoretSlim(displayName, cellphone, 0));
            //        }
            //        else
            //            dic[ridepatnum].escorts.Add(new EscoretSlim("אנונימי", "", 0));

            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //}
            //dr.Close();

            // get the equipment
            //old sp
            //cmd.CommandText = "spGetPastEquipmentPerPatientPerDriver";
            // the new sp 
            cmd.CommandText = "spGetPastEquipmentPerPatientPerDriver_UnityRide";
            dr = db.GetDataReaderSP(cmd);
            while (dr.Read())
            {
                string patient = dr["patient"].ToString();
                string equipment = dr["Name"].ToString();
                foreach (KeyValuePair<int, RideSlimExt> kv in dic)
                {
                    if (kv.Value.PatientName == patient)
                    {
                        if (kv.Value.equipment == null)
                            kv.Value.equipment = new List<string>();
                        kv.Value.equipment.Add(equipment);
                    }
                }
            }

            db.CloseConnection();


            List<RideSlimExt> l = dic.Values.ToList<RideSlimExt>();

            Object o = RideSlimExtToObjectWithRideId(l);

            return o;
        }
        catch (Exception ex)
        {

            DBservice_Gilad.WriteToErrorFile("in_mobileService getPastRidesSlim GetPastRides spGetPastRideOfVol_unityRide spGetPastEquipmentPerPatientPerDriver_UnityRide", ex.Message + ex.ToString());
            throw new Exception("Error in GetPastRides " + ex.Message);
        }


    }

}