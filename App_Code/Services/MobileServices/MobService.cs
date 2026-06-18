using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class MobileService
{
    private readonly string _connectionString;

    public MobileService()
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
}