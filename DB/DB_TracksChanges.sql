/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 08/04/2022
-- Description: Gets volunteer data by his cellphone number
-- =============================================
CREATE PROCEDURE spVolunteer_GetVolunteerByCellphone
(
	@cellphone NVARCHAR(20)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	SELECT * FROM Volunteer WHERE CellPhone=@cellphone
END
GO

-- =============================================
-- Author:      <Benny, , Name>
-- Create Date: <19-4-2022, , >
-- Description: <Get the basic data for ridepat, , >
-- =============================================
CREATE PROCEDURE [dbo].[spGetBasicRidePatData]
(
    -- Add the parameters for the stored procedure here
	@daysAhead int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	SELECT rp.ridepatnum, rp.patient, p.CellPhone, p.IsAnonymous,
	       rp.Area, rp.origin, rp.destination, rp.pickuptime, rp.OnlyEscort FROM ridepat rp
    JOIN patient p ON p.DisplayName = rp.Patient
    WHERE rp.pickuptime > GETDATE() AND status = N'ממתינה לשיבוץ' AND rp.pickuptime < DATEADD(day, @daysAhead, GETDATE())  
	order by rp.pickuptime asc

END
GO

-- =============================================
-- Author:      <Benny>
-- Create Date: <20-4-2022>
-- Description: <Gets the equipment needed for future rides>
-- =============================================
CREATE PROCEDURE [dbo].[spGetEquipmentPerPatient](
@daysAhead int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
select p.DisplayName as patient, e.Name
from equipment_patient ep
join patient p on ep.PatientId = p.Id
join Equipment e on e.Id = ep.EquipmentId
where p.DisplayName in 
(
select rp.patient from ridepat rp where rp.pickuptime > GETDATE() and status = N'ממתינה לשיבוץ' and rp.pickuptime < DATEADD(day, @daysAhead, GETDATE())  
)
END
GO

-- =============================================
-- Author:      <Benny>
-- Create Date: <19-4-2022>
-- Description: <Get all the escorts for future ridapats>
-- =============================================
CREATE PROCEDURE [dbo].[spGetEscortsForRides]
(
@daysAhead int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

-- get the escorts
select [PatientInRide (RidePat)RidePatNum] as ridepatnum, PatientEscortPatientId as eid, es.DisplayName, es.isAnonymous, es.CellPhone
from [PatientEscort_PatientInRide (RidePat)] 
join escorted es on es.id = PatientEscortEscortId
where [PatientInRide (RidePat)RidePatNum] in (
select rp.ridepatnum from ridepat rp where rp.pickuptime > GETDATE() and status = N'ממתינה לשיבוץ' and rp.pickuptime < DATEADD(day, @daysAhead, GETDATE())  
)
order by ridepatnum
END
GO

-- =============================================
-- Author:      <Benny>
-- Create Date: <20-4-2022>
-- Description: <Gets the equipment needed for future rides per driver
-- =============================================
CREATE PROCEDURE [dbo].[spGetFutureEquipmentPerPatientPerDriver](
@driverId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
select p.DisplayName as patient, e.Name
from equipment_patient ep
join patient p on ep.PatientId = p.Id
join Equipment e on e.Id = ep.EquipmentId
where p.DisplayName in 
(
select rp.patient from ridepat rp
join ride r on r.RideNum = rp.RideId
where r.MainDriver = @driverId
and rp.pickuptime >= GETDATE() and status <> N'נמחקה'   
)
END
GO

-- =============================================
-- Author:      <Benny>
-- Create Date: <20-4-2022>
-- Description: Get all the escorts for future ridapats for a specific volunteer
-- =============================================
CREATE PROCEDURE [dbo].[spGetFutureEscortsForRidesOfVolunteer]
(
@driverId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

-- get the escorts
select [PatientInRide (RidePat)RidePatNum] as ridepatnum, PatientEscortPatientId as eid, es.DisplayName, es.isAnonymous, es.CellPhone
from [PatientEscort_PatientInRide (RidePat)] 
join escorted es on es.id = PatientEscortPatientId
where [PatientInRide (RidePat)RidePatNum] in (
select rp.ridepatnum from ridepat rp 
join ride r on r.ridenum = rp.rideid
where r.MainDriver = @driverId  AND pickuptime >= GETDATE() AND status <> N'נמחקה' 
)
order by ridepatnum
END
GO

-- =============================================
-- Author:      <Benny>
-- Create Date: <20-4-2022>
-- Description: <Returns all the past rides of volunteer>
-- =============================================
CREATE PROCEDURE [dbo].[spGetFutureRidesOfVolunteer]
(
@driverId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	SELECT rp.ridepatnum, rp.patient, p.CellPhone, p.IsAnonymous,
	       rp.origin, rp.destination, rp.Area, rp.pickuptime, r.RideNum, rp.OnlyEscort FROM ridepat rp
    JOIN patient p ON p.DisplayName = rp.Patient
	JOIN ride r on r.RideNum = rp.rideid
    WHERE r.maindriver = @driverId and rp.pickuptime >= GETDATE() AND status <> N'נמחקה' 
	order by rp.pickuptime desc
END
GO


-- =============================================
-- Author:      <Benny>
-- Create Date: <20-4-2022>
-- Description: <Gets the equipment needed for future rides per driver
-- =============================================
CREATE PROCEDURE [dbo].[spGetPastEquipmentPerPatientPerDriver](
@driverId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
select p.DisplayName as patient, e.Name
from equipment_patient ep
join patient p on ep.PatientId = p.Id
join Equipment e on e.Id = ep.EquipmentId
where p.DisplayName in 
(
select rp.patient from ridepat rp
join ride r on r.RideNum = rp.RideId
where r.MainDriver = @driverId
and rp.pickuptime < GETDATE() and status <> N'נמחקה'   
)
END
GO

-- =============================================
-- Author:      <Benny>
-- Create Date: <20-4-2022>
-- Description: Get all the escorts for future ridapats for a specific volunteer
-- =============================================
CREATE PROCEDURE [dbo].[spGetPastEscortsForRidesOfVolunteer]
(
@driverId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

-- get the escorts
select [PatientInRide (RidePat)RidePatNum] as ridepatnum, PatientEscortPatientId as eid, es.DisplayName, es.isAnonymous, es.CellPhone
from [PatientEscort_PatientInRide (RidePat)] 
join escorted es on es.id = PatientEscortPatientId
where [PatientInRide (RidePat)RidePatNum] in (
select rp.ridepatnum from ridepat rp 
join ride r on r.ridenum = rp.rideid
where r.MainDriver = @driverId  AND pickuptime < GETDATE() AND status <> N'נמחקה' 
)
order by ridepatnum
END
GO


-- =============================================
-- Author:      <Benny>
-- Create Date: <20-4-2022>
-- Description: <Returns all the past rides of volunteer>
-- =============================================
CREATE PROCEDURE [dbo].[spGetPastRidesOfVolunteer]
(
@driverId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	SELECT rp.ridepatnum, rp.patient, p.CellPhone, p.IsAnonymous,
	       rp.origin, rp.destination, rp.pickuptime, rp.Area,rp.OnlyEscort, r.RideNum FROM ridepat rp
    JOIN patient p ON p.DisplayName = rp.Patient
	JOIN ride r on r.RideNum = rp.rideid
    WHERE r.maindriver = @driverId and rp.pickuptime < GETDATE() AND status <> N'נמחקה' 
	order by rp.pickuptime desc
END
GO







