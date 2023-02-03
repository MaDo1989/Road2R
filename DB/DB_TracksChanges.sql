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


