using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

public class MobileDBService
{
    private readonly string _connectionString;

    public MobileDBService()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    }

    public MobileLoginResponse LoginWithPhoneNumber(string cellPhone)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("dbo.USP_Login_with_phone_number_Mobile", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@CellPhone", SqlDbType.NVarChar, 11).Value = cellPhone;

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return new MobileLoginResponse
                    {
                        ResponseStatus = 500,
                        Message = "No response from database"
                    };
                }

                int responseStatus = reader.GetInt32(reader.GetOrdinal("ResponseStatus"));

                if (responseStatus != 200)
                {
                    return new MobileLoginResponse
                    {
                        ResponseStatus = responseStatus,
                        Message = reader.GetNullableString("Message")
                    };
                }

                return new MobileLoginResponse
                {
                    ResponseStatus = responseStatus,
                    Message = "OK",

                    Id = reader.GetNullableInt("Id"),
                    DisplayName = reader.GetNullableString("DisplayName"),
                    FirstNameH = reader.GetNullableString("FirstNameH"),
                    LastNameH = reader.GetNullableString("LastNameH"),
                    CellPhone = reader.GetNullableString("CellPhone"),
                    CellPhone2 = reader.GetNullableString("CellPhone2"),
                    HomePhone = reader.GetNullableString("HomePhone"),
                    Address = reader.GetNullableString("Address"),
                    Email = reader.GetNullableString("Email"),
                    JoinDate = reader.GetNullableDateTime("JoinDate"),
                    IsActive = reader.GetNullableBool("IsActive"),
                    BirthDate = reader.GetNullableDateTime("BirthDate"),
                    Gender = reader.GetNullableString("Gender"),
                    AvailableSeats = reader.GetNullableInt("AvailableSeats"),
                    JoinYear = reader.GetNullableInt("joinYear"),
                    NoOfDocumentedRides = reader.GetNullableInt("NoOfDocumentedRides"),
                    NoOfRides = reader.GetNullableInt("No_of_Rides"),
                    IsBooster = reader.GetNullableBool("IsBooster"),
                    IsBabyChair = reader.GetNullableBool("IsBabyChair")
                };
            }
        }
    }

    public VolunteerPreferencesResponse GetVolunteerPreferences(int volunteerId)
    {
        VolunteerPreferencesResponse result = new VolunteerPreferencesResponse();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("dbo.USP_Get_Pref_Area_And_Time_Vol_Mobile", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@volunteerId", SqlDbType.Int).Value = volunteerId;

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.PreferredDays.Add(new PreferredDayMobile
                    {
                        PreferedDayDayInWeek = reader.GetNullableString("PreferedDayDayInWeek"),
                        VolunteerId = reader.GetNullableInt("VolunteerId"),
                        Shift = reader.GetNullableString("Shift")
                    });
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        result.PreferredAreas.Add(new PreferredAreaMobile
                        {
                            PreferredArea = reader.GetNullableString("PreferredArea"),
                            VolunteerId = reader.GetNullableInt("VolunteerId")
                        });
                    }
                }
            }
        }

        return result;
    }

    public List<MobileMyUnityRide> GetMyUnityRides(int volunteerId)
    {
        List<MobileMyUnityRide> rides = new List<MobileMyUnityRide>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("dbo.USP_Get_All_My_UnityRides_Mobile", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@volunteerId", SqlDbType.Int).Value = volunteerId;

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    rides.Add(new MobileMyUnityRide
                    {
                        RidePatNum = reader.GetNullableInt("RidePatNum"),
                        PatientName = reader.GetNullableString("PatientName"),
                        PatientCellPhone = reader.GetNullableString("PatientCellPhone"),
                        Origin = reader.GetNullableString("Origin"),
                        Destination = reader.GetNullableString("Destination"),
                        PickupTime = reader.GetNullableDateTime("pickupTime"),
                        IsAfterNoon = reader.GetNullableBool("isAfterNoon"),
                        Area = reader.GetNullableString("Area"),
                        Status = reader.GetNullableString("Status"),
                        AmountOfEscorts = reader.GetNullableInt("AmountOfEscorts"),
                        IsInThePast = reader.GetNullableBool("IsInThePast")
                    });
                }
            }
        }

        return rides;
    }

    public List<MobileUnityRide> GetAllUnityRidesWithEquipments()
    {
        List<MobileUnityRide> rides = GetAllUnityRides();

        List<int> patientIds = rides
            .Where(r => r.PatientId.HasValue)
            .Select(r => r.PatientId.Value)
            .Distinct()
            .ToList();

        if (patientIds.Count == 0)
            return rides;

        Dictionary<int, List<MobilePatientEquipment>> equipmentsByPatientId =
            GetEquipmentsByPatientIds(patientIds);

        foreach (MobileUnityRide ride in rides)
        {
            if (ride.PatientId.HasValue &&
                equipmentsByPatientId.ContainsKey(ride.PatientId.Value))
            {
                ride.Equipments = equipmentsByPatientId[ride.PatientId.Value];
            }
        }

        return rides;
    }




    //-------------private methods for internal use-------------\\
    private List<MobileUnityRide> GetAllUnityRides()
    {
        List<MobileUnityRide> rides = new List<MobileUnityRide>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("dbo.USP_Get_All_UnityRides_Mobile", connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    rides.Add(new MobileUnityRide
                    {
                        RidePatNum = reader.GetNullableInt("RidePatNum"),
                        PatientName = reader.GetNullableString("PatientName"),
                        PatientCellPhone = reader.GetNullableString("PatientCellPhone"),
                        Origin = reader.GetNullableString("Origin"),
                        Destination = reader.GetNullableString("Destination"),
                        PickupTime = reader.GetNullableDateTime("PickupTime"),
                        IsAfterNoon = reader.GetNullableBool("IsAfterNoon"),
                        Area = reader.GetNullableString("Area"),
                        Status = reader.GetNullableString("Status"),
                        AmountOfEscorts = reader.GetNullableInt("AmountOfEscorts"),
                        IsAnonymous = reader.GetNullableBool("IsAnonymous"),
                        PatientId = reader.GetNullableInt("PatientId"),
                        PatientCellPhone2 = reader.GetNullableString("PatientCellPhone2"),
                        PatientHomePhone = reader.GetNullableString("PatientHomePhone")
                    });
                }
            }
        }

        return rides;
    }

    private Dictionary<int, List<MobilePatientEquipment>> GetEquipmentsByPatientIds(List<int> patientIds)
    {
        Dictionary<int, List<MobilePatientEquipment>> result =
            new Dictionary<int, List<MobilePatientEquipment>>();

        DataTable table = new DataTable();
        table.Columns.Add("Id", typeof(int));

        foreach (int patientId in patientIds)
        {
            table.Rows.Add(patientId);
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("dbo.USP_Get_Equipments_By_List_Patient_Ids_Mobile", connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter parameter = command.Parameters.AddWithValue("@PatientIds", table);
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.TypeName = "dbo.IntIdList";

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    MobilePatientEquipment equipment = new MobilePatientEquipment
                    {
                        PatientId = reader.GetNullableInt("PatientId"),
                        EquipmentId = reader.GetNullableInt("EquipmentId"),
                        Name = reader.GetNullableString("Name")
                    };

                    if (!equipment.PatientId.HasValue)
                        continue;

                    int patientId = equipment.PatientId.Value;

                    if (!result.ContainsKey(patientId))
                        result[patientId] = new List<MobilePatientEquipment>();

                    result[patientId].Add(equipment);
                }
            }
        }

        return result;
    }
}