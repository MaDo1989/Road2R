/// <summary>
/// Summary description for Area
/// </summary>
public class Area
{

    public Area(string hebrewName, string englishName, bool isRoute)
    {
        HebrewName = hebrewName;
        EnglishName = englishName;
        IsRoute = isRoute;
    }

    public string HebrewName { get; set; }
    public string EnglishName { get; set; }
    public bool IsRoute { get; set; }
}