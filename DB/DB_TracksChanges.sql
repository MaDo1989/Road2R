/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/****** Object:  StoredProcedure [dbo].[spVolunteer_deactivation]    Script Date: 12/3/2022 7:27:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 03/12/2022 @night
-- Description: Active Or Deactivate Volunteer Base on isActive Parameter
-- Returns IsSuccesfulOperation bit, and optional VolunteerWithFutureRidesIncludedToday
-- when try to deactivate volunteer
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[spVolunteer_ToggleActiveness]
(
   @displayName NVARCHAR(255),
   @isActive BIT
		)
AS
BEGIN

    SET NOCOUNT ON;
	DECLARE @volunteerId INT = (SELECT Id from volunteer where Displayname=@displayName)

	IF(@isActive) = 0
	BEGIN
		IF(
			SELECT COUNT(*) FROM RIDE 
			WHERE MAINDRIVER=14430
			AND  CONVERT(VARCHAR, GETDATE(), 110) <= CONVERT(VARCHAR, Date, 110) 
		   ) = 0
			BEGIN
				UPDATE Volunteer 
				SET IsActive=@isActive, 
				lastModified=DATEADD(hour, 2, SYSDATETIME())
				WHERE Id=@volunteerId
			SELECT 
				1 AS IsSuccesfulOperation,
				0 AS VolunteerWithFutureRidesIncludedToday
			END
		ELSE
			BEGIN
			SELECT
				0 AS IsSuccesfulOperation,
				1 AS VolunteerWithFutureRidesIncludedToday
			END
	END
	ELSE
	BEGIN 
			UPDATE Volunteer 
			SET IsActive=@isActive, 
			lastModified=DATEADD(hour, 2, SYSDATETIME())
			WHERE Id=@volunteerId

			SELECT
				1 AS IsSuccesfullOperation
END
END