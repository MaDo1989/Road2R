
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TYPE TableNameList AS TABLE
(
    TableName NVARCHAR(128)
);
GO

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
/****** Object:  StoredProcedure [dbo].[sp_getTablesStracture]    Script Date: 2/5/2026 8:59:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad Meirson>
-- Create Date: <11/30/25>
-- Description: <get the stracture of spesific tables>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getTablesStracture]
(
    -- Add the parameters for the stored procedure here
        @Tables TableNameList READONLY

)
AS
BEGIN

    -- Insert statements for procedure here
    SELECT 
        TABLE_NAME,
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE,
        CHARACTER_MAXIMUM_LENGTH
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME IN (SELECT TableName FROM @Tables) AND
	COLUMN_NAME IN (
	'RidePatNum',
	'PatientName',
	'PatientCellPhone',
	'PatientId',
	'PatientGender',
	'PatientStatus',
	'PatientBirthDate',
	'AmountOfEscorts',
	'AmountOfEquipments',
	'Destination',
	'Origin',
	'pickupTime',
	'Coordinator',
	'Remark',
	'Status',
	'Area',
	'OnlyEscort',
	'lastModified',
	'CoordinatorID',
	'MainDriver',
	'DriverName',
	'DriverCellPhone',
	'NoOfDocumentedRides',
	'IsAnonymous',
	'IsNewDriver',
	'Id',
	'DisplayName',
	'CellPhone',
	'CellPhone2',
	'Address',
	'JoinDate',
	'IsActive',
	'Gender',
	'Remarks',
	'CityCityName',
	'AvailableSeats',
	'VolunteerIdentity',
	'isDriving',
	'joinYear',
	'NoOfDocumentedCalls',
	'NoOfDocumentedRides',
	'LastUpdateBy',
	'No_of_Rides',
	'IsBooster',
	'IsBabyChair'
	)
    ORDER BY TABLE_NAME, ORDINAL_POSITION;
END



------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE Executions(
	ExecId INT IDENTITY(1,1) PRIMARY KEY,
    UserPhone NVARCHAR(20)      NULL,
    UserName NVARCHAR(55)      NULL,
    ExecutionTime DATETIME2     NOT NULL,
    SqlQuery NVARCHAR(MAX)      NULL,
    AiDescription NVARCHAR(MAX) NULL,
    UserPrompt NVARCHAR(MAX)    NULL,
    RelatedTables NVARCHAR(100) NULL
)


------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad Meirson>
-- Create Date: <10/02/2026>
-- Description: <insert values to execution table>
-- =============================================
CREATE PROCEDURE SP_insertValuesToExecutions
(
    -- Add the parameters for the stored procedure here
	@UserPhone NVARCHAR(20) = NULL,
    @UserName NVARCHAR(55) = NULL,
    @ExecutionTime DATETIME2,
    @SqlQuery NVARCHAR(MAX) = NULL,
    @AiDescription NVARCHAR(MAX) = NULL,
    @UserPrompt NVARCHAR(MAX) = NULL,
    @RelatedTables NVARCHAR(100) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    INSERT INTO Executions
    (
        UserPhone,
        UserName,
        ExecutionTime,
        SqlQuery,
        AiDescription,
        UserPrompt,
        RelatedTables
    )
    VALUES
    (
        @UserPhone,
        @UserName,
        @ExecutionTime,
        @SqlQuery,
        @AiDescription,
        @UserPrompt,
        @RelatedTables
    );

    -- מחזיר את ה-ID שנוצר
    SELECT SCOPE_IDENTITY() AS NewExecId;
END
GO




------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_getUnityRidesSplit]    Script Date: 4/19/2026 6:46:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad Meirson>
-- Create Date: <21/10/2025>
-- Modified:    <10/12/2025> - Added EquipmentList to avoid N+1 problem
-- Modified:    <31/01/2026> - Added afternoon detection for rides >= 12:00
-- Description: <try to split the unityRide packs to improve load times>
-- =============================================
ALTER PROCEDURE [dbo].[sp_getUnityRidesSplit]
(
    @ride_date datetime,
    @isAfternoon bit,
    @isFutureTable bit,
    @days smallint
)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF(@isFutureTable = 0)
    BEGIN
        SELECT 
            u.*,
            NULLIF(p.CellPhone, '') AS CellPhone,
            NULLIF(p.CellPhone2, '') AS PatientCellPhone2,
            NULLIF(p.HomePhone, '') AS PatientCellPhone3,
            (
                SELECT STRING_AGG(EquipmentName, ',') WITHIN GROUP (ORDER BY EquipmentName)
                FROM EquipmentForPatientView E
                WHERE E.id = u.PatientId
            ) AS EquipmentList
        FROM UnityRide AS u
        LEFT JOIN Patient AS p ON p.Id = u.PatientId
        WHERE u.Status <> N'נמחקה'
            AND CONVERT(DATE, @ride_date) = CONVERT(DATE, u.PickupTime)
            AND (
                (
                    @isAfternoon = 1 
                    AND (DATEPART(MINUTE, u.PickupTime) = 14 OR DATEPART(HOUR, u.PickupTime) >= 12)
                )
                OR
                (
                    ISNULL(@isAfternoon, 0) = 0 
                    AND DATEPART(MINUTE, u.PickupTime) <> 14 
                    AND DATEPART(HOUR, u.PickupTime) < 12
                )
            )
    END
    ELSE IF(@isFutureTable = 1)
    BEGIN
        SELECT 
            u.*,
            NULLIF(p.CellPhone, '') AS CellPhone,
            NULLIF(p.CellPhone2, '') AS PatientCellPhone2,
            NULLIF(p.HomePhone, '') AS PatientCellPhone3,
            (
                SELECT STRING_AGG(EquipmentName, ',') WITHIN GROUP (ORDER BY EquipmentName)
                FROM EquipmentForPatientView E
                WHERE E.id = u.PatientId
            ) AS EquipmentList
        FROM UnityRide AS u
        LEFT JOIN Patient AS p ON p.Id = u.PatientId
        WHERE 
            CONVERT(DATE, u.PickupTime) BETWEEN
                CONVERT(DATE, DATEADD(DAY, 3, GETDATE())) AND
                CONVERT(DATE, DATEADD(DAY, @days, GETDATE()))
            AND u.Status <> N'נמחקה'
    END
    ELSE
    BEGIN
        SELECT -1 AS RidePatNum, 'Error @isFutureTable is not defined' AS error
    END
END









------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- =============================================================
-- סקריפט להמרת pickupTime מ-UTC לשעון ישראל (GMT+2 / GMT+3)
-- =============================================================
-- שעון קיץ (IDT, UTC+3): מיום שישי שלפני יום ראשון האחרון של מרץ
-- שעון חורף (IST, UTC+2): מיום ראשון האחרון של אוקטובר
-- =============================================================

-- שלב 1: בדיקה בלבד (SELECT) - לראות את התוצאות לפני עדכון
SELECT 
    RidePatNum,
    PatientName,
    pickupTime AS pickupTime_UTC,
    
    -- חישוב תאריך תחילת שעון קיץ: יום שישי שלפני יום ראשון האחרון של מרץ
    -- חישוב תאריך סיום שעון קיץ: יום ראשון האחרון של אוקטובר
    CASE 
        WHEN pickupTime >= 
            -- תחילת שעון קיץ: יום שישי לפני יום ראשון האחרון של מרץ, בשעה 02:00
            DATEADD(DAY, -2, -- יום שישי = יום ראשון פחות 2
                DATEADD(DAY, 
                    -(DATEPART(WEEKDAY, DATEFROMPARTS(YEAR(pickupTime), 3, 31)) - 1 + @@DATEFIRST - 1) % 7,
                    DATEFROMPARTS(YEAR(pickupTime), 3, 31)
                )
            )
        AND pickupTime < 
            -- סיום שעון קיץ: יום ראשון האחרון של אוקטובר, בשעה 02:00
            DATEADD(DAY, 
                -(DATEPART(WEEKDAY, DATEFROMPARTS(YEAR(pickupTime), 10, 31)) - 1 + @@DATEFIRST - 1) % 7,
                DATEFROMPARTS(YEAR(pickupTime), 10, 31)
            )
        THEN DATEADD(HOUR, 3, pickupTime)  -- שעון קיץ: +3
        ELSE DATEADD(HOUR, 2, pickupTime)  -- שעון חורף: +2
    END AS pickupTime_Israel,

    CASE 
        WHEN pickupTime >= 
            DATEADD(DAY, -2,
                DATEADD(DAY, 
                    -(DATEPART(WEEKDAY, DATEFROMPARTS(YEAR(pickupTime), 3, 31)) - 1 + @@DATEFIRST - 1) % 7,
                    DATEFROMPARTS(YEAR(pickupTime), 3, 31)
                )
            )
        AND pickupTime < 
            DATEADD(DAY, 
                -(DATEPART(WEEKDAY, DATEFROMPARTS(YEAR(pickupTime), 10, 31)) - 1 + @@DATEFIRST - 1) % 7,
                DATEFROMPARTS(YEAR(pickupTime), 10, 31)
            )
        THEN N'שעון קיץ (+3)'
        ELSE N'שעון חורף (+2)'
    END AS timezone_type

FROM unityRide
ORDER BY pickupTime;


-- =============================================================
-- שלב 2: UPDATE בפועל - להריץ רק אחרי שבדקת שה-SELECT נראה תקין!
-- =============================================================

--UPDATE unityRide
--SET pickupTime = 
--    CASE 
--        WHEN pickupTime >= 
--            DATEADD(DAY, -2,
--                DATEADD(DAY, 
--                    -(DATEPART(WEEKDAY, DATEFROMPARTS(YEAR(pickupTime), 3, 31)) - 1 + @@DATEFIRST - 1) % 7,
--                    DATEFROMPARTS(YEAR(pickupTime), 3, 31)
--                )
--            )
--        AND pickupTime < 
--            DATEADD(DAY, 
--                -(DATEPART(WEEKDAY, DATEFROMPARTS(YEAR(pickupTime), 10, 31)) - 1 + @@DATEFIRST - 1) % 7,
--                DATEFROMPARTS(YEAR(pickupTime), 10, 31)
--            )
--        THEN DATEADD(HOUR, 3, pickupTime)  -- שעון קיץ: +3
--        ELSE DATEADD(HOUR, 2, pickupTime)  -- שעון חורף: +2
--    END;



------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE unityRide
ADD isAfterNoon BIT NOT NULL
CONSTRAINT DF_unityRide_isAfterNoon DEFAULT 0;



--FOR TEST
SELECT 
    pickupTime,
    DATEPART(HOUR, pickupTime)   AS PickupHour,
    DATEPART(MINUTE, pickupTime) AS PickupMinute,
    CASE 
        WHEN DATEPART(HOUR, pickupTime) > 14
             OR DATEPART(MINUTE, pickupTime) = 14
        THEN 1
        ELSE 0
    END AS ShouldBeIsAfterNoon
FROM unityRide
ORDER BY pickupTime;

--UPDATE


UPDATE unityRide
SET isAfterNoon = 
    CASE 
        WHEN DATEPART(HOUR, pickupTime) >= 12 
             OR DATEPART(MINUTE, pickupTime) = 14
        THEN 1
        ELSE 0
    END;



------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spSetNewUnityRide]    Script Date: 2/19/2026 9:18:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <09/12/23>
-- Description: <this sp is for insert data to unity ride and [PatientEscort_PatientInRide (RidePat)] if need to. >
-- ALTER DATE 19/02/26 - insert new field "isAfternoon" for a good indication about the time of the ride.
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
	@amountOfEscorts int,
	@isAfternoon bit

	
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
IF @coorId is null -- in case the coor id not found in the query above - probablity local storage issues in the client 
begin
SET @coorId = (select id from volunteer where CellPhone like '0512122455')
SET @coorName = (select DisplayName from volunteer where CellPhone like '0512122455')
end
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

	IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @destination =Destination and @pickupTime = pickupTime and MainDriver = @driverId)
		update Volunteer
		SET No_of_Rides = No_of_Rides+1
		Where Id = @driverId


	DECLARE @FlagVar int =-1
	set @FlagVar= (select top 1 RidepatNum
	from unityRide
	where PatientId = @patientId and pickupTime = @pickupTime  and IsAnonymous = 0 and Status not like N'נמחקה' )
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
Status,
isAfterNoon
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
@status,
@isAfternoon
)
Select SCOPE_IDENTITY() 'RidePatNum'
END
else
select -1 'RidePatNum'

END


------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

ALTER PROCEDURE [dbo].[spLogTable_AutoTrackChanges_GetLogs] 
    @startDate datetime = NULL,
    @endDate datetime = NULL,
    @timeRange nvarchar(50) = NULL
AS 
BEGIN 
    SET NOCOUNT ON;

    -- אם נשלח timeRange, נחשב את הטווח אוטומטית
    IF @timeRange IS NOT NULL
    BEGIN
        SET @endDate = GETDATE();
        SET @startDate = 
            CASE @timeRange
                WHEN 'day'   THEN DATEADD(DAY,   -1, CAST(GETDATE() AS date))
                WHEN 'week'  THEN DATEADD(WEEK,  -1, CAST(GETDATE() AS date))
                WHEN 'month' THEN DATEADD(MONTH, -1, CAST(GETDATE() AS date))
                WHEN 'year'  THEN DATEADD(YEAR,  -1, CAST(GETDATE() AS date))
            END;
    END

    SELECT * 
    FROM LogTable_AutoTrackChanges  
    WHERE (@startDate IS NULL OR DateAdded >= @startDate)
      AND (@endDate   IS NULL OR DateAdded <= @endDate);
END
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER FUNCTION dbo.GetIsraelTime()
RETURNS datetime
AS
BEGIN
    RETURN CONVERT(datetime,
        CAST(GETUTCDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'Israel Standard Time' AS datetime)
    )
END

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spUpdateDriverUnityRide]    Script Date: 5/14/2026 10:00:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <25/12/23>
-- ALTER Date : 11/06/24 -- to fix the duplicate Driver bug 

-- Description: <update a driver to spesific unity ride also to delete driver from ride>
-- =============================================
ALTER PROCEDURE [dbo].[spUpdateDriverUnityRide]
(
    -- Add the parameters for the stored procedure here
	@driverID int,
	@unityRideID int,
	@CoorName nvarchar(255)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
	DECLARE @driverName nvarchar(255) = (select displayname from volunteer where Id=@driverID)
	Declare @This_origin Nvarchar(55) = (select Origin from UnityRide where RidePatNum=@UnityRideID)
	DECLARE @pickupTime DateTime = (select pickupTime from UnityRide where RidePatNum = @unityRideID)
	DECLARE @driverPhone varchar(11) = (select cellphone from volunteer where id= @driverID)
	DECLARE @isNewDriver bit = case when (select count(*) from UnityRide where pickupTime<=GETDATE() and MainDriver=@driverID)<=3 then 1 else 0 end
	DECLARE @oldDriver int = (select MainDriver from UnityRide where RidePatNum = @unityRideID)
	DECLARE @coorId int = (select id from volunteer where displayName like @CoorName)
	DECLARE @origin Nvarchar(55) = (select origin from UnityRide where RidePatNum = @unityRideID)
	DECLARE @dest Nvarchar(55) = (select Destination from UnityRide where RidePatNum = @unityRideID)

-- care of the NoOfDocumentedRides in volunteer table.
	--switch drivers
	IF(@oldDriver is not null AND @driverID!=-1)
		begin
			update Volunteer
			SET NoOfDocumentedRides = NoOfDocumentedRides-1
			where Id = @oldDriver

			IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @dest =Destination and @pickupTime = pickupTime and MainDriver = @oldDriver and RidePatNum!=@unityRideID)
			update Volunteer
			SET No_of_Rides = No_of_Rides-1
			Where Id = @oldDriver


			update Volunteer
			SET NoOfDocumentedRides = NoOfDocumentedRides+1
			where Id = @driverID

			IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @dest =Destination and @pickupTime = pickupTime and MainDriver = @driverID)
			update Volunteer
			SET No_of_Rides = No_of_Rides+1
			Where Id = @driverID


		end
	--add driver
	IF(@oldDriver is null AND @driverID!=-1)
		begin
			update Volunteer
			SET NoOfDocumentedRides = NoOfDocumentedRides+1
			where Id = @driverID

			IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @dest =Destination and @pickupTime = pickupTime and MainDriver = @driverID)
			update Volunteer
			SET No_of_Rides = No_of_Rides+1
			Where Id = @driverID

		end
	-- remove driver
	IF(@oldDriver is not null AND @driverID = -1)
		begin
			update Volunteer
			SET NoOfDocumentedRides = NoOfDocumentedRides-1
			where Id = @oldDriver

			IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @dest =Destination and @pickupTime = pickupTime and MainDriver = @oldDriver and RidePatNum!=@unityRideID)
			update Volunteer
			SET No_of_Rides = No_of_Rides-1
			Where Id = @oldDriver
		end



DECLARE @NoOfDocumentedRides int = (select NoOfDocumentedRides from volunteer where id= @driverID)

	-- for assign driver to unity ride
	IF @driverID!=-1
	BEGIN
	-- check if the driver got any ride in this time
	IF EXISTS(select 1
	from UnityRide
	where RidePatNum != @UnityRideID and
		  pickupTime = @pickupTime and 
		  MainDriver = @driverId and
		  status!=N'נמחקה' and 
		  Origin!=@This_origin
		  )
	begin
	select -5 as 'RidePatNum'
	Return
	end

	ELSE
	begin
	UPDATE UnityRide
	set MainDriver = @driverID,
		DriverName = @driverName,
		DriverCellPhone = @driverPhone,
		NoOfDocumentedRides = @NoOfDocumentedRides,
		IsNewDriver = @isNewDriver,
		lastModified = dbo.GetIsraelTime(),
		Coordinator = @CoorName,
		CoordinatorID = @coorId,
		Status = N'שובץ נהג'
	where RidePatNum = @unityRideID
	end
	END
	
	-- for delete driver from unity ride
	ELSE 
		UPDATE UnityRide
	set MainDriver = NULL,
		DriverName = NULL,
		DriverCellPhone = NULL,
		NoOfDocumentedRides = NULL,
		IsNewDriver = 1,
		lastModified = dbo.GetIsraelTime(),
		Coordinator = @CoorName,
		CoordinatorID = @coorId,
		Status = N'ממתינה לשיבוץ'
	where RidePatNum = @unityRideID

SELECT 
	u.*,
	NULLIF(p.CellPhone, '') AS CellPhone,
	NULLIF(p.CellPhone2, '') AS PatientCellPhone2,
	NULLIF(p.HomePhone, '') AS PatientCellPhone3
FROM UnityRide AS u
LEFT JOIN Patient AS p ON p.Id = u.PatientId
where RidePatNum = @unityRideID
END


------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spUnityRide_UpdateDateAndTime]    Script Date: 5/14/2026 10:09:44 AM ******/
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
	@unityRideId INT,
	@CoorName NVARCHAR(255)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	DECLARE @PatientId INT;
    DECLARE @Origin NVARCHAR(255);
    DECLARE @Destination NVARCHAR(255);
	DECLARE @coorId int = (select id from volunteer where displayName like @CoorName)


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
			IsAnonymous = 0 AND
			status not like N'נמחקה' AND
            RidePatNum != @unityRideId  -- Exclude the current ride
    )
    BEGIN
        SELECT -2 AS 'RidePatNum', 'Duplicate ride exists with the same values' AS 'Message';
        RETURN;
    END


