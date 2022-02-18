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
CREATE OR ALTER PROCEDURE [dbo].[spGetCandidatesForRidePat]
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
				@DestinationSubArea INT, @pickupDay NVARCHAR(10),
				@AfterNoonString NVARCHAR(10) = ':14';

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
		AS IsSuperDriver,
		(
			SELECT COUNT(*) FROM Ride R_In
			WHERE R_In.MainDriver=R_Out.MainDriver AND CHARINDEX(':14',Date) > 0
		) AmmountOfAfterNoonRides,

				(
			SELECT COUNT(*) FROM Ride R_In
			WHERE R_In.MainDriver=R_Out.MainDriver AND CHARINDEX(':14',Date) = 0
		) AmmountOfMorningRides
		 INTO #TempScoreTable
		 FROM RIDE R_Out
		 WHERE Date BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
		 AND MainDriver IS NOT NULL

		SELECT 
		MainDriver, IsSuperDriver, AmmountOfAfterNoonRides, AmmountOfMorningRides,
		COUNT(CASE WHEN PathMatchScore = 0 THEN 1 END) AS AmmountOfPathMatchScoreOfType_0,
		COUNT(CASE WHEN PathMatchScore = 1 THEN 1 END) AS AmmountOfPathMatchScoreOfType_1,
		COUNT(CASE WHEN PathMatchScore = 2 THEN 1 END) AS AmmountOfPathMatchScoreOfType_2,
		COUNT(CASE WHEN PathMatchScore = 3 THEN 1 END) AS AmmountOfPathMatchScoreOfType_3,
		COUNT(CASE WHEN PathMatchScore = 4 THEN 1 END) AS AmmountOfPathMatchScoreOfType_4,
		COUNT(CASE WHEN IsDayMatch = 1 THEN 1 END)	   AS AmmountOfMatchByDay,
		COUNT(CASE WHEN IsDayMatch = 0 THEN 1 END)     AS AmmountOfDissMatchByDay
		INTO  #CandidatesBucketsTable FROM #TempScoreTable T
		WHERE NOT EXISTS
		(
			select * from Ride
			where MainDriver=T.MainDriver and Date 
				BETWEEN 
					IIF(IsSuperDriver = 1, @TimeWindowPastLimit_CheckRides_Super, @TimeWindowPastLimit_CheckRides_Regular)
				AND
					IIF(IsSuperDriver = 1, @TimeWindowFutureLimit_CheckRides_Super, @TimeWindowFutureLimit_CheckRides_Regular)
		)
		GROUP BY MainDriver, IsSuperDriver, AmmountOfAfterNoonRides, AmmountOfMorningRides

		SELECT
		MainDriver as Id,V.DisplayName, IsSuperDriver, AmmountOfAfterNoonRides, AmmountOfMorningRides, AmmountOfPathMatchScoreOfType_0,
		AmmountOfPathMatchScoreOfType_1, AmmountOfPathMatchScoreOfType_2, AmmountOfPathMatchScoreOfType_3,
		AmmountOfPathMatchScoreOfType_4, AmmountOfMatchByDay, AmmountOfDissMatchByDay
		FROM #CandidatesBucketsTable C INNER JOIN Volunteer V ON C.MainDriver = V.Id
			WHERE 
				AmmountOfPathMatchScoreOfType_1 +
				AmmountOfPathMatchScoreOfType_2 +
				AmmountOfPathMatchScoreOfType_3 +
				AmmountOfPathMatchScoreOfType_4		> 0

		DROP TABLE #CandidatesBucketsTable
		DROP TABLE #TempScoreTable
	END
GO

CREATE TYPE [dbo].[IntList] AS TABLE(
	item INT NOT NULL
)
GO


-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 18/02/2022
-- Description: For given list of candidates's ids provided candidate details
-- =============================================
CREATE OR ALTER PROCEDURE spGetCandidatesDetails
(
	@IDs [IntList] readonly
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON


SELECT Id, CellPhone, 
  (SELECT top 1 DATEDIFF(DAY,Date,GETDATE()) from Ride where MainDriver=V.Id AND Date <= GETDATE() order by Date desc) DaysSinceLastRide,

  (SELECT COUNT(*)
	FROM Ridepat RP INNER JOIN Ride R
	ON RP.RideId=R.RideNum
	WHERE R.MainDriver = V.Id AND PickupTime BETWEEN DATEADD(Month, -2, GETDATE()) and  GETDATE())  NumOfRides_last2Months,

  (SELECT TOP 1 DATEDIFF(d,Date,GETDATE()) from Ride where MainDriver=V.Id AND Date > GETDATE() order by Date desc)*-1 DaysUntilNextRide,
  (SELECT TOP 1 Concat(CallRecordedDate,' ', CallRecordedTime) DateAndTime FROM DocumentedCall WHERE DriverId=V.Id Order by DateAndTime desc) LatestDocumentedCallDate,

FLOOR((DATEDIFF(MONTH,V.JoinDate, GETDATE())/12.0)*4) / 4 SeniorityInYears
FROM 
	Volunteer V 
INNER JOIN @IDs ON V.Id=item

END
GO




