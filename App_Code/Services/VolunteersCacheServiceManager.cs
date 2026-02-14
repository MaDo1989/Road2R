using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for VolunteersCacheServiceManager
/// </summary>
public static class VolunteersCacheServiceManager
{
    private static string key = "Volunteers";


    // This method retrieves the list of volunteers from the cache. If the cache is empty, it fetches the data from the database and stores it in the cache for future use.
    public static List<Volunteer> GetVolunteers()
    {
        List<Volunteer> Cache_volunteers = CacheService.GetList<Volunteer>(key);
        if (Cache_volunteers == null)
        {
            DBservice_Gilad db = new DBservice_Gilad();
            Cache_volunteers = db.GetVolunteersList_All_Fast();
            CacheService.SetList<Volunteer>(key, Cache_volunteers);
        }
        return Cache_volunteers;
    }

    // עדכון מתנדב קיים בקאש
    public static void UpdateVolunteer(Volunteer v)
    {
        // אם אין קאש עדיין, אין מה לעדכן - יטען מ-DB בפעם הבאה
        if (CacheService.GetList<Volunteer>(key) == null)
            return;

        bool updated = CacheService.UpdateItemInList<Volunteer>(
            key,
            vol => vol.Id == v.Id,
            v
        );

        // אם לא נמצא (למשל שינוי DisplayName) - נקה קאש לטעינה מחדש
        if (!updated)
            CacheService.Remove(key);
    }

    // הוספת מתנדב חדש לקאש
    public static void AddVolunteer(Volunteer v)
    {
        if (CacheService.GetList<Volunteer>(key) == null)
            return;

        CacheService.AddItemToList<Volunteer>(key, v);
    }

    // ניקוי קאש מלא - למקרים שצריך רענון
    public static void ClearCache()
    {
        CacheService.Remove(key);
    }

}