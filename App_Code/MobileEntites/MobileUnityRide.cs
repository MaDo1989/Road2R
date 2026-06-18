using System;
using System.Collections.Generic;

public class MobileUnityRide
{
    public int? RidePatNum { get; set; }
    public string PatientName { get; set; }
    public string PatientCellPhone { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTime? PickupTime { get; set; }
    public bool? IsAfterNoon { get; set; }
    public string Area { get; set; }
    public string Status { get; set; }
    public int? AmountOfEscorts { get; set; }
    public bool? IsAnonymous { get; set; }
    public int? PatientId { get; set; }
    public string PatientCellPhone2 { get; set; }
    public string PatientHomePhone { get; set; }

    public List<MobilePatientEquipment> Equipments { get; set; }

    public MobileUnityRide()
    {
        Equipments = new List<MobilePatientEquipment>();
    }
}

public class MobilePatientEquipment
{
    public int? PatientId { get; set; }
    public int? EquipmentId { get; set; }
    public string Name { get; set; }
}