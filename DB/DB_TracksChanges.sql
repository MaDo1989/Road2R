/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

CREATE TYPE [dbo].[IntList] AS TABLE(
	item INT NOT NULL
)
GO

-- =============================================
-- Authors:     Dr. Benny Bornfeld & Yogev Strauber
-- Create Date: 17/02/2022
-- Description: RETURNS TABLE OF CANDIDATE TO A GIVEN RIDEPAT WITH VARIOUS SCORES TO EACH CANDIDATE
-- =============================================
	CREATE PROCEDURE [dbo].[spGetCandidatesForRidePat]
	(
		@RidePatNum INT,
		@NumOfDaysToThePast INT,					@NUmOfDaysToTheFuture INT,
		@NumOfDaysToThePast_CheckRides_Regular INT,	@NumOfDaysToTheFuture_CheckRides_Regular INT,
		@NumOfDaysToThePast_CheckRides_Super INT,   @NumOfDaysToTheFuture_CheckRides_Super INT,
		@AmountBottomLimitToBeSuperUserDriver INT,  @AmountOfRidesInNewDriverTimeWindow INT,
		@NewDriverTimeWindow INT

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
					@AfterNoonString NVARCHAR(10) = ':14',
					@pickupTime DATETIME;


			--RIDE WITHOUT NEWBIES IN 3 STEPS

			--1
			SELECT MainDriver, COUNT(*) AS NUMOFRIDES
			INTO #NUMOFRIDE_PER_VOLUNTEER
			FROM RIDE
			GROUP BY MainDriver

			--2
			SELECT MainDriver AS CandidateId
			INTO #CANDIDATES_NO_NEWBIES
			FROM #NUMOFRIDE_PER_VOLUNTEER NPV INNER JOIN Volunteer V
			ON NPV.MainDriver=V.Id
			WHERE NUMOFRIDES > @AmountOfRidesInNewDriverTimeWindow OR DATEDIFF(d,v.JoinDate, getdate()) > @NewDriverTimeWindow
		
			--3 
			SELECT *
			INTO #NO_NEWBIESRIDES
			FROM #CANDIDATES_NO_NEWBIES CNN INNER JOIN RIDE R ON CNN.CandidateId=R.MainDriver

			SELECT @Origin = origin FROM ridepat WHERE RidePatNum = @RidePatNum
			SELECT @Destination = destination FROM ridepat WHERE RidePatNum = @RidePatNum 
			SELECT @OriginSubArea = RegionId FROM Location WHERE Name =  @origin
			SELECT @DestinationSubArea = RegionId FROM Location WHERE Name =  @Destination
			SELECT @pickupDay = DATENAME(dw, pickupTime) FROM ridepat WHERE RidePatNum = @RidePatNum
			SELECT @pickupTime= pickupTime FROM ridepat WHERE RidePatNum = @RidePatNum

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
						select MainDriver from ride R_In where R_In.MainDriver=R_Out.MainDriver and
						Date BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
						group by MainDriver having COUNT(*) > @AmountBottomLimitToBeSuperUserDriver
					 ) THEN 1 ELSE 0
			END
			AS IsSuperDriver,
			(
			 CASE
				 WHEN 
				 (
					(
					(CHARINDEX(@AfterNoonString,Date) > 0 OR  CAST(Date as time) >= '12:00:00')
					 AND
					(CHARINDEX(@AfterNoonString,@pickupTime) > 0 OR  CAST(@pickupTime as time) >= '12:00:00')
					) -- = afternoon match
					OR
					(
					(CHARINDEX(@AfterNoonString,Date) = 0 AND  CAST(Date as time) < '12:00:00')
					 AND
					(CHARINDEX(@AfterNoonString,@pickupTime) = 0 AND  CAST(@pickupTime as time) < '12:00:00')
					)-- = morning match
				 )
				 THEN 1 ELSE 0
			 END

			) DayPartMatch
			 INTO #TempScoreTable
			 FROM #NO_NEWBIESRIDES R_Out
			 WHERE Date BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
			 AND MainDriver IS NOT NULL


			 --SELECT * FROM #TempScoreTable

			SELECT 
			MainDriver, IsSuperDriver,
			COUNT(CASE WHEN PathMatchScore = 0 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_0,
			COUNT(CASE WHEN PathMatchScore = 1 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_1,
			COUNT(CASE WHEN PathMatchScore = 2 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_2,
			COUNT(CASE WHEN PathMatchScore = 3 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_3,
			COUNT(CASE WHEN PathMatchScore = 4 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_4,
			COUNT(CASE WHEN IsDayMatch = 1 THEN 1 END)			 AS AmmountOfMatchByDay,
			COUNT(CASE WHEN IsDayMatch = 0 THEN 1 END)		     AS AmmountOfDisMatchByDay,
			COUNT(CASE WHEN DayPartMatch = 1 THEN 1 END)         AS AmmountOfMatchDayPart,
			COUNT(CASE WHEN DayPartMatch = 0 THEN 1 END)         AS AmmountOfDisMatchDayPart
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
			GROUP BY MainDriver, IsSuperDriver

			SELECT
			MainDriver as Id,V.DisplayName, IsSuperDriver, AmmountOfMatchDayPart, AmmountOfDisMatchDayPart, AmmountOfPathMatchScoreOfType_0,
			AmmountOfPathMatchScoreOfType_1, AmmountOfPathMatchScoreOfType_2, AmmountOfPathMatchScoreOfType_3,
			AmmountOfPathMatchScoreOfType_4, AmmountOfMatchByDay, AmmountOfDisMatchByDay
			FROM #CandidatesBucketsTable C INNER JOIN Volunteer V ON C.MainDriver = V.Id
				WHERE 
					AmmountOfPathMatchScoreOfType_1 +
					AmmountOfPathMatchScoreOfType_2 +
					AmmountOfPathMatchScoreOfType_3 +
					AmmountOfPathMatchScoreOfType_4		> 0


			DROP TABLE #CandidatesBucketsTable, #TempScoreTable, #NUMOFRIDE_PER_VOLUNTEER, #CANDIDATES_NO_NEWBIES, #NO_NEWBIESRIDES
		
		END		
GO

-- =============================================
-- Authors:     Dr. Benny Bornfeld & Yogev Strauber
-- Create Date: 04/03/2022
-- Description: RETURNS TABLE OF CANDIDATE TO A GIVEN RIDEPAT WITH VARIOUS SCORES TO EACH NEWBIE CANDIDATE
-- =============================================
	CREATE PROCEDURE [dbo].[spGetNoobsCandidatesForRidePat]
	(
		@RidePatNum INT,
		@NumOfDaysToThePast INT,					@NUmOfDaysToTheFuture INT,
		@NumOfDaysToThePast_CheckRides_Regular INT,	@NumOfDaysToTheFuture_CheckRides_Regular INT,
		@NumOfDaysToThePast_CheckRides_Super INT,   @NumOfDaysToTheFuture_CheckRides_Super INT,
		@AmountBottomLimitToBeSuperUserDriver INT,  @AmountOfRidesInNewDriverTimeWindow INT,
		@NewDriverTimeWindow INT

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
					@AfterNoonString NVARCHAR(10) = ':14',
					@pickupTime DATETIME;



			SELECT MainDriver, COUNT(*) AS NUMOFRIDES
			INTO #NUMOFRIDE_PER_VOLUNTEER
			FROM RIDE
			GROUP BY MainDriver

			SELECT MainDriver AS CandidateId
			INTO #CANDIDATES_NEWBIES_NO_ZERORIDES
			FROM #NUMOFRIDE_PER_VOLUNTEER NPV INNER JOIN Volunteer V
			ON NPV.MainDriver=V.Id
			WHERE NUMOFRIDES <= @AmountOfRidesInNewDriverTimeWindow AND DATEDIFF(d,v.JoinDate, getdate()) < @NewDriverTimeWindow
		
			SELECT *
			INTO #NO_NEWBIESRIDES
			FROM #CANDIDATES_NEWBIES_NO_ZERORIDES CNNZ INNER JOIN RIDE R ON CNNZ.CandidateId=R.MainDriver

			SELECT @Origin = origin FROM ridepat WHERE RidePatNum = @RidePatNum
			SELECT @Destination = destination FROM ridepat WHERE RidePatNum = @RidePatNum 
			SELECT @OriginSubArea = RegionId FROM Location WHERE Name =  @origin
			SELECT @DestinationSubArea = RegionId FROM Location WHERE Name =  @Destination
			SELECT @pickupDay = DATENAME(dw, pickupTime) FROM ridepat WHERE RidePatNum = @RidePatNum
			SELECT @pickupTime= pickupTime FROM ridepat WHERE RidePatNum = @RidePatNum

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
						select MainDriver from ride R_In where R_In.MainDriver=R_Out.MainDriver and
						Date BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
						group by MainDriver having COUNT(*) > @AmountBottomLimitToBeSuperUserDriver
					 ) THEN 1 ELSE 0
			END
			AS IsSuperDriver,
			(
			 CASE
				 WHEN 
				 (
					(
					(CHARINDEX(@AfterNoonString,Date) > 0 OR  CAST(Date as time) >= '12:00:00')
					 AND
					(CHARINDEX(@AfterNoonString,@pickupTime) > 0 OR  CAST(@pickupTime as time) >= '12:00:00')
					) -- = afternoon match
					OR
					(
					(CHARINDEX(@AfterNoonString,Date) = 0 AND  CAST(Date as time) < '12:00:00')
					 AND
					(CHARINDEX(@AfterNoonString,@pickupTime) = 0 AND  CAST(@pickupTime as time) < '12:00:00')
					)-- = morning match
				 )
				 THEN 1 ELSE 0
			 END

			) DayPartMatch
			 INTO #TempScoreTable
			 FROM #NO_NEWBIESRIDES R_Out
			 WHERE Date BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
			 AND MainDriver IS NOT NULL


			 --SELECT * FROM #TempScoreTable

			SELECT 
			MainDriver, IsSuperDriver,
			COUNT(CASE WHEN PathMatchScore = 0 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_0,
			COUNT(CASE WHEN PathMatchScore = 1 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_1,
			COUNT(CASE WHEN PathMatchScore = 2 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_2,
			COUNT(CASE WHEN PathMatchScore = 3 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_3,
			COUNT(CASE WHEN PathMatchScore = 4 THEN 1 END)		 AS AmmountOfPathMatchScoreOfType_4,
			COUNT(CASE WHEN IsDayMatch = 1 THEN 1 END)			 AS AmmountOfMatchByDay,
			COUNT(CASE WHEN IsDayMatch = 0 THEN 1 END)		     AS AmmountOfDisMatchByDay,
			COUNT(CASE WHEN DayPartMatch = 1 THEN 1 END)         AS AmmountOfMatchDayPart,
			COUNT(CASE WHEN DayPartMatch = 0 THEN 1 END)         AS AmmountOfDisMatchDayPart
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
			GROUP BY MainDriver, IsSuperDriver


			SELECT V.Id, COUNT(RIDENUM) NumOfRides
			INTO #NEBIEWSWITHZERORIDES
			FROM Volunteer V LEFT JOIN RIDE R ON V.Id=R.MainDriver
			WHERE DATEDIFF(d,V.JoinDate, getdate()) < @NewDriverTimeWindow
			GROUP BY V.Id
			HAVING  COUNT(RIDENUM) = 0 

			SELECT
			MainDriver as Id,V.DisplayName, IsSuperDriver, AmmountOfMatchDayPart, AmmountOfDisMatchDayPart, AmmountOfPathMatchScoreOfType_0,
			AmmountOfPathMatchScoreOfType_1, AmmountOfPathMatchScoreOfType_2, AmmountOfPathMatchScoreOfType_3,
			AmmountOfPathMatchScoreOfType_4, AmmountOfMatchByDay, AmmountOfDisMatchByDay
			FROM #CandidatesBucketsTable C INNER JOIN Volunteer V ON C.MainDriver = V.Id

				UNION
			
			SELECT 
			V.Id, V.DisplayName, 0 AS IsSuperDriver,
			0 AS AmmountOfMatchDayPart, 0 AS AmmountOfDisMatchDayPart, 0 AS AmmountOfPathMatchScoreOfType_0,
			0 AS AmmountOfPathMatchScoreOfType_1, 0 AS AmmountOfPathMatchScoreOfType_2, 0 AS AmmountOfPathMatchScoreOfType_3,
			0 AS AmmountOfPathMatchScoreOfType_4, 0 AS AmmountOfMatchByDay, 0 AS AmmountOfDisMatchByDay
			FROM #NEBIEWSWITHZERORIDES N INNER JOIN Volunteer V ON N.Id = V.Id



			DROP TABLE #CandidatesBucketsTable, #TempScoreTable, #NUMOFRIDE_PER_VOLUNTEER, #CANDIDATES_NEWBIES_NO_ZERORIDES, #NO_NEWBIESRIDES
		
		END	
GO

-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 18/02/2022
-- Description: For given list of candidates's ids provided candidate details
-- =============================================
CREATE PROCEDURE [dbo].[spGetCandidatesDetails]
(
	@IDs [IntList] readonly
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON


SELECT Id, CellPhone, CityCityName,
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