BEGIN TRAN UpdateUnityRideTime

UPDATE UnityRide
SET PickupTime = @editedTime, lastModified = dbo.GetIsraelTime(), Coordinator = @CoorName, CoordinatorID = @coorId
where RidePatNum=@unityRideId

DECLARE @rowCount INT = 0 ;
SET @rowCount = @@ROWCOUNT;



IF @rowCount>0
SELECT 
    u.*,
    NULLIF(p.CellPhone, '') AS CellPhone,
    NULLIF(p.CellPhone2, '') AS PatientCellPhone2,
    NULLIF(p.HomePhone, '') AS PatientCellPhone3
FROM UnityRide AS u
LEFT JOIN Patient AS p ON p.Id = u.PatientId
where RidePatNum=@unityRideId
ELSE
select -1 as 'RidePatNum'

COMMIT TRAN UpdateUnityRideTime
END

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spUpdatePatientStatusUnityRide]    Script Date: 5/14/2026 10:12:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <24/12/23>
-- Description: <for update the status of patiet in a ride update in 2 tables because of the unity>
-- =============================================
ALTER PROCEDURE [dbo].[spUpdatePatientStatusUnityRide]
(
    -- Add the parameters for the stored procedure here
	@PatientId INT,
	@RidePatNum INT,
	@PatientStatus nvarchar(55),
	@EditTimeStamp datetime,
	@CoorName nvarchar(255)
)
AS
BEGIN

		DECLARE @coorId int = (select id from volunteer where displayName like @CoorName)

		UPDATE UnityRide
		SET LastModified=dbo.GetIsraelTime(),PatientStatus=@PatientStatus,patientStatusTime=@EditTimeStamp,Coordinator = @CoorName,CoordinatorID=@coorId
		WHERE RidePatNum=@RidePatNum

		SELECT 
			u.*,
			NULLIF(p.CellPhone, '') AS CellPhone,
			NULLIF(p.CellPhone2, '') AS PatientCellPhone2,
			NULLIF(p.HomePhone, '') AS PatientCellPhone3
		FROM UnityRide AS u
		LEFT JOIN Patient AS p ON p.Id = u.PatientId
		where RidePatNum=@RidePatNum
