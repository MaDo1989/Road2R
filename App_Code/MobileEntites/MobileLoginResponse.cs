using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MobileLoginResponse
/// </summary>
public class MobileLoginResponse
{
    public int ResponseStatus { get; set; }
    public string Message { get; set; }

    public int? Id { get; set; }
    public string DisplayName { get; set; }
    public string FirstNameH { get; set; }
    public string LastNameH { get; set; }
    public string CellPhone { get; set; }
    public string CellPhone2 { get; set; }
    public string HomePhone { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public DateTime? JoinDate { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Gender { get; set; }
    public int? AvailableSeats { get; set; }
    public int? JoinYear { get; set; }
    public int? NoOfDocumentedRides { get; set; }
    public int? NoOfRides { get; set; }
    public bool? IsBooster { get; set; }
    public bool? IsBabyChair { get; set; }
}