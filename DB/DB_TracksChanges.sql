
/*NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD*/





/*DO NOT DEPLOY IT YET*/

CREATE PROCEDURE spVolunteer_GetActiveVolunteers_NotDriversYet 
@daysSinceJoin int  as  
	BEGIN 
		select * from volunteer 
		where  ( 
				CASE WHEN JoinDate is not null THEN DATEDIFF(DAY, JoinDate, getdate())
				end
			   ) > @daysSinceJoin  
			and   IsActive=1
			and   Id not in (select distinct MainDriver from ride where MainDriver is not null)
	END
	GO

CREATE procedure [dbo].[spVolunteer_GetActiveVolunteers_NotDriversYet] 
@daysSinceJoin int
as
	BEGIN
	select * from volunteer v
	where 
		(
			CASE
				WHEN JoinDate is not null
					THEN DATEDIFF(DAY, JoinDate, getdate())
			end
		) > @daysSinceJoin
	and	IsActive=1
	and	not exists (select distinct MainDriver from ride where MainDriver is not null and maindriver=v.id)
END
GO
/**************************************************************************************DO NOT DEPLOY IT YET ↓*/

/*ADD R ENERAL PATH)*/
--1. CREATE TABLE REGION














/**************************************************************************************DO NOT DEPLOY IT YET ↑*/
/*DO NOT DEPLOY IT YET*/
-- =============================================
-- Author:      <Benny Bornfeld>
-- Create Date: <5-sep-2021 >
-- Description: <get candidates for rides >
-- =============================================

/****** Object:  StoredProcedure [dbo].[spGetRideCandidates]    Script Date: 12/3/2021 12:11:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spGetRideCandidates]
(
	@RidePatNum AS INT
    -- Add the parameters for the stored procedure here
    --<@Param1, sysname, @p1> <Datatype_For_Param1, , int> = <Default_Value_For_Param1, , 0>,
    --<@Param2, sysname, @p2> <Datatype_For_Param2, , int> = <Default_Value_For_Param2, , 0>
)
AS
BEGIN
DECLARE @lookBackDaysPeriod AS INT = 180
DECLARE @searchTime AS DATETIME = GETDATE()
DECLARE @noOfferDaysWindow AS INT = 10
DECLARE @superUserDrives AS INT = 15
--DECLARE @RidePatNum AS INT = 42106

DECLARE @origin AS NVARCHAR(50)
SELECT @origin = [origin]
FROM ridepat
WHERE RidePatNum = @RidePatNum
PRINT @origin

DECLARE @destination AS NVARCHAR(50)
SELECT @destination = [destination]
FROM ridepat
WHERE RidePatNum = @RidePatNum 
PRINT @destination

DECLARE @pickupDay AS char(10)
SELECT @pickupDay = datename(dw,[pickupTime])
FROM ridepat
WHERE RidePatNum = @RidePatNum
PRINT @pickupDay

declare @originSubArea AS INT
SELECT @originSubArea = [Remarks]
FROM Location
WHERE [Name] =  @origin
PRINT @originSubArea

declare @destinationSubArea AS INT
SELECT @destinationSubArea = [Remarks]
FROM Location
WHERE [Name] =  @destination
PRINT @destinationSubArea

--select Name from Location where remarks = @originSubArea and isActive = 1
--select Name from Location where remarks = @destinationSubArea and isActive = 1

select MainDriver, MAX(v.DisplayName) as DisplayName, 
				   MAX(v.cellPhone) as cellPhone,
				   MAX(v.cityCityName) as city,
				   MAX(v.joinYear) as joinYear,
                   MAX(pathMatch) as maxPathMatch,
				   MAX(dayMatch) as dayMatch,
				   MAX(superDriver) as superUser,
				   MIN(ABS(DATEDIFF(day, @searchTime, [Date]))) as closestRideInDays,
				   (MAX(pathMatch) + MAX(dayMatch)) as totalScore
 from
(select rideNum, MainDriver, Origin, Destination, [Date],
 CASE
    WHEN Origin = @origin and Destination = @destination THEN 3
	WHEN Destination = @destination and Origin <> @origin           and Origin in (select Name from Location where remarks = @originSubArea and isActive = 1) THEN 2
	WHEN Origin = @origin           and Destination <> @destination and Destination in (select Name from Location where remarks = @destinationSubArea and isActive = 1) THEN 2
	WHEN origin in (select Name from Location where remarks = @originSubArea and isActive = 1) and
	     destination in (select Name from Location where remarks = @destinationSubArea and isActive = 1) THEN 1
    ELSE 0
 END
as pathMatch,
 CASE
	 WHEN @pickupDay = datename(dw,[Date]) THEN 1
	 ELSE 0
 END
 as dayMatch,
 CASE
	WHEN MainDriver in	(select MainDriver from ride 
	     WHERE [Date] >= DATEADD(day,-@lookBackDaysPeriod, @searchTime) AND [Date] <= @searchTime
		 group by MainDriver having count(*) > @superUserDrives) THEN 1
	ELSE 0
END
as superDriver
from ride
WHERE [Date] >= DATEADD(day,-@lookBackDaysPeriod, @searchTime)
and MainDriver is not null) as x
join Volunteer v on v.Id = MainDriver
where v.IsActive = 1
group by MainDriver
having ( (MAX(pathMatch) > 1 or MAX(dayMatch) > 0) and MAX([Date]) < DATEADD(day,-@noOfferDaysWindow, @searchTime)) 
order by totalScore desc



--****************************************
--Get the number of Rides for each candidate in the last period
--****************************************

--DECLARE @lookBackDaysPeriod AS INT = 180
--DECLARE @searchTime AS DATETIME = GETDATE()
--DECLARE @noOfferDaysWindow AS INT = 10
--DECLARE @superUserDrives AS INT = 15

--select MainDriver, count(*) as cnt
--from ride 
--join volunteer v on v.id = MainDriver
--WHERE [Date] >= DATEADD(day,-@lookBackDaysPeriod, @searchTime) AND [Date] <= @searchTime
--and v.isActive = 1
--group by MainDriver
--order by cnt desc


END

--THE LAST PROGRAMMER WHO 

--if (select RideID from RidePat where RidePatNum=@RPid) is not null
	--update RidePat set Status=(select TOP 1 statusStatusName from status_Ride where RideRideNum=@Rid order by [Timestamp] desc) where RideId=@Rid
	--else update RidePat set Status=N'ממתינה לשיבוץ' where RidePatNum=@RPid
--end

--dont forget to back volunteer table
update volunteer 
	set NoOfDocumentedRides = (select count(*) from ride 
								where MainDriver=Id
								and exists (select * from ridepat where RideId=RideNum))
GO