END




------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spUnityRide_updateRemark]    Script Date: 5/14/2026 10:14:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <24/12/23>
-- Description: <according to unity rides need to update remark in the same way like ridepat>
-- =============================================
ALTER PROCEDURE [dbo].[spUnityRide_updateRemark]
(
    -- Add the parameters for the stored procedure here
	@ridePatNum int,
	@newRemark nvarchar(255),
	@CoorName nvarchar(255)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
		DECLARE @coorId int = (select id from volunteer where displayName like @CoorName)

    -- Insert statements for procedure here
	UPDATE UnityRide
	SET Remark=@newRemark,
	lastModified = dbo.GetIsraelTime(),
	Coordinator = @CoorName,
	CoordinatorID = @coorId
	WHERE RidePatNum=@ridePatNum

	if @@ROWCOUNT>0
	select u.*, 
	NULLIF(p.CellPhone, '') AS CellPhone,
    NULLIF(p.CellPhone2, '') AS PatientCellPhone2,
    NULLIF(p.HomePhone, '') AS PatientCellPhone3
	FROM UnityRide AS u
	LEFT JOIN Patient AS p ON p.Id = u.PatientId
	where RidePatNum=@ridePatNum
	else
	select -1 as 'RidePatNum'
	
END


------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spSetNewUnityRide]    Script Date: 5/14/2026 10:17:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <09/12/23>
-- Description: <this sp is for insert data to unity ride and [PatientEscort_PatientInRide (RidePat)] if need to. >
-- ALTER DATE 19/02/26 - insert new field "isAfternoon" for a good indication about the time of the ride.
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
	@amountOfEscorts int,
	@isAfternoon bit

	
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    

    -- Insert statements for procedure here
	DECLARE @now datetime = CAST(
    GETUTCDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'Israel Standard Time' 
    AS datetime
)
DECLARE @patientCellPhone varchar(50) = (select CellPhone from patient where Id = @patientId)
DECLARE @patientGender nvarchar(50) = (select Gender from patient where Id = @patientId)
DECLARE @patientBirthDate date = (select BirthDate from patient where Id = @patientId)
DECLARE @coorId int = (select id from volunteer where displayName like @coorName)
IF @coorId is null -- in case the coor id not found in the query above - probablity local storage issues in the client 
begin
SET @coorId = (select id from volunteer where CellPhone like '0512122455')
SET @coorName = (select DisplayName from volunteer where CellPhone like '0512122455')
end
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

	IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @destination =Destination and @pickupTime = pickupTime and MainDriver = @driverId)
		update Volunteer
		SET No_of_Rides = No_of_Rides+1
		Where Id = @driverId


	DECLARE @FlagVar int =-1
	set @FlagVar= (select top 1 RidepatNum
	from unityRide
	where PatientId = @patientId and pickupTime = @pickupTime  and IsAnonymous = 0 and Status not like N'נמחקה' )
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
Status,
isAfterNoon
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
@now,
@coorId,
@driverId,
@driverName,
@driverPhone,
@NoOfDocumentedRides,
@isAnonymous,
@isNewDriver,
@status,
@isAfternoon
)
Select SCOPE_IDENTITY() 'RidePatNum'
END
else
select -1 'RidePatNum'

