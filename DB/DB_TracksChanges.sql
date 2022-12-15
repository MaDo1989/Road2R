/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

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
GO

-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 14/12/2022 @night
-- Description: Active Or Deactivate Volunteer's isDriving
-- Returns IsSuccesfullOperation bit, and optional VolunteerWithFutureRidesIncludedToday
-- when try to deactivate volunteer
-- =============================================
CREATE    PROCEDURE [dbo].[spVolunteer_ToggleIsDrive]
(
   @displayName NVARCHAR(255),
   @isDriving BIT
		)
AS
BEGIN

    SET NOCOUNT ON;
	DECLARE @volunteerId INT = (SELECT Id from volunteer where Displayname=@displayName)

	IF(@isDriving) = 0
	BEGIN
		IF(
			SELECT COUNT(*) FROM RIDE 
			WHERE MAINDRIVER=@volunteerId
			AND  CONVERT(VARCHAR, GETDATE(), 110) <= CONVERT(VARCHAR, Date, 110) 
		   ) = 0
			BEGIN
				UPDATE Volunteer 
				SET IsActive=@isDriving, 
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
			SET isDriving=@isDriving, 
			lastModified=DATEADD(hour, 2, SYSDATETIME())
			WHERE Id=@volunteerId

			SELECT
				1 AS IsSuccesfullOperation

END
END
GO

-- Remove calculation of most common regional path
ALTER procedure [dbo].[spVolunteerTypeView_GetVolunteersList]

@IsActive bit
as
begin
select r.MainDriver, r.Origin, r.Destination into #tempNotDeletedOnly from  ridepat rp
inner join ride r
on r.RideNum=rp.RideId

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
														where t.MainDriver = id
														group by maindriver, origin, destination
														order by numberOfTimesDrove desc
														) t
					) mostCommonPath
		from VolunteerTypeView vtv
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
														where t.MainDriver = id
														group by maindriver, origin, destination
														order by numberOfTimesDrove desc
														) t
					) mostCommonPath,
					( 
				select dbo.SVF_GET_REGION_NAME(
					(
				select origin from
									(
										select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
										where t.MainDriver = id
										group by maindriver, origin, destination
										order by numberOfTimesDrove desc
										) t)
									 )
				 ) AS mostCommonRegionalOrigin,
				 				( 
	select dbo.SVF_GET_REGION_NAME(
				(
				select destination from
										(
										select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
										where t.MainDriver = id
										group by maindriver, origin, destination
										order by numberOfTimesDrove desc
										) t)
									 )
				 ) AS mostCommonRegionalDestination
		from VolunteerTypeView vtv
		where IsActive = @IsActive
		order by firstNameH
	end

	drop table #tempNotDeletedOnly 

end
