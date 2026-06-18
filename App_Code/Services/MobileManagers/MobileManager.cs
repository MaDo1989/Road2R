using System.Collections.Generic;
using System.Linq;

public class MobileManager
{
    private readonly MobileDBService _mobileDBService;

    public MobileManager()
    {
        _mobileDBService = new MobileDBService();
    }

    public MobileLoginResponse LoginWithPhoneNumber(string cellPhone)
    {
        if (string.IsNullOrWhiteSpace(cellPhone))
        {
            return new MobileLoginResponse
            {
                ResponseStatus = 400,
                Message = "Phone number is required"
            };
        }

        cellPhone = cellPhone.Trim();

        return _mobileDBService.LoginWithPhoneNumber(cellPhone);
    }

    public VolunteerPreferencesResponse GetVolunteerPreferences(int volunteerId)
    {
        if (volunteerId <= 0)
        {
            return new VolunteerPreferencesResponse();
        }

        return _mobileDBService.GetVolunteerPreferences(volunteerId);
    }

    public List<MobileMyUnityRide> GetMyUnityRides(int volunteerId)
    {
        if (volunteerId <= 0)
        {
            return new List<MobileMyUnityRide>();
        }

        return _mobileDBService.GetMyUnityRides(volunteerId);
    }

    public List<MobileUnityRide> GetAllUnityRidesWithEquipments()
    {
        return _mobileDBService.GetAllUnityRidesWithEquipments();
    }
}