/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

--Patient Status ↓
CREATE TABLE RidePatPatientStatus (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PatientId INT FOREIGN KEY REFERENCES Patient(Id),
    RidePatNum INT FOREIGN KEY REFERENCES RidePat(RidePatNum),
    PatientStatus NVARCHAR(55),
    EditTimeStamp DATETIME
);
GO

ALTER VIEW [dbo].[RPView]
AS
SELECT    dbo.Patient.DisplayName, dbo.Patient.Id, dbo.Patient.CellPhone, dbo.Patient.IsAnonymous, dbo.Patient.BirthDate, dbo.Patient.Gender, dbo.RidePat.RidePatNum, dbo.RidePat.Origin, dbo.RidePat.Destination, dbo.RidePat.PickupTime, V.DisplayName AS Coordinator, 
                         dbo.RidePat.Status, dbo.RidePat.Area, dbo.RidePat.Shift, dbo.Ride.RideNum, dbo.Ride.Origin AS RideOrigin, dbo.Ride.Destination AS RideDestination, dbo.Ride.Date, dbo.Ride.MainDriver, dbo.Ride.secondaryDriver, 
                         dbo.RidePat.Remark, dbo.RidePat.OnlyEscort, dbo.Patient.EnglishName, dbo.RidePat.lastModified, driver.NoOfDocumentedRides,
						 RPPS.PatientStatus, RPPS.EditTimeStamp,
							CASE 
							WHEN (SELECT COUNT(*) FROM Ride WHERE MainDriver=driver.Id  AND Date <= GETDATE()) <= 3 THEN 1
							ELSE 0
						END
						AS IsNewDriver
FROM            dbo.Patient INNER JOIN 
                         dbo.RidePat ON dbo.Patient.DisplayName = dbo.RidePat.Patient LEFT OUTER JOIN
                         dbo.Ride ON dbo.RidePat.RideId = dbo.Ride.RideNum LEFT JOIN Volunteer V on RidePat.CoordinatorID = V.Id
						 LEFT JOIN Volunteer driver on driver.Id = dbo.Ride.MainDriver
						 LEFT JOIN RidePatPatientStatus RPPS ON RidePat.RidePatNum = RPPS.RidePatNum
GO


-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 03/02/2023
-- Description: Toggle Patient in a RidePat Status
-- =============================================
CREATE OR ALTER PROCEDURE spRidePatPatientStatus_TogglePatientStatus
(
	@PatientId INT,
	@RidePatNum INT,
	@PatientStatus nvarchar(55),
	@EditTimeStamp datetime
)
AS
BEGIN
    SET NOCOUNT ON
	
	IF NOT EXISTS (SELECT 1 FROM RidePatPatientStatus WHERE RidePatNum=@RidePatNum)
		BEGIN
			INSERT INTO RidePatPatientStatus (PatientId, RidePatNum, PatientStatus, EditTimeStamp)
			VALUES (@PatientId, @RidePatNum, @PatientStatus, @EditTimeStamp);
		END
	ELSE
		BEGIN
			UPDATE RidePatPatientStatus
			SET PatientStatus = @PatientStatus, EditTimeStamp=@EditTimeStamp
			WHERE RidePatNum=@RidePatNum
		END

		UPDATE RidePat
		SET LastModified=GETDATE()
		WHERE RidePatNum=@RidePatNum
END
GO

--Patient Status ↑

-- Start Update RidePat Remark
-- =============================================
-- Author: Yogev Strauber
-- Create Date: 17.03.2023
-- Description: Gets ridepatnum + remark description and updates it accordingly (set new lasymodified value to this record)
-- =============================================

CREATE PROCEDURE spRidePat_UpdateRemark
(
	@ridePatNum int,
	@newRemark nvarchar(255)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
    SET NOCOUNT ON

	UPDATE RidePat
	SET Remark=@newRemark
	WHERE RidePatNum=@ridePatNum
END
GO

-- End Update RidePat Remark

-- =============================================
-- Author:       Yogev Strauber
-- ALTER Date:	 06/07/2023
-- ALTER Reason: Add latestDrive
-- =============================================
ALTER procedure [dbo].[spVolunteerTypeView_GetVolunteersList]

@IsActive bit
as
begin

select r.MainDriver, r.Origin, r.Destination
into #tempNotDeletedOnly from  ridepat rp
inner join ride r
on r.RideNum=rp.RideId


SELECT v.id, MAX(r.date) AS latestDrive into #tempLatesetDrives
					FROM Volunteer v
					JOIN Ride r ON v.id = r.MainDriver
					GROUP BY v.id


if (@IsActive = 0)
	begin
				select *, (select count(*)
					from ridepat rp inner join ride r
					on rp.rideid=r.ridenum
					where r.maindriver = vtv.Id
					and pickuptime between DATEADD(Month, -2, GETDATE()) and  GETDATE()) as NumOfRides_last2Months
					,
					(
				select origin + '-'+destination from
													(
														select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
														where t.MainDriver = vtv.Id
														group by maindriver, origin, destination
														order by numberOfTimesDrove desc
														) t
					) mostCommonPath, tld.latestDrive
		from VolunteerTypeView vtv
		left join #tempLatesetDrives tld on tld.Id=vtv.Id
		where IsActive = @IsActive or IsActive = 1
		order by firstNameH

	end
else
	begin
	select *, (select count(*)
					from ridepat rp inner join ride r
					on rp.rideid=r.ridenum
					where r.maindriver = vtv.Id
					and pickuptime between DATEADD(Month, -2, GETDATE()) and  GETDATE()) as NumOfRides_last2Months
					,
					(
				select origin + '-'+destination from
													(
														select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
														where t.MainDriver = vtv.Id
														group by maindriver, origin, destination
														order by numberOfTimesDrove desc
														) t
					) mostCommonPath, tld.latestDrive
		from VolunteerTypeView vtv
		left join #tempLatesetDrives tld on tld.Id=vtv.Id
		where IsActive = @IsActive
		order by firstNameH
	end

	drop table #tempNotDeletedOnly, #tempLatesetDrives

end
