



/****** Object:  StoredProcedure [dbo].[spUnityRide_UpdateDateAndTime]    Script Date: 06/06/2024 19:44:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Gilad
-- Create Date: 08/12/23
-- Description: to update the time of spesific ride in unityride
-- ALTER Date 19/05/24 - try to avoid duplicated rides when change time values. - Gilad
-- =============================================
ALTER PROCEDURE [dbo].[spUnityRide_UpdateDateAndTime]
(
    -- Add the parameters for the stored procedure here
	@editedTime DATETIME,
	@unityRideId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	DECLARE @PatientId INT;
    DECLARE @Origin NVARCHAR(255);
    DECLARE @Destination NVARCHAR(255);

	SELECT 
        @PatientId = PatientId,
        @Origin = Origin,
        @Destination = Destination
	FROM UnityRide
	WHERE RidePatNum = @unityRideId;


			  -- Check for duplicates
    IF EXISTS (
        SELECT 1
        FROM UnityRide
        WHERE 
            PatientId = @PatientId AND 
            pickupTime =@editedTime  AND 
            RidePatNum != @unityRideId  -- Exclude the current ride
    )
    BEGIN
        SELECT -2 AS 'RidePatNum', 'Duplicate ride exists with the same values' AS 'Message';
        RETURN;
    END


BEGIN TRAN UpdateUnityRideTime

UPDATE UnityRide
SET PickupTime = @editedTime, lastModified = GETDATE()
where RidePatNum=@unityRideId

DECLARE @rowCount INT = 0 ;
SET @rowCount = @@ROWCOUNT;



IF @rowCount>0
select * from UnityRide where RidePatNum=@unityRideId
ELSE
select -1 as 'RidePatNum'

COMMIT TRAN UpdateUnityRideTime
END




---------------------------------------------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[spSetNewUnityRide]    Script Date: 03/06/2024 18:02:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <09/12/23>
-- Description: <this sp is for insert data to unity ride and [PatientEscort_PatientInRide (RidePat)] if need to. >
-- ALTER Date: <03/06/24>
-- ALTER Description: < add status to ride (fix some bug)>
-- =============================================
ALTER PROCEDURE [dbo].[spSetNewUnityRide]
(
    -- Add the parameters for the stored procedure here
	@patientName nvarchar(255),
	@patientId int,
	@origin nvarchar(255),
	@destination nvarchar(255),
	@pickupTime dateTime,
	@remark nvarchar(255),
	@onlyEscort bit,
	@area nvarchar(50),
	@isAnonymous bit,
	@coorName nvarchar(255),
	@driverName nvarchar(255),
	@amountOfEscorts int

	
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    

    -- Insert statements for procedure here
DECLARE @patientCellPhone varchar(50) = (select CellPhone from patient where Id = @patientId)
DECLARE @patientGender nvarchar(50) = (select Gender from patient where Id = @patientId)
DECLARE @patientBirthDate date = (select BirthDate from patient where Id = @patientId)
DECLARE @coorId int = (select id from volunteer where displayName like @coorName)
DECLARE @driverId int = case when @driverName is null then null else (select id from volunteer where displayName like @driverName) end
DECLARE @driverPhone varchar(11) = (select cellphone from volunteer where id= @driverId)
DECLARE @NoOfDocumentedRides int = (select NoOfDocumentedRides from volunteer where id= @driverId)
DECLARE @isNewDriver bit = case when (select count(*) from UnityRide where pickupTime<=GETDATE() and MainDriver=@driverId)<=3 then 1 else 0 end
DECLARE @status Nvarchar(50) = case when @driverId is null then N'ממתינה לשיבוץ' else N'שובץ נהג' end

DECLARE @AmountOfEquipments int = 0
SET @AmountOfEquipments  = case when(select count(PatientId) from Equipment_Patient where PatientId = @patientId group by PatientId) is null then 0 ELSE 
(select count(PatientId) from Equipment_Patient where PatientId = @patientId group by PatientId)
end

	IF(@driverId is not null)
	Update Volunteer
	SET NoOfDocumentedRides = NoOfDocumentedRides + 1
	where Id = @driverId


	DECLARE @FlagVar int =-1
	set @FlagVar= (select top 1 RidepatNum
	from unityRide
	where PatientId = @patientId and pickupTime = @pickupTime and Origin like @origin
	and Destination like @destination and IsAnonymous = 0 and Status not like N'נמחקה' )
if @FlagVar is null 
BEGIN
set @driverName = case when @driverName='' then null else @driverName end

Insert into UnityRide
(
PatientName,
PatientCellPhone,
PatientId,
PatientGender,
PatientBirthDate,
AmountOfEscorts,
AmountOfEquipments,
Origin,
Destination,
pickupTime,
Coordinator,
Remark,
Area,
OnlyEscort,
lastModified,
CoordinatorID,
MainDriver,
DriverName,
DriverCellPhone,
NoOfDocumentedRides,
IsAnonymous,
IsNewDriver,
Status
)
values (
@patientName,
@patientCellPhone,
@patientId,
@patientGender,
@patientBirthDate,
@amountOfEscorts,
@AmountOfEquipments,
@origin,
@destination,
@pickupTime,
@coorName,
@remark,
@area,
@onlyEscort,
GETDATE(),
@coorId,
@driverId,
@driverName,
@driverPhone,
@NoOfDocumentedRides,
@isAnonymous,
@isNewDriver,
@status
)
Select SCOPE_IDENTITY() 'RidePatNum'
END
else
select -1 'RidePatNum'

END











---------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spUnityRide_UpdateDateAndTime]    Script Date: 19/05/2024 14:09:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Gilad
-- Create Date: 08/12/23
-- Description: to update the time of spesific ride in unityride
-- ALTER Date 19/05/24 - try to avoid duplicated rides when change time values. - Gilad
-- =============================================
ALTER PROCEDURE [dbo].[spUnityRide_UpdateDateAndTime]
(
    -- Add the parameters for the stored procedure here
	@editedTime DATETIME,
	@unityRideId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	DECLARE @PatientId INT;
    DECLARE @Origin NVARCHAR(255);
    DECLARE @Destination NVARCHAR(255);

	SELECT 
        @PatientId = PatientId,
        @Origin = Origin,
        @Destination = Destination
	FROM UnityRide
	WHERE RidePatNum = @unityRideId;


			  -- Check for duplicates
    IF EXISTS (
        SELECT 1
        FROM UnityRide
        WHERE 
            PatientId = @PatientId AND 
            pickupTime =@editedTime  AND 
            Origin = @Origin AND 
            Destination = @Destination AND
            RidePatNum != @unityRideId  -- Exclude the current ride
    )
    BEGIN
        SELECT -2 AS 'RidePatNum', 'Duplicate ride exists with the same values' AS 'Message';
        RETURN;
    END


BEGIN TRAN UpdateUnityRideTime

UPDATE UnityRide
SET PickupTime = @editedTime, lastModified = GETDATE()
where RidePatNum=@unityRideId

DECLARE @rowCount INT = 0 ;
SET @rowCount = @@ROWCOUNT;



IF @rowCount>0
select * from UnityRide where RidePatNum=@unityRideId
ELSE
select -1 as 'RidePatNum'

COMMIT TRAN UpdateUnityRideTime
END




---------------------------------------------------------------------------------------------------------------------------------



ALTER TABLE UnityRide
DROP CONSTRAINT FK__UnityRide__Patie__4D1564AE; -- Drop the existing foreign key constraint

ALTER TABLE UnityRide
ADD CONSTRAINT FK__UnityRide__Patie__4D1564AE
FOREIGN KEY (PatientName) REFERENCES Patient(displayName)
ON UPDATE CASCADE; -- Add the foreign key constraint with cascade update





---------------------------------------------------------------------------------------------------------------------------------



-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <21/04/2024>
-- Description: <to get return ride after delete some ride, to ask the user if delete the return ride.>
-- =============================================
ALTER  PROCEDURE spGetReturnRide_UnityRide
(
    -- Add the parameters for the stored procedure here
    -- the originals parametes without switch!!! 
		@UnityRideID int 
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here


	--THE SWITCH IS HERE !!!!
	DECLARE @dest nvarchar(55) = (select origin from UnityRide where RidePatNum = @UnityRideID)
	DECLARE @origin nvarchar(55) = (select destination from UnityRide where RidePatNum = @UnityRideID)
	--THE SWITCH IS HERE !!!!
	DECLARE @pickupTime datetime = (select pickupTime from UnityRide where RidePatNum = @UnityRideID)
	DECLARE @patientName nvarchar(55) = (select patientName from UnityRide where RidePatNum = @UnityRideID)

    select * from unityRide where destination like @dest
	and origin like @origin
	and CONVERT(date, pickupTime) like CONVERT(date, @pickupTime)
	and patientName like @patientName
	and Status not like N'נמחקה'
END
GO



---------------------------------------------------------------------------------------------------------------------------------


-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <24/04/24>
-- Description: <temp to try to get candidate after rides tables is united ! with unityRide>
-- =============================================
CREATE PROCEDURE spGetNoobsCandidatesForUnityRide
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

    -- Insert statements for procedure here
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
    FROM UnityRide
    GROUP BY MainDriver

    SELECT MainDriver AS CandidateId
    INTO #CANDIDATES_NEWBIES_NO_ZERORIDES
    FROM #NUMOFRIDE_PER_VOLUNTEER NPV INNER JOIN Volunteer V
    ON NPV.MainDriver=V.Id
    WHERE NUMOFRIDES <= @AmountOfRidesInNewDriverTimeWindow AND DATEDIFF(d,v.JoinDate, getdate()) < @NewDriverTimeWindow

    SELECT *
    INTO #NO_NEWBIESRIDES
    FROM #CANDIDATES_NEWBIES_NO_ZERORIDES CNNZ INNER JOIN UnityRide R ON CNNZ.CandidateId=R.MainDriver

    SELECT @Origin = origin FROM UnityRide WHERE RidePatNum = @RidePatNum
    SELECT @Destination = destination FROM UnityRide WHERE RidePatNum = @RidePatNum 
    SELECT @OriginSubArea = RegionId FROM Location WHERE Name =  @origin
    SELECT @DestinationSubArea = RegionId FROM Location WHERE Name =  @Destination
    SELECT @pickupDay = DATENAME(dw, pickupTime) FROM UnityRide WHERE RidePatNum = @RidePatNum
    SELECT @pickupTime= pickupTime FROM UnityRide WHERE RidePatNum = @RidePatNum

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
         WHEN @pickupDay = DATENAME(DW, pickupTime) THEN 1 ELSE 0
     END
     as IsDayMatch
     ,
      CASE
        WHEN exists
            (
                select MainDriver from UnityRide R_In where R_In.MainDriver=R_Out.MainDriver and
                pickupTime BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
                group by MainDriver having COUNT(*) > @AmountBottomLimitToBeSuperUserDriver
             ) THEN 1 ELSE 0
    END
    AS IsSuperDriver,
    (
     CASE
         WHEN 
         (
            (
            (CHARINDEX(@AfterNoonString,pickupTime) > 0 OR  CAST(pickupTime as time) >= '12:00:00')
             AND
            (CHARINDEX(@AfterNoonString,@pickupTime) > 0 OR  CAST(@pickupTime as time) >= '12:00:00')
            ) -- = afternoon match
            OR
            (
            (CHARINDEX(@AfterNoonString,pickupTime) = 0 AND  CAST(pickupTime as time) < '12:00:00')
             AND
            (CHARINDEX(@AfterNoonString,@pickupTime) = 0 AND  CAST(@pickupTime as time) < '12:00:00')
            )-- = morning match
         )
         THEN 1 ELSE 0
     END

    ) DayPartMatch
     INTO #TempScoreTable
     FROM #NO_NEWBIESRIDES R_Out
     WHERE pickupTime BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
     AND MainDriver IS NOT NULL


     --SELECT * FROM #TempScoreTable

    SELECT 
    MainDriver, IsSuperDriver,
    COUNT(CASE WHEN PathMatchScore = 0 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_0,
    COUNT(CASE WHEN PathMatchScore = 1 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_1,
    COUNT(CASE WHEN PathMatchScore = 2 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_2,
    COUNT(CASE WHEN PathMatchScore = 3 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_3,
    COUNT(CASE WHEN PathMatchScore = 4 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_4,
    COUNT(CASE WHEN IsDayMatch = 1 THEN 1 END)             AS AmmountOfMatchByDay,
    COUNT(CASE WHEN IsDayMatch = 0 THEN 1 END)             AS AmmountOfDisMatchByDay,
    COUNT(CASE WHEN DayPartMatch = 1 THEN 1 END)           AS AmmountOfMatchDayPart,
    COUNT(CASE WHEN DayPartMatch = 0 THEN 1 END)           AS AmmountOfDisMatchDayPart
    INTO  #CandidatesBucketsTable FROM #TempScoreTable T
    WHERE NOT EXISTS
    (
        select * from UnityRide
        where MainDriver=T.MainDriver and pickupTime 
            BETWEEN 
                IIF(IsSuperDriver = 1, @TimeWindowPastLimit_CheckRides_Super, @TimeWindowPastLimit_CheckRides_Regular)
            AND
                IIF(IsSuperDriver = 1, @TimeWindowFutureLimit_CheckRides_Super, @TimeWindowFutureLimit_CheckRides_Regular)
    )
    GROUP BY MainDriver, IsSuperDriver


    SELECT V.Id, COUNT(*) NumOfRides
    INTO #NEBIEWSWITHZERORIDES
    FROM Volunteer V LEFT JOIN UnityRide R ON V.Id=R.MainDriver
    WHERE DATEDIFF(d,V.JoinDate, getdate()) < @NewDriverTimeWindow
    GROUP BY V.Id
    HAVING  COUNT(*) = 0 

    SELECT
    MainDriver as Id,V.DisplayName, V.NoOfDocumentedCalls, V.NoOfDocumentedRides, IsSuperDriver, AmmountOfMatchDayPart, AmmountOfDisMatchDayPart, AmmountOfPathMatchScoreOfType_0,
    AmmountOfPathMatchScoreOfType_1, AmmountOfPathMatchScoreOfType_2, AmmountOfPathMatchScoreOfType_3,
    AmmountOfPathMatchScoreOfType_4, AmmountOfMatchByDay, AmmountOfDisMatchByDay
    FROM #CandidatesBucketsTable C INNER JOIN Volunteer V ON C.MainDriver = V.Id

        UNION

    SELECT 
    V.Id, V.DisplayName, V.NoOfDocumentedCalls, V.NoOfDocumentedRides, 0 AS IsSuperDriver,
    0 AS AmmountOfMatchDayPart, 0 AS AmmountOfDisMatchDayPart, 0 AS AmmountOfPathMatchScoreOfType_0,
    0 AS AmmountOfPathMatchScoreOfType_1, 0 AS AmmountOfPathMatchScoreOfType_2, 0 AS AmmountOfPathMatchScoreOfType_3,
    0 AS AmmountOfPathMatchScoreOfType_4, 0 AS AmmountOfMatchByDay, 0 AS AmmountOfDisMatchByDay
    FROM #NEBIEWSWITHZERORIDES N INNER JOIN Volunteer V ON N.Id = V.Id

    DROP TABLE #CandidatesBucketsTable, #TempScoreTable, #NUMOFRIDE_PER_VOLUNTEER, #CANDIDATES_NEWBIES_NO_ZERORIDES, #NO_NEWBIESRIDES
END
GO
---------------------------------------------------------------------------------------------------------------------------------

-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <24/04/24>
-- Description: <temp to write new sp for find candidate AFTER unityRide>
-- =============================================
CREATE PROCEDURE spGetCandidatesForUnityRide
(
    @RidePatNum INT,
    @NumOfDaysToThePast INT,                    @NUmOfDaysToTheFuture INT,
    @NumOfDaysToThePast_CheckRides_Regular INT, @NumOfDaysToTheFuture_CheckRides_Regular INT,
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

    --pre process

    SELECT r.* 
    INTO #CleanRide
    FROM UnityRide r 

    --RIDE WITHOUT NEWBIES IN 3 STEPS
    --1
    SELECT MainDriver, COUNT(*) AS NUMOFRIDES
    INTO #NUMOFRIDE_PER_VOLUNTEER
    FROM #CleanRide
    GROUP BY MainDriver

    --2
    SELECT MainDriver AS CandidateId
    INTO #CANDIDATES_NO_NEWBIES
    FROM #NUMOFRIDE_PER_VOLUNTEER NPV INNER JOIN Volunteer V
    ON NPV.MainDriver=V.Id
    WHERE 
    (NUMOFRIDES > @AmountOfRidesInNewDriverTimeWindow OR DATEDIFF(d,v.JoinDate, getdate()) > @NewDriverTimeWindow)
    AND V.IsActive=1 AND V.isDriving=1

    --3 
    SELECT *
    INTO #NO_NEWBIESRIDES
    FROM #CANDIDATES_NO_NEWBIES CNN INNER JOIN #CleanRide R ON CNN.CandidateId=R.MainDriver

    SELECT @Origin = origin FROM UnityRide WHERE RidePatNum = @RidePatNum
    SELECT @Destination = destination FROM UnityRide WHERE RidePatNum = @RidePatNum 
    SELECT @OriginSubArea = RegionId FROM Location WHERE Name =  @origin
    SELECT @DestinationSubArea = RegionId FROM Location WHERE Name =  @Destination
    SELECT @pickupDay = DATENAME(dw, pickupTime) FROM UnityRide WHERE RidePatNum = @RidePatNum
    SELECT @pickupTime= pickupTime FROM UnityRide WHERE RidePatNum = @RidePatNum

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
         WHEN @pickupDay = DATENAME(DW, pickupTime) THEN 1 ELSE 0
     END
     as IsDayMatch
     ,
      CASE
        WHEN exists
            (
                select MainDriver from #CleanRide R_In where R_In.MainDriver=R_Out.MainDriver and
                pickupTime BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
                group by MainDriver having COUNT(*) > @AmountBottomLimitToBeSuperUserDriver
             ) THEN 1 ELSE 0
    END
    AS IsSuperDriver,
    (
     CASE
         WHEN 
         (
            (
            (CHARINDEX(@AfterNoonString,pickupTime) > 0 OR  CAST(pickupTime as time) >= '12:00:00')
             AND
            (CHARINDEX(@AfterNoonString,@pickupTime) > 0 OR  CAST(@pickupTime as time) >= '12:00:00')
            ) -- = afternoon match
            OR
            (
            (CHARINDEX(@AfterNoonString,pickupTime) = 0 AND  CAST(pickupTime as time) < '12:00:00')
             AND
            (CHARINDEX(@AfterNoonString,@pickupTime) = 0 AND  CAST(@pickupTime as time) < '12:00:00')
            )-- = morning match
         )
         THEN 1 ELSE 0
     END

    ) DayPartMatch
     INTO #TempScoreTable
     FROM #NO_NEWBIESRIDES R_Out
     WHERE pickupTime BETWEEN @TimeWindowPastLimit AND @TimeWindowFutureLimit
     AND MainDriver IS NOT NULL


     --SELECT * FROM #TempScoreTable

    SELECT 
    MainDriver, IsSuperDriver,
    COUNT(CASE WHEN PathMatchScore = 0 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_0,
    COUNT(CASE WHEN PathMatchScore = 1 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_1,
    COUNT(CASE WHEN PathMatchScore = 2 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_2,
    COUNT(CASE WHEN PathMatchScore = 3 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_3,
    COUNT(CASE WHEN PathMatchScore = 4 THEN 1 END)         AS AmmountOfPathMatchScoreOfType_4,
    COUNT(CASE WHEN IsDayMatch = 1 THEN 1 END)             AS AmmountOfMatchByDay,
    COUNT(CASE WHEN IsDayMatch = 0 THEN 1 END)             AS AmmountOfDisMatchByDay,
    COUNT(CASE WHEN DayPartMatch = 1 THEN 1 END)          AS AmmountOfMatchDayPart,
    COUNT(CASE WHEN DayPartMatch = 0 THEN 1 END)          AS AmmountOfDisMatchDayPart
    INTO  #CandidatesBucketsTable FROM #TempScoreTable T
    WHERE NOT EXISTS
    (
        select * from #CleanRide
        where MainDriver=T.MainDriver and pickupTime 
            BETWEEN 
                IIF(IsSuperDriver = 1, @TimeWindowPastLimit_CheckRides_Super, @TimeWindowPastLimit_CheckRides_Regular)
            AND
                IIF(IsSuperDriver = 1, @TimeWindowFutureLimit_CheckRides_Super, @TimeWindowFutureLimit_CheckRides_Regular)
    )
    GROUP BY MainDriver, IsSuperDriver

    SELECT
    MainDriver as Id,V.DisplayName, V.NoOfDocumentedCalls, V.NoOfDocumentedRides, IsSuperDriver, AmmountOfMatchDayPart, AmmountOfDisMatchDayPart, AmmountOfPathMatchScoreOfType_0,
    AmmountOfPathMatchScoreOfType_1, AmmountOfPathMatchScoreOfType_2, AmmountOfPathMatchScoreOfType_3,
    AmmountOfPathMatchScoreOfType_4, AmmountOfMatchByDay, AmmountOfDisMatchByDay
    FROM #CandidatesBucketsTable C INNER JOIN Volunteer V ON C.MainDriver = V.Id
        WHERE 
            AmmountOfPathMatchScoreOfType_1 +
            AmmountOfPathMatchScoreOfType_2 +
            AmmountOfPathMatchScoreOfType_3 +
            AmmountOfPathMatchScoreOfType_4        > 0


    DROP TABLE #CandidatesBucketsTable, #TempScoreTable, #NUMOFRIDE_PER_VOLUNTEER, #CANDIDATES_NO_NEWBIES, #NO_NEWBIESRIDES

END

GO
