using System;
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

    public BaseResponse SetVolunteerPreferences(
    int volunteerId,
    int availableSeats,
    List<PreferredDayMobile> preferredDays,
    List<PreferredAreaMobile> preferredAreas)
    {
        BaseResponse response = new BaseResponse();

        try
        {
            if (volunteerId <= 0)
            {
                response.ResponseStatus = 400;
                response.Message = "Invalid volunteer id";
                return response;
            }

            if (availableSeats < 0)
            {
                response.ResponseStatus = 400;
                response.Message = "Available seats cannot be negative";
                return response;
            }

            if (preferredDays == null)
                preferredDays = new List<PreferredDayMobile>();

            if (preferredAreas == null)
                preferredAreas = new List<PreferredAreaMobile>();

            foreach (PreferredDayMobile day in preferredDays)
            {
                if (string.IsNullOrWhiteSpace(day.PreferedDayDayInWeek))
                {
                    response.ResponseStatus = 400;
                    response.Message = "Preferred day is required";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(day.Shift))
                {
                    response.ResponseStatus = 400;
                    response.Message = "Shift is required";
                    return response;
                }

                day.VolunteerId = volunteerId;
            }

            foreach (PreferredAreaMobile area in preferredAreas)
            {
                if (string.IsNullOrWhiteSpace(area.PreferredArea))
                {
                    response.ResponseStatus = 400;
                    response.Message = "Preferred area is required";
                    return response;
                }

                area.VolunteerId = volunteerId;
            }

            return _mobileDBService.SetVolunteerPreferences(
                volunteerId,
                availableSeats,
                preferredDays,
                preferredAreas);
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                ResponseStatus = 500,
                Message = ex.Message
            };
        }
    }
}