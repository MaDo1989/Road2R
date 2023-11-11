-- =============================================
-- Author:      Yogev Strauber
-- Create Date : 03/12/2022 @night
-- ALTER  Date : 11/11/2023 
-- ALTER  Last Reason: Change The Way We Update Volunteer From DisplayName to Id.
-- Description: Active Or Deactivate Volunteer Base on isActive Parameter
-- Returns IsSuccesfullOperation bit, and optional VolunteerWithFutureRidesIncludedToday
-- when try to deactivate volunteer
-- =============================================
ALTER     PROCEDURE [dbo].[spVolunteer_ToggleActiveness]
(
   @volunteerId INT,
   @isActive BIT
		)
AS
BEGIN

    SET NOCOUNT ON;

	IF(@isActive) = 0
	BEGIN
		IF(
			SELECT COUNT(*) FROM RIDE 
			WHERE MAINDRIVER=@volunteerId
			AND GETDATE() < DATE
		   ) = 0
			BEGIN
				UPDATE Volunteer 
				SET IsActive=@isActive, 
				lastModified=DATEADD(hour, 2, SYSDATETIME())
				WHERE Id=@volunteerId
			SELECT 
				1 AS IsSuccesfullOperation,
				0 AS VolunteerWithFutureRidesIncludedToday
			END
		ELSE
			BEGIN
			SELECT
				0 AS IsSuccesfullOperation,
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