END


------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spUpdateRideInUnityRide]    Script Date: 5/14/2026 10:21:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <11/12/23>
-- Description: <this sp is for update spesific ride in the table (edit in the client)>
-- =============================================
ALTER PROCEDURE [dbo].[spUpdateRideInUnityRide]
(
    -- Add the parameters for the stored procedure here
	@unityRideId int ,
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
	DECLARE @isNewDriver bit = case when (select count(*) from UnityRide where pickupTime<=GETDATE() and MainDriver=@driverId)<=3 then 1 else 0 end
	DECLARE @oldDriver int = (select mainDriver from UnityRide where RidePatNum = @unityRideId)

	DECLARE @AmountOfEquipments int = 0
	SET @AmountOfEquipments  = case when(select count(PatientId) from Equipment_Patient where PatientId = @patientId group by PatientId) is null then 0 ELSE 
	(select count(PatientId) from Equipment_Patient where PatientId = @patientId group by PatientId)end
	set @driverName = case when @driverName='' then null else @driverName end


	DECLARE @FlagVar int =-1
	set @FlagVar= (select top 1 RidepatNum
	from unityRide
	where PatientId = @patientId and pickupTime = @pickupTime and Status !=N'נמחקה' and  IsAnonymous = 0 and RidePatNum!=@unityRideId )

	--DECLARE @isDuplicated int = -1
	--set @isDuplicated =(select top 1 RidePatNum
	--from UnityRide
	--where PatientId = @patientId and pickupTime = @pickupTime and RidePatNum!=@unityRideId
	
	--handle NoOfDocumentedRides in volunteer table
	UPDATE Volunteer
	SET NoOfDocumentedRides = NoOfDocumentedRides+1
	where Id = @driverId

	IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @destination =Destination and @pickupTime = pickupTime and MainDriver = @driverId and RidePatNum!=@unityRideID)
	update Volunteer
	SET No_of_Rides = No_of_Rides+1
	Where Id = @driverId

	UPDATE Volunteer
	SET NoOfDocumentedRides = NoOfDocumentedRides-1
	where Id = @oldDriver

	IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @destination =Destination and @pickupTime = pickupTime and MainDriver = @oldDriver and RidePatNum!=@unityRideID)
	update Volunteer
	SET No_of_Rides = No_of_Rides-1
	Where Id = @oldDriver

	DECLARE @NoOfDocumentedRides int = (select NoOfDocumentedRides from volunteer where id= @driverId)

	if (@FlagVar is null)
	UPDATE UnityRide
	SET PatientName = @patientName,
	PatientId = @patientId,
	Origin = @origin,
	Destination = @destination,
	pickupTime = @pickupTime,
	Remark = @remark,
	OnlyEscort = @onlyEscort,
	Area = @area,
	IsAnonymous = @isAnonymous,
	Coordinator = @coorName,
	DriverName = @driverName,
	AmountOfEscorts = @amountOfEscorts,
	PatientCellPhone=@patientCellPhone,
	PatientGender=@patientGender,
	PatientBirthDate =@patientBirthDate,
	CoordinatorID=@coorId,
	MainDriver=@driverId,
	DriverCellPhone=@driverPhone,
	NoOfDocumentedRides = @NoOfDocumentedRides,
	lastModified = dbo.GetIsraelTime(),
	IsNewDriver = @isNewDriver,
	AmountOfEquipments = @AmountOfEquipments,
	status = case when @driverId is null then N'ממתינה לשיבוץ' else N'שובץ נהג' end
	WHERE RidePatNum=@unityRideId;
	
	ELSE 
	return -1

