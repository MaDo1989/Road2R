using System.Collections.Generic;

public class VolunteerPreferencesResponse
{
    public List<PreferredDayMobile> PreferredDays { get; set; }
    public List<PreferredAreaMobile> PreferredAreas { get; set; }

    public VolunteerPreferencesResponse()
    {
        PreferredDays = new List<PreferredDayMobile>();
        PreferredAreas = new List<PreferredAreaMobile>();
    }
}

public class PreferredDayMobile
{
    public string PreferedDayDayInWeek { get; set; }
    public int? VolunteerId { get; set; }
    public string Shift { get; set; }
}

public class PreferredAreaMobile
{
    public string PreferredArea { get; set; }
    public int? VolunteerId { get; set; }
}