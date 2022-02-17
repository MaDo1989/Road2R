/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

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

-- =============================================
-- Authors:     Dr. Benny Bornfeld & Yogev Strauber
-- Create Date: 17/02/2022
-- Description: RETURNS TABLE OF CANDIDATE TO A GIVEN RIDEPAT WITH VARIOUS SCORES TO EACH CANDIDATE
-- =============================================
CREATE PROCEDURE spGetCandidatesForRidePat
(
	@RidePatNum INT,
	@NumOfDaysToThePast INT, @NUmOfDaysToTheFuture INT,
	@NumOfDaysToThePast_CheckRides_Regular INT,	@NumOfDaysToTheFuture_CheckRides_Regular INT,
	@NumOfDaysToThePast_CheckRides_Super INT,   @NumOfDaysToTheFuture_CheckRides_Super INT,
	@AmountBottomLimitToBeSuperUserDriver INT

)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

 
DECLARE 
        @TimeWindowPastLimit DATETIME = DATEADD(DAY, -@NumOfDaysToThePast, GETDATE()),
		@TimeWindowFutureLimit DATETIME = DATEADD(DAY, @NumOfDaysToTheFuture, GETDATE()),
		@TimeWindowPastLimit_CheckRides_Regular DATETIME = DATEADD(DAY, -@NumOfDaysToThePast_CheckRides_Regular, GETDATE()),
		@TimeWindowFutureLimit_CheckRides_Regular DATETIME = DATEADD(DAY, @NumOfDaysToTheFuture_CheckRides_Regular, GETDATE()),
		@TimeWindowPastLimit_CheckRides_Super DATETIME = DATEADD(DAY, -@NumOfDaysToThePast_CheckRides_Super, GETDATE()),
		@TimeWindowFutureLimit_CheckRides_Super DATETIME = DATEADD(DAY, @NumOfDaysToTheFuture_CheckRides_Super, GETDATE()),
		@Origin NVARCHAR(50), @Destination NVARCHAR(50),  @OriginSubArea INT,
		@DestinationSubArea INT, @pickupDay NVARCHAR(10)

SELECT @Origin = origin FROM ridepat WHERE RidePatNum = @RidePatNum
SELECT @Destination = destination FROM ridepat WHERE RidePatNum = @RidePatNum 
SELECT @OriginSubArea = RegionId FROM Location WHERE Name =  @origin
SELECT @DestinationSubArea = RegionId FROM Location WHERE Name =  @Destination
SELECT @pickupDay = DATENAME(dw, pickupTime) FROM ridepat WHERE RidePatNum = @RidePatNum

SELECT *, 
CASE
    WHEN Origin = @Origin and Destination = @Destination THEN 4
	WHEN Origin = @Destination and Destination = @Origin THEN 3
	WHEN Destination = @Destination 
		 and 
		 Origin <> @Origin
		 and exists (select Name from Location where RegionId = @OriginSubArea and isActive = 1 and Name=Origin) THEN 2
	WHEN Origin = @Origin 
		and
		Destination <> @Destination and exists
			(select Name from Location where RegionId = @DestinationSubArea and isActive = 1 and Name=Destination) THEN 2
	WHEN exists (select Name from Location where RegionId = @OriginSubArea and isActive = 1 and Name=Origin) 
		and exists (select Name from Location where RegionId = @destinationSubArea and isActive = 1 and Name=Destination) THEN 1
    ELSE 0
 END as PathMatchScore,
  CASE
	 WHEN @pickupDay = DATENAME(DW, Date) THEN 1 ELSE 0
 END
 as IsDayMatch
 ,
  CASE
	WHEN exists
		(
			select MainDriver from ride R_In where R_In.MainDriver=R_Out.MainDriver and Date BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
			group by MainDriver having COUNT(*) > @AmountBottomLimitToBeSuperUserDriver
		 ) THEN 1 ELSE 0
END
AS IsSuperDriver
 INTO #TempScoreTable
 FROM RIDE R_Out
 WHERE Date BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
 AND MainDriver IS NOT NULL

SELECT 
MainDriver, IsSuperDriver,
COUNT(CASE WHEN PathMatchScore = 0 THEN 1 END) AS AmmountOfPathMatchScore_0,
COUNT(CASE WHEN PathMatchScore = 1 THEN 1 END) AS AmmountOfPathMatchScore_1,
COUNT(CASE WHEN PathMatchScore = 2 THEN 1 END) AS AmmountOfPathMatchScore_2,
COUNT(CASE WHEN PathMatchScore = 3 THEN 1 END) AS AmmountOfPathMatchScore_3,
COUNT(CASE WHEN PathMatchScore = 4 THEN 1 END) AS AmmountOfPathMatchScore_4,
COUNT(CASE WHEN IsDayMatch = 1 THEN 1 END) AS AmmountOfDayMatch_True,
COUNT(CASE WHEN IsDayMatch = 0 THEN 1 END) AS AmmountOfDayMatch_False
INTO  #CandidatesBucketsTable FROM #TempScoreTable T
WHERE NOT EXISTS
(
	select * from ride
	where MainDriver=T.MainDriver and Date 
		BETWEEN 
			IIF(IsSuperDriver = 1, @TimeWindowPastLimit_CheckRides_Super, @TimeWindowPastLimit_CheckRides_Regular)
		AND
			IIF(IsSuperDriver = 1, @TimeWindowFutureLimit_CheckRides_Super, @TimeWindowFutureLimit_CheckRides_Regular)
)
GROUP BY MainDriver, IsSuperDriver

select * from #CandidatesBucketsTable
where AmmountOfPathMatchScore_1+AmmountOfPathMatchScore_2+AmmountOfPathMatchScore_3+AmmountOfPathMatchScore_4>0

DROP TABLE #CandidatesBucketsTable
DROP TABLE #TempScoreTable
END
GO
