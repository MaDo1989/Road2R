/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ AssignedFromAppId Altering ↓ */

--1.
ALTER TABLE [dbo].[Ride] DROP CONSTRAINT [Ride_App]
GO

--2. (THIS PROCEDURE TAKES TIME... 30 SEC +/-)
ALTER TABLE Ride
ALTER COLUMN AssignedFromAppId varchar(255);
GO

--3.

-- =============================================
-- Author:       Yogev Strauber
-- Description:  Assign Driver To Ride And Wire Ride To RidePat
-- ALTER DATE:   19/08/2022
-- ALTER REASON: Add  AssignedFromAppId += CONCAT(', ', @assignedFromAppId)
-- =============================================
ALTER PROCEDURE [dbo].[spRideAndRidePat_AssignDriver]
(
	@ridePatId INT,
	@driverId INT,
	@assignedFromAppId INT
)
AS
BEGIN

    SET NOCOUNT ON
	DECLARE 
		@Success NVARCHAR(75)						= N'הנסיעה עודכנה בהצלחה',
		@AllReadyAssigned NVARCHAR(75)				= N'הנסיעה אליה נרשמת כבר מלאה',
		@NotExistsRidePat NVARCHAR(75)				= N'נסיעה זו בוטלה, תודה על הרצון לעזור',
		@DriverIsNotActiveNorDriving NVARCHAR(75)	= N'הנהג אינו פעיל/ אינו נוהג או שניהם',
		@Origin NVARCHAR(255)						= (SELECT Origin      FROM RidePat WHERE RidePatNum=@ridePatId),
		@Destination NVARCHAR(255)					= (SELECT Destination FROM RidePat WHERE RidePatNum=@ridePatId),
		@Date DATETIME								= (SELECT PickupTime  FROM RidePat WHERE RidePatNum=@ridePatId),
		@rideId INT									= (SELECT RideId      FROM RidePat WHERE RidePatNum=@ridePatId),
		@CreatedRideId INT							= NULL


	IF NOT EXISTS (SELECT 1	FROM RidePat WHERE RidePatNum=@ridePatId)
	BEGIN
		SELECT 1 AS IsError, @NotExistsRidePat AS Message
		RETURN;
	END

	IF (SELECT MainDriver FROM RPView WHERE RidePatNum=@ridePatId) IS NOT NULL 
	BEGIN
		SELECT 1 AS IsError, @AllReadyAssigned AS Message
		RETURN;
	END

	IF NOT EXISTS (SELECT 1 FROM Volunteer WHERE Id=@driverId AND IsActive=1 AND isDriving=1)
		BEGIN
		SELECT 1 AS IsError, @DriverIsNotActiveNorDriving AS Message
		RETURN;
		END

	IF EXISTS (SELECT 1 FROM Ride WHERE RideNum=@rideId)
		BEGIN
				UPDATE Ride SET MainDriver=@driverId, AssignedFromAppId += CONCAT(', ', @assignedFromAppId) WHERE RideNum=@rideId
				SELECT 0 AS IsError, @Success AS Message, @rideId AS RideId
		END
	ELSE
		BEGIN
			SET DATEFORMAT dmy;

			INSERT INTO Ride (Origin,Destination,Date,MainDriver, AssignedFromAppId) 
			VALUES (@origin,@Destination,@date,@driverId, @assignedFromAppId) 

			SET @CreatedRideId = (SELECT SCOPE_IDENTITY());

			UPDATE RidePat SET RideId=@CreatedRideId, lastModified=GETDATE() WHERE RidePatNum=@ridePatId

			SELECT 0 AS IsError, @Success AS Message, @CreatedRideId AS RideId

		END
END


/* ↑ AssignedFromAppId Altering ↑ */
