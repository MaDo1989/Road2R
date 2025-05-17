/// <summary>
/// Summary description for EscoretSlim
/// </summary>
public class EscoretSlim
{
    public EscoretSlim()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public EscoretSlim(string displayName, string cellPhone, int id)
    {
        this.DisplayName = displayName;
        this.CellPhone = cellPhone;
        this.Id = id;
    }

    string displayName;
    string cellPhone;
    int id;

    public string DisplayName
    {
        get
        {
            return displayName;
        }

        set
        {
            displayName = value;
        }
    }

    public string CellPhone
    {
        get
        {
            return cellPhone;
        }

        set
        {
            cellPhone = value;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }
}