END


------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spDeleteUnityRide]    Script Date: 5/14/2026 10:24:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <28/12/23>
-- Description: <this sp is for delete spesific ride or update the status
--				if this is anonymous ride and there is no driver -> delete 
--				anything else only change status to -> נמחקה 
--				then need to return the return-Ride to ask the client if delete it too.

-->
-- =============================================
ALTER PROCEDURE [dbo].[spDeleteUnityRide]
(
    -- Add the parameters for the stored procedure here
   @unityRideID INT,
   @CoorName NVARCHAR(255)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
		DECLARE @DriverId INT = (select MainDriver from unityRide where ridepatnum = @unityRideID)
		DECLARE @isAnonymous bit = (select IsAnonymous from UnityRide where ridepatnum = @unityRideID)
		DECLARE @origin Nvarchar(55) = (select origin from UnityRide where RidePatNum = @unityRideID)
		DECLARE @dest Nvarchar(55) = (select Destination from UnityRide where RidePatNum = @unityRideID)
		DECLARE @pickupTime dateTime = (select pickupTime from UnityRide where  RidePatNum = @unityRideID)

		DECLARE @CoorId INT = (select id from Volunteer where DisplayName like @CoorName )


		

		
		IF(@DriverId IS NULL and @isAnonymous = 1)
		BEGIN
		IF OBJECT_ID('tempdb..#DeletedUnityRide') IS NOT NULL
            DROP TABLE #DeletedUnityRide;

         -- טבלת יעד זמנית ללא IDENTITY
    CREATE TABLE #DeletedUnityRide
    (
        RidePatNum            INT,
        PatientName           NVARCHAR(255),
        PatientCellPhone      NVARCHAR(50),
        PatientId             INT,
        PatientGender         NVARCHAR(50),
        PatientStatus         NVARCHAR(50),
        patientStatusTime     DATETIME,
        PatientBirthDate      DATETIME,
        AmountOfEscorts       INT,
        AmountOfEquipments    INT,
        Origin                NVARCHAR(55),
        Destination           NVARCHAR(55),
        pickupTime            DATETIME,
        Coordinator           NVARCHAR(255),
        Remark                NVARCHAR(MAX),
        Status                NVARCHAR(50),
        Area                  NVARCHAR(50),
        Shift                 NVARCHAR(50),
        OnlyEscort            BIT,
        lastModified          DATETIME,
        CoordinatorID         INT,
        MainDriver            INT,
        DriverName            NVARCHAR(255),
        DriverCellPhone       NVARCHAR(50),
        NoOfDocumentedRides   INT,
        IsAnonymous           BIT,
        IsNewDriver           BIT
    );

    -- מוחקים מהטבלה האמיתית ומזרימים את הנתונים שנמחקו לטבלה הזמנית
    UPDATE UnityRide
		set Status = N'נמחקה', lastModified = dbo.GetIsraelTime(),Coordinator = @CoorName , CoordinatorID = @CoorId
		where ridepatnum = @unityRideID
	DELETE FROM UnityRide
    OUTPUT
        deleted.RidePatNum,
        deleted.PatientName,
        deleted.PatientCellPhone,
        deleted.PatientId,
        deleted.PatientGender,
        deleted.PatientStatus,
        deleted.patientStatusTime,
        deleted.PatientBirthDate,
        deleted.AmountOfEscorts,
        deleted.AmountOfEquipments,
        deleted.Origin,
        deleted.Destination,
        deleted.pickupTime,
        deleted.Coordinator,
        deleted.Remark,
        deleted.Status,
        deleted.Area,
        deleted.Shift,
        deleted.OnlyEscort,
        deleted.lastModified,
        deleted.CoordinatorID,
        deleted.MainDriver,
        deleted.DriverName,
        deleted.DriverCellPhone,
        deleted.NoOfDocumentedRides,
        deleted.IsAnonymous,
        deleted.IsNewDriver
    INTO #DeletedUnityRide
    WHERE RidePatNum = @unityRideID;

	
    -- מחזירים את כל הנתונים + טלפונים נוספים מה-Patient
    SELECT
        d.*,
        NULLIF(p.CellPhone2, '') AS PatientCellPhone2,
        NULLIF(p.HomePhone,  '') AS PatientCellPhone3
    FROM #DeletedUnityRide AS d
    LEFT JOIN Patient AS p ON p.Id = d.PatientId;


		END
		
		ELSE
		BEGIN
		UPDATE UnityRide
		set Status = N'נמחקה', lastModified = dbo.GetIsraelTime(),Coordinator = @CoorName , CoordinatorID = @CoorId
		where ridepatnum = @unityRideID
		--return the update ride
		Select u.*,
		NULLIF(p.CellPhone, '') AS CellPhone,
		NULLIF(p.CellPhone2, '') AS PatientCellPhone2,
		NULLIF(p.HomePhone, '') AS PatientCellPhone3
		FROM UnityRide AS u
		LEFT JOIN Patient AS p ON p.Id = u.PatientId
		where  ridepatnum = @unityRideID

		IF(@DriverId IS not NULL)
		update Volunteer
		SET NoOfDocumentedRides = NoOfDocumentedRides-1
		where Id = @DriverId

		IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @dest =Destination and @pickupTime = pickupTime and MainDriver = @DriverId and status != N'נמחקה')
		update Volunteer
		SET No_of_Rides = No_of_Rides-1
		Where Id = @DriverId
		END



END



------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------