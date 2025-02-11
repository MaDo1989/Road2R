
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------




/****** Object:  StoredProcedure [dbo].[spVolunteer_ToggleActiveness]    Script Date: 05/07/2024 10:24:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yogev Strauber
-- Create Date : 03/12/2022 @night
-- ALTER  Date : 27/01/2023 
-- Description: Active Or Deactivate Volunteer Base on isActive Parameter
-- Returns IsSuccesfullOperation bit, and optional VolunteerWithFutureRidesIncludedToday
-- when try to deactivate volunteer
-- =============================================
ALTER     PROCEDURE [dbo].[spVolunteer_ToggleActiveness]
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
			SELECT COUNT(*) FROM UnityRide 
			WHERE MAINDRIVER=@volunteerId
			AND GETDATE() < pickupTime
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



---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[spVolunteer_ToggleIsDrive]    Script Date: 05/07/2024 10:40:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 14/12/2022 @night
-- Description: Active Or Deactivate Volunteer's isDriving
-- Returns IsSuccesfullOperation bit, and optional VolunteerWithFutureRidesIncludedToday
-- when try to deactivate volunteer
-- =============================================
ALTER    PROCEDURE [dbo].[spVolunteer_ToggleIsDrive]
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
			SELECT COUNT(*) FROM UnityRide 
			WHERE MAINDRIVER=@volunteerId
			AND  CONVERT(VARCHAR, GETDATE(), 110) <= CONVERT(VARCHAR, pickupTime, 110) 
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




---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------



/****** Object:  StoredProcedure [dbo].[spVolunteer_GetDrivers]    Script Date: 05/07/2024 10:52:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yogev Strauber
-- Create Date  : 
-- Description  : Get drivers who isDriving & isActive
-- Alter Date   : 26/08/2022
-- Alter Reason : Add IsNewDriver
-- =============================================
ALTER   PROCEDURE [dbo].[spVolunteer_GetDrivers]
@isActive BIT,  
@isDriving BIT 
AS 
BEGIN
SET NOCOUNT ON;

SELECT
Id, DisplayName, CellPhone, EnglishFN, EnglishLN, NoOfDocumentedRides,
	
	CASE 
		WHEN (SELECT COUNT(*) FROM UnityRide WHERE MainDriver=Id AND pickupTime <= GETDATE()) <= 3 THEN 1
		ELSE 0
	END
	AS IsNewDriver

FROM VOLUNTEER
WHERE isDriving=@isDriving    and    isActive=@isActive 
END



---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------





/****** Object:  StoredProcedure [dbo].[spGetEquipmentPerPatient]    Script Date: 05/07/2024 11:50:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Benny>
-- Create Date: <20-4-2022>
-- Description: <Gets the equipment needed for future rides>
-- =============================================
ALTER PROCEDURE [dbo].[spGetEquipmentPerPatient](
@daysAhead int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
select p.DisplayName as patient, e.Name
from equipment_patient ep
join patient p on ep.PatientId = p.Id
join Equipment e on e.Id = ep.EquipmentId
where p.DisplayName in 
(
select rp.PatientName from UnityRide rp where rp.pickuptime > GETDATE() and status = N'ממתינה לשיבוץ' and rp.pickuptime < DATEADD(day, @daysAhead, GETDATE())  
)
END


---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------


---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

--Update the NoOfDocumentedRides in Volunteer Table !!!! 
--becarful with this query need to be tested before use it




select id, displayName, NoOfDocumentedRides,u.counter
into #temp1
from volunteer inner join (
select mainDriver , DriverName , count(mainDriver) as 'counter'
from unityRide
where status !=N'נמחקה'
group by mainDriver , DriverName 
) as u on u.mainDriver = id

select * , NoOfDocumentedRides-counter as 'diff'
from #temp1
order by diff desc



-------------------
drop table #temp1
--------------------



UPDATE 
    volunteer
SET 
    NoOfDocumentedRides = rc.counter
FROM 
    volunteer v
INNER JOIN 
    #temp1 rc ON v.id = rc.id;

	
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------





/****** Object:  StoredProcedure [dbo].[spVolunteer_ToggleIsDrive]    Script Date: 14/07/2024 11:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 14/12/2022 @night
-- Description: Active Or Deactivate Volunteer's isDriving
-- Returns IsSuccesfullOperation bit, and optional VolunteerWithFutureRidesIncludedToday
-- when try to deactivate volunteer
-- =============================================
ALTER    PROCEDURE [dbo].[spVolunteer_ToggleIsDrive]
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
			SELECT COUNT(*) FROM UnityRide 
			WHERE MAINDRIVER=@volunteerId
			AND  GETDATE() <= pickupTime
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




	
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------




---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------





/****** Object:  StoredProcedure [dbo].[spUpdateRideInUnityRide]    Script Date: 21/07/2024 13:45:07 ******/
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

	UPDATE Volunteer
	SET NoOfDocumentedRides = NoOfDocumentedRides-1
	where Id = @oldDriver

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
	lastModified = GETDATE(),
	IsNewDriver = @isNewDriver,
	AmountOfEquipments = @AmountOfEquipments,
	status = case when @driverId is null then N'ממתינה לשיבוץ' else N'שובץ נהג' end
	WHERE RidePatNum=@unityRideId;
	
	ELSE 
	return -1

END



---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------



/****** Object:  StoredProcedure [dbo].[spUnityRide_updateRemark]    Script Date: 21/07/2024 14:13:08 ******/
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
	lastModified = GETDATE(),
	Coordinator = @CoorName,
	CoordinatorID = @coorId
	WHERE RidePatNum=@ridePatNum

	if @@ROWCOUNT>0
	select * from UnityRide where RidePatNum=@ridePatNum
	else
	select -1 as 'RidePatNum'
	
END









---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------



/****** Object:  StoredProcedure [dbo].[spUpdatePatientStatusUnityRide]    Script Date: 21/07/2024 14:27:41 ******/
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
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	
	--	IF NOT EXISTS (SELECT 1 FROM RidePatPatientStatus WHERE RidePatNum=@RidePatNum)
	--	BEGIN
	--		INSERT INTO RidePatPatientStatus (PatientId, RidePatNum, PatientStatus, EditTimeStamp)
	--		VALUES (@PatientId, @RidePatNum, @PatientStatus, @EditTimeStamp);
	--	END
	--ELSE
	--	BEGIN
	--		UPDATE RidePatPatientStatus
	--		SET PatientStatus = @PatientStatus, EditTimeStamp=@EditTimeStamp
	--		WHERE RidePatNum=@RidePatNum
	--	END
		DECLARE @coorId int = (select id from volunteer where displayName like @CoorName)

		UPDATE UnityRide
		SET LastModified=GETDATE(),PatientStatus=@PatientStatus,patientStatusTime=@EditTimeStamp,Coordinator = @CoorName,CoordinatorID=@coorId
		WHERE RidePatNum=@RidePatNum

		select * 
		from UnityRide
		where RidePatNum=@RidePatNum
END





---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[spUpdateDriverUnityRide]    Script Date: 21/07/2024 14:35:59 ******/
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

-- care of the NoOfDocumentedRides in volunteer table.
	--switch drivers
	IF(@oldDriver is not null AND @driverID!=-1)
		begin
			update Volunteer
			SET NoOfDocumentedRides = NoOfDocumentedRides-1
			where Id = @oldDriver

			update Volunteer
			SET NoOfDocumentedRides = NoOfDocumentedRides+1
			where Id = @driverID
		end
	--add driver
	IF(@oldDriver is null AND @driverID!=-1)
		begin
			update Volunteer
			SET NoOfDocumentedRides = NoOfDocumentedRides+1
			where Id = @driverID
		end
	-- remove driver
	IF(@oldDriver is not null AND @driverID = -1)
		begin
			update Volunteer
			SET NoOfDocumentedRides = NoOfDocumentedRides-1
			where Id = @oldDriver
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
		lastModified = GETDATE(),
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
		lastModified = GETDATE(),
		Coordinator = @CoorName,
		CoordinatorID = @coorId,
		Status = N'ממתינה לשיבוץ'
	where RidePatNum = @unityRideID

select * from UnityRide where RidePatNum = @unityRideID
END





---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spUnityRide_UpdateDateAndTime]    Script Date: 15/09/2024 17:53:11 ******/
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
SET PickupTime = @editedTime, lastModified = GETDATE(), Coordinator = @CoorName, CoordinatorID = @coorId
where RidePatNum=@unityRideId

DECLARE @rowCount INT = 0 ;
SET @rowCount = @@ROWCOUNT;



IF @rowCount>0
select * from UnityRide where RidePatNum=@unityRideId
ELSE
select -1 as 'RidePatNum'

COMMIT TRAN UpdateUnityRideTime
END



---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spRecoverUnityRide]    Script Date: 23/07/2024 11:53:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <02/01/24>
-- Description: <To recover a ride with status deleted>
-- =============================================
ALTER PROCEDURE [dbo].[spRecoverUnityRide]
(
    -- Add the parameters for the stored procedure here
		@unityRideID INT,
		@CoorName NVARCHAR(255)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

		DECLARE @CoorId INT = (select id from Volunteer where DisplayName like @CoorName )
    -- Insert statements for procedure here
		DECLARE @driverID INT = (select MainDriver from UnityRide where RidePatNum =  @unityRideID)

		IF(@driverID is null)
		Update UnityRide
		SET Status  = N'ממתינה לשיבוץ',
		lastModified = GETDATE(),
		Coordinator = @CoorName,
		CoordinatorID = @CoorId
		where RidePatNum = @unityRideID

		ELSE
		begin
		Update UnityRide
		SET Status  = N'שובץ נהג',
		lastModified = GETDATE(),
		Coordinator = @CoorName,
		CoordinatorID = @CoorId
		where RidePatNum = @unityRideID
		Update Volunteer
		SET NoOfDocumentedRides = NoOfDocumentedRides + 1
		Where Id = @driverID
		end


END




---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spDeleteUnityRide]    Script Date: 23/07/2024 12:26:14 ******/
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
		DECLARE @CoorId INT = (select id from Volunteer where DisplayName like @CoorName )


		IF(@DriverId IS not NULL)
		update Volunteer
		SET NoOfDocumentedRides = NoOfDocumentedRides-1
		where Id = @DriverId

		
		IF(@DriverId IS NULL and @isAnonymous = 1)

		BEGIN
		DELETE FROM UnityRide
		WHERE ridepatnum = @unityRideID;
		select @unityRideID*-1 as 'RidePatNum'
		END
		
		ELSE

		BEGIN
		UPDATE UnityRide
		set Status = N'נמחקה', lastModified = GETDATE(),Coordinator = @CoorName , CoordinatorID = @CoorId
		where ridepatnum = @unityRideID
		--return the update ride
		Select * from UnityRide where  ridepatnum = @unityRideID
		END



END




---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spGetUnitedRides]    Script Date: 28/07/2024 14:39:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad >
-- Create Date: <03/10/2023>
-- Description: <Because of the United Tables try to Re-orgenaize this query>
-- =============================================
ALTER PROCEDURE [dbo].[spGetUnitedRides]
(
    -- Add the parameters for the stored procedure here
	@days int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
select *,
case when (select cellphone2 from Patient where id = PatientId) !='' then (select cellphone2 from Patient where id = PatientId)
when (select HomePhone from Patient where id = PatientId) !='' then (select HomePhone from Patient where id = PatientId)
else '0' END as 'PatientCellPhone2'
from UnityRide where DATEDIFF(day,getdate(),pickuptime)<=@days and Convert(date,pickuptime)>=CONVERT(date, getdate()) and Status <> N'נמחקה'; 
END





---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[sp_GetPatientList]    Script Date: 28/07/2024 14:53:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad Meirson>
-- Create Date: <18/06/2024>
-- Description: <this sp is for efficiency and time tests,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_GetPatientList]
(
    -- Add the parameters for the stored procedure here
    @active bit
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
   

    -- Insert statements for procedure here
	select Id,DisplayName,Cellphone,
	case when CellPhone2 !='' then CellPhone2 
	when HomePhone !='' then HomePhone 
	else '0' end as'CellPhone2',
	BirthDate,Gender,Hospital,Barrier,PatientIdentity,LastModified,EnglishName,isActive
	from Patient where IsActive = @active  order by Id
END


---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

-- Update English names for hospitals with Unicode support
UPDATE location
SET EnglishName = 
    CASE Name
        WHEN N'אוגוסטה ויקטוריה' THEN 'Augusta Victoria'
        WHEN N'אל מקאסד' THEN 'Al-Makassed'
        WHEN N'אל נג''אח' THEN 'An-Najah'
        WHEN N'באב אל עמוד' THEN 'Bab Al-Amoud'
        WHEN N'מר יוסיף' THEN 'Mar Youssef'
        WHEN N'נק'' אמצע' THEN 'Midpoint'
        WHEN N'סנג''ון' THEN 'Sanjon'
        ELSE EnglishName  -- Keep existing value if no match
    END
WHERE EnglishName IS NULL OR EnglishName = '';

---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Gilad Meirson
-- Create Date: 04/08/2024
-- Description: needed to get the english name for the patient and locations.
-- =============================================
CREATE PROCEDURE spGetEnglishNamePatientAndLocation
(
    -- Add the parameters for the stored procedure here
  @RideID int 
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    DECLARE  @patientEnglishName varchar(255);
	SET @patientEnglishName = (select EnglishName from patient where id = (select PatientId from UnityRide where RidePatNum = @RideID))

	DECLARE @OriginEnglishName varchar(255);
	SET @OriginEnglishName = (select EnglishName from Location where Name like (select Origin from UnityRide where RidePatNum = @RideID))

	DECLARE @DestEnglishName varchar(255);
	SET @DestEnglishName = (select EnglishName from Location where Name like (select Destination from UnityRide where RidePatNum = @RideID))
	

    -- Insert statements for procedure here
	select @patientEnglishName as 'patientEnglishName' ,@OriginEnglishName as 'OriginEnglishName' ,@DestEnglishName as 'DestEnglishName'
 
END
GO

---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Gilad
-- Create Date: 14/08/2024
-- Description: this is for Avital get the data to csv report.
-- =============================================
CREATE PROCEDURE sp_monthlyReportRides_patients
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
   
DECLARE @PreviousMonthStart DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0);
DECLARE @CurrentMonthStart DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0);
print(@PreviousMonthStart)
print(@CurrentMonthStart)
SELECT 
    CONVERT(VARCHAR, PickupTime, 103) AS PickupTime, 
    Origin, 
    Destination, 
    Volunteer.DisplayName, 
    PatName
FROM 
(
    SELECT 
        pickuptime, 
        Origin, 
        Destination, 
        MainDriver, 
        patientName AS PatName 
    FROM UnityRide 
    WHERE 
        pickuptime >= '2022-01-01'
        AND MainDriver IS NOT NULL
        AND pickuptime >= @PreviousMonthStart
        AND pickuptime < @CurrentMonthStart
) AS BUFF
INNER JOIN Volunteer ON MainDriver = Volunteer.Id
ORDER BY Volunteer.DisplayName ASC;
END
GO







---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[sp_getALLCandidateUnityRideV2]    Script Date: 07/10/2024 13:59:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad Meirson>
-- Create Date: <17/09/2024>
-- Description: <get all type of volunteer to unit Table for candidate moudle>
-- =============================================
ALTER PROCEDURE [dbo].[sp_getALLCandidateUnityRideV2]
(
    -- Add the parameters for the stored procedure here
    @ridePatNum INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    

    -- Insert statements for procedure here
    --declare basic vars 
DECLARE @R_origin NVARCHAR(255) = (SELECT origin FROM unityRide WHERE ridePatNum = @ridePatNum);
DECLARE @R_dest NVARCHAR(255) = (SELECT destination FROM unityRide WHERE ridePatNum = @ridePatNum);
DECLARE @R_time DateTIme = (SELECT pickupTime FROM unityRide WHERE ridePatNum = @ridePatNum);
DECLARE @R_isAfterNoon bit = case when (DATEPART(HOUR, @R_time) >=15) then 1 else 0 end
DECLARE @R_area NVARCHAR(255) = (SELECT area FROM unityRide WHERE ridePatNum = @ridePatNum);
DECLARE @dayInWeek NVARCHAR(20);
DECLARE @R_regionID_origin INT = (select RegionId from location where Name Like @R_origin)
DECLARE @R_regionID_dest INT = (select RegionId from location where Name Like @R_dest)

--distance vars
DECLARE @R_origin_lat FLOAT = (SELECT lat from Location where Name like @R_origin) -- x
DECLARE @R_origin_lng FLOAT = (SELECT lng from Location where Name like @R_origin) -- y
DECLARE @R_destination_lat FLOAT = (SELECT lat from Location where Name like @R_dest) --x
DECLARE @R_destination_lng FLOAT = (SELECT lng from Location where Name like @R_dest) --y





--declare Geo Points
DECLARE @Start_point GEOGRAPHY = GEOGRAPHY::Point(@R_origin_lat, @R_origin_lng, 4326); 
DECLARE @End_point GEOGRAPHY = GEOGRAPHY::Point(@R_destination_lat, @R_destination_lng, 4326); 

SET @dayInWeek = 
    CASE DATENAME(WEEKDAY, @R_time)
        WHEN 'Sunday' THEN N'ראשון'
        WHEN 'Monday' THEN N'שני'
        WHEN 'Tuesday' THEN N'שלישי'
        WHEN 'Wednesday' THEN N'רביעי'
        WHEN 'Thursday' THEN N'חמישי'
        WHEN 'Friday' THEN N'שישי'
        WHEN 'Saturday' THEN N'שבת'
    END;



-- Attempt to optimize the query

select *
into #tempUnity
from UnityRide
where pickuptime >=DATEADD(year, -1, GETDATE()) and pickuptime<= DATEADD(DAY, 30, GETDATE()) AND status not like N'נמחקה'


-- Attempt to optimize the query
	select * 
	into #ShortenDriversTable
	from Volunteer v
	--only volunteers that active and driving.
	where isActive =1 AND isDriving = 1
	--only volunteers that not in absence
	AND v.Id NOT IN (SELECT volunteerId FROM Absence WHERE @R_time >= FromDate AND @R_time <= UntilDate)
	--volunteers that didnt talk with them today*
    AND v.Id NOT IN (SELECT DriverId FROM documentedCall WHERE CAST(GETDATE() AS DATE) = CallRecordedDate)
	--volunteers who isnt drive in this day (ride day)
    AND v.Id NOT IN (SELECT DISTINCT mainDriver FROM #tempUnity WHERE CAST(pickupTime AS DATE) = CAST(@R_time AS DATE) AND mainDriver IS NOT NULL )
	--volunteers who isnt drive day before the ride day
	AND v.Id NOT IN (SELECT DISTINCT MainDriver FROM #tempUnity WHERE CAST(pickupTime AS DATE)= DATEADD(day, -1, CAST(@R_time AS DATE)) AND MainDriver is NOT NULL  )
	--volunteers who isnt drive day after the ride day
	AND v.Id NOT IN (SELECT DISTINCT MainDriver FROM #tempUnity WHERE CAST(pickupTime AS DATE)= DATEADD(day, 1, CAST(@R_time AS DATE)) AND MainDriver is NOT NULL  );




	--NEWBIS
	select * ,'NEWBIS' as Vtype
	into #newbis
	from #ShortenDriversTable
	where DATEDIFF(DAY,JoinDate,GETDATE())<=260

	--Regular
	select * ,'REGULAR' as Vtype
	into #regular
	from #ShortenDriversTable
	where NoOfDocumentedRides >3 AND NoOfDocumentedRides<100 AND  DATEDIFF(DAY,JoinDate,GETDATE())>=60


	--SUPER
	select * ,'SUPER' as Vtype
	into #super
	from #ShortenDriversTable
	where NoOfDocumentedRides>=100;



-- this is query to calc the points before the main query
WITH VolunteerCityPoints AS (
    SELECT 
        v.Id,
        GEOGRAPHY::Point(ISNULL(c.lat, 32.0853), ISNULL(c.lng, 34.7818), 4326) AS VolunteerCityPoint
    FROM 
        #ShortenDriversTable v
    JOIN 
        City c ON v.CityCityName = c.CityName
),


newbisCTE AS (

	SELECT  Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides,Vtype,
			(DATEDIFF(day, JoinDate, GETDATE())) / 365.0 as 'SeniorityInYears',
			(SELECT TOP 1 CAST(CallRecordedDate AS DATETIME) + CAST(CallRecordedTime AS DATETIME) 
			FROM documentedCall 
			WHERE driverId = n.Id 
			ORDER BY CallRecordedDate DESC) as 'LastCallDateTime',
			DATEDIFF(day, (SELECT TOP 1 pickupTime 
                       FROM #tempUnity 
                       WHERE MainDriver = n.Id AND pickupTime < GETDATE()   
                       ORDER BY pickupTime DESC), GETDATE()) as 'LastRideInDays',

			DATEDIFF(day, GETDATE(), (SELECT TOP 1 pickupTime 
                                  FROM #tempUnity 
                                  WHERE MainDriver = n.Id AND pickupTime >= GETDATE()   
                                  ORDER BY pickupTime)) as 'NextRideInDays',

			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE MainDriver = n.Id  AND pickupTime BETWEEN DATEADD(MONTH, -2, GETDATE()) AND GETDATE()) as 'NumOfRidesLast2Month',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE origin LIKE @R_origin AND destination LIKE @R_dest AND mainDriver = n.Id ) as 'AmountOfRidesInThisPath',
			 (SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = n.Id AND tu.destination LIKE @R_dest AND tu.Origin NOT LIKE @R_origin   AND l.RegionId = @R_regionID_origin) AS 'AmountOfRidesFromRegionToDest',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE destination LIKE @R_origin AND origin LIKE @R_dest AND mainDriver = n.Id ) as 'AmountOfRidesInOppositePath',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = n.Id AND tu.Origin LIKE @R_origin AND tu.Destination NOT LIKE @R_dest   AND l.RegionId = @R_regionID_dest) as 'AmountOfRides_OriginToArea',
			 (CASE WHEN @R_isAfterNoon = 1 
              THEN (SELECT COUNT(*) FROM #tempUnity WHERE n.Id = mainDriver AND DATEPART(HOUR, pickupTime) >= 15 ) 
              ELSE (SELECT COUNT(*) FROM #tempUnity WHERE n.Id = mainDriver AND DATEPART(HOUR, pickupTime) < 15 ) END) as 'AmountOfRidesAtThisTime',
			 (SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE DATENAME(WEEKDAY, @R_time) = DATENAME(WEEKDAY, pickupTime) AND MainDriver = n.Id ) as 'AmountOfRidesAtThisDayWeek',

			 (SELECT(
			 (SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = n.Id).STDistance(@Start_point)
			 + 
             @Start_point.STDistance(@End_point)
			 + 
             @End_point.STDistance((SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = n.Id))
			 )/1000
			 ) as 'SumOfKM'

	FROM #newbis n

),


RegularCTE AS (

	SELECT  Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides,Vtype,
			(DATEDIFF(day, JoinDate, GETDATE())) / 365.0 as 'SeniorityInYears',
			(SELECT TOP 1 CAST(CallRecordedDate AS DATETIME) + CAST(CallRecordedTime AS DATETIME) 
			FROM documentedCall 
			WHERE driverId = r.Id 
			ORDER BY CallRecordedDate DESC) as 'LastCallDateTime',
			DATEDIFF(day, (SELECT TOP 1 pickupTime 
                       FROM #tempUnity 
                       WHERE MainDriver = r.Id AND pickupTime < GETDATE()   
                       ORDER BY pickupTime DESC), GETDATE()) as 'LastRideInDays',

			DATEDIFF(day, GETDATE(), (SELECT TOP 1 pickupTime 
                                  FROM #tempUnity 
                                  WHERE MainDriver = r.Id AND pickupTime >= GETDATE()   
                                  ORDER BY pickupTime)) as 'NextRideInDays',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = r.Id AND tu.destination LIKE @R_dest AND tu.Origin NOT LIKE @R_origin   AND l.RegionId = @R_regionID_origin) AS 'AmountOfRidesFromRegionToDest',

			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE MainDriver = r.Id  AND pickupTime BETWEEN DATEADD(MONTH, -2, GETDATE()) AND GETDATE()) as 'NumOfRidesLast2Month',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE origin LIKE @R_origin AND destination LIKE @R_dest AND mainDriver = r.Id ) as 'AmountOfRidesInThisPath',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE destination LIKE @R_origin AND origin LIKE @R_dest AND mainDriver = r.Id ) as 'AmountOfRidesInOppositePath',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = r.Id AND tu.Origin LIKE @R_origin AND tu.Destination NOT LIKE @R_dest   AND l.RegionId = @R_regionID_dest) as 'AmountOfRides_OriginToArea',
			 (CASE WHEN @R_isAfterNoon = 1 
              THEN (SELECT COUNT(*) FROM #tempUnity WHERE r.Id = mainDriver AND DATEPART(HOUR, pickupTime) >= 15 ) 
              ELSE (SELECT COUNT(*) FROM #tempUnity WHERE r.Id = mainDriver AND DATEPART(HOUR, pickupTime) < 15  ) END) as 'AmountOfRidesAtThisTime',
			 (SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE DATENAME(WEEKDAY, @R_time) = DATENAME(WEEKDAY, pickupTime) AND MainDriver = r.Id ) as 'AmountOfRidesAtThisDayWeek',
			 (SELECT (SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = r.Id).STDistance(@Start_point) / 1000 + 
             (@Start_point.STDistance(@End_point) / 1000) + 
             (@End_point.STDistance((SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = r.Id)) / 1000)) as 'SumOfKM'
	FROM #regular r

),


SuperCTE AS (

	SELECT  Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides,Vtype,
			(DATEDIFF(day, JoinDate, GETDATE())) / 365.0 as 'SeniorityInYears',
			(SELECT TOP 1 CAST(CallRecordedDate AS DATETIME) + CAST(CallRecordedTime AS DATETIME) 
			FROM documentedCall 
			WHERE driverId = s.Id 
			ORDER BY CallRecordedDate DESC) as 'LastCallDateTime',
			DATEDIFF(day, (SELECT TOP 1 pickupTime 
                       FROM #tempUnity 
                       WHERE MainDriver = s.Id AND pickupTime < GETDATE()   
                       ORDER BY pickupTime DESC), GETDATE()) as 'LastRideInDays',

			DATEDIFF(day, GETDATE(), (SELECT TOP 1 pickupTime 
                                  FROM #tempUnity 
                                  WHERE MainDriver = s.Id AND pickupTime >= GETDATE()   
                                  ORDER BY pickupTime)) as 'NextRideInDays',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = s.Id AND tu.destination LIKE @R_dest AND tu.Origin NOT LIKE @R_origin   AND l.RegionId = @R_regionID_origin) AS 'AmountOfRidesFromRegionToDest',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE MainDriver = s.Id  AND pickupTime BETWEEN DATEADD(MONTH, -2, GETDATE()) AND GETDATE()) as 'NumOfRidesLast2Month',
			(SELECT COUNT(*) 
			 FROM #tempUnity tu
			 WHERE tu.Origin LIKE @R_origin AND tu.Destination LIKE @R_dest AND tu.MainDriver = Id ) as 'AmountOfRidesInThisPath',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE destination LIKE @R_origin AND origin LIKE @R_dest AND mainDriver = s.Id ) as 'AmountOfRidesInOppositePath',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = s.Id AND tu.Origin LIKE @R_origin AND tu.Destination NOT LIKE @R_dest   AND l.RegionId = @R_regionID_dest) as 'AmountOfRides_OriginToArea',
			(CASE WHEN @R_isAfterNoon = 1 
              THEN (SELECT COUNT(*) FROM #tempUnity WHERE s.Id = mainDriver AND DATEPART(HOUR, pickupTime) >= 15 ) 
              ELSE (SELECT COUNT(*) FROM #tempUnity WHERE s.Id = mainDriver AND DATEPART(HOUR, pickupTime) < 15  ) END) as 'AmountOfRidesAtThisTime',
			 (SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE DATENAME(WEEKDAY, @R_time) = DATENAME(WEEKDAY, pickupTime) AND MainDriver = s.Id ) as 'AmountOfRidesAtThisDayWeek',
			 (SELECT (SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = s.Id).STDistance(@Start_point) / 1000 + 
             (@Start_point.STDistance(@End_point) / 1000) + 
             (@End_point.STDistance((SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = s.Id)) / 1000)) as 'SumOfKM'
	FROM #super s

),


newbis_25_Temp AS (
    SELECT TOP 25 *
    FROM newbisCTE
    ORDER BY SumOfKM
),
reg_25_Temp AS (
    SELECT TOP 25 *
    FROM RegularCTE
    ORDER BY AmountOfRidesInThisPath DESC
),
super_25_Temp AS (
    SELECT TOP 25 *
    FROM SuperCTE
    ORDER BY AmountOfRidesInThisPath DESC
)

SELECT Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides, Vtype,
       SeniorityInYears, LastCallDateTime, LastRideInDays, NextRideInDays,
       AmountOfRidesFromRegionToDest, NumOfRidesLast2Month, AmountOfRidesInThisPath,
       AmountOfRidesInOppositePath, AmountOfRides_OriginToArea, AmountOfRidesAtThisTime,
       AmountOfRidesAtThisDayWeek, SumOfKM,(select  count(*) from DocumentedCall where DriverId = Id) as 'NoOfDocumentedCalls'
FROM newbis_25_Temp
UNION ALL
SELECT Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides, Vtype,
       SeniorityInYears, LastCallDateTime, LastRideInDays, NextRideInDays,
       AmountOfRidesFromRegionToDest, NumOfRidesLast2Month, AmountOfRidesInThisPath,
       AmountOfRidesInOppositePath, AmountOfRides_OriginToArea, AmountOfRidesAtThisTime,
       AmountOfRidesAtThisDayWeek, SumOfKM,(select  count(*) from DocumentedCall where DriverId = Id) as 'NoOfDocumentedCalls'
FROM reg_25_Temp
UNION ALL
SELECT Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides, Vtype,
       SeniorityInYears, LastCallDateTime, LastRideInDays, NextRideInDays,
       AmountOfRidesFromRegionToDest, NumOfRidesLast2Month, AmountOfRidesInThisPath,
       AmountOfRidesInOppositePath, AmountOfRides_OriginToArea, AmountOfRidesAtThisTime,
       AmountOfRidesAtThisDayWeek, SumOfKM,(select  count(*) from DocumentedCall where DriverId = Id) as 'NoOfDocumentedCalls'
FROM super_25_Temp;





	--drop tables
	drop table #ShortenDriversTable,#newbis,#regular,#super,#tempUnity
END




---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------


-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad Meirson>
-- Create Date: <01/10/2024>
-- Description: <get all managers from db to valid if display spesific button in the client>
-- =============================================
CREATE PROCEDURE sp_GetManagersTypeVolunteers

AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    

    -- Insert statements for procedure here
    select CellPhone
	from  VolunType_Volunteer inner join volunteer on Id=VolunteerId
	where VolunTypeType like N'מנהל' AND IsActive = 1
END
GO





---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_GetPatientList]    Script Date: 16/11/2024 18:38:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad Meirson>
-- Create Date: <18/06/2024>
-- Description: <this sp is for efficiency and time tests,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_GetPatientList]
(
    -- Add the parameters for the stored procedure here
    @active bit
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
   

    -- Insert statements for procedure here
	select Id,DisplayName,Cellphone,
	case when HomePhone !='' then HomePhone 
	else '0' end as'CellPhone2',
	BirthDate,Gender,Hospital,Barrier,PatientIdentity,LastModified,EnglishName,isActive
	from Patient where IsActive = @active  order by Id
END

---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------


--NOTE HERE 
-- DO NOT FORGET TO EDIT the volunteerTypeView-> IsBooster and No_of_Rides  !!!!


ALTER TABLE volunteer
ADD No_of_Rides int;

ALTER TABLE volunteer
ADD IsBooster bit;

select * 
from volunteer

UPDATE volunteer
SET IsBooster = 0


ALTER TABLE volunteer
ADD IsBabyChair bit;


UPDATE volunteer
SET IsBabyChair = 0
 

UPDATE volunteer
Set No_of_Rides = (
SELECT COUNT(DISTINCT pickuptime)
    FROM unityRide
    WHERE unityRide.maindriver = volunteer.id
    AND status NOT LIKE N'נמחקה'
)


-- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- -- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- -- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- 
-- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- -- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- -- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- 
-- the ALTER of the view !!!! 

/****** Object:  View [dbo].[VolunteerTypeView]    Script Date: 05/02/2025 17:02:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER view [dbo].[VolunteerTypeView]
as
SELECT        dbo.Volunteer.Id, dbo.Volunteer.DisplayName, dbo.Volunteer.FirstNameH, dbo.Volunteer.LastNameH, dbo.Volunteer.CellPhone, dbo.Volunteer.CellPhone2, dbo.Volunteer.HomePhone, dbo.Volunteer.Address, dbo.Volunteer.Email, 
                         dbo.Volunteer.JoinDate, dbo.Volunteer.IsActive, dbo.Volunteer.KnowsArabic, dbo.Volunteer.BirthDate, dbo.Volunteer.Gender, dbo.Volunteer.Remarks, dbo.Volunteer.Department, dbo.Volunteer.UserName, dbo.Volunteer.Password, 
                         dbo.Volunteer.CityCityName, dbo.Volunteer.pnRegId, dbo.Volunteer.FirstNameA, dbo.Volunteer.LastNameA, dbo.Volunteer.AvailableSeats, dbo.VolunType_Volunteer.VolunTypeType, dbo.Volunteer.device, 
                         dbo.Volunteer.EnglishName, dbo.Volunteer.isAssistant, dbo.Volunteer.LastModified, dbo.Volunteer.VolunteerIdentity, dbo.Volunteer.EnglishFN, dbo.Volunteer.EnglishLN, dbo.Volunteer.isDriving, dbo.Volunteer.howCanHelp, 
                         dbo.Volunteer.feedback, dbo.Volunteer.NewsLetter, dbo.Volunteer.Refered, dbo.Volunteer.RoleInR2R, dbo.Volunteer.NoOfDocumentedCalls, dbo.Volunteer.NoOfDocumentedRides,dbo.Volunteer.No_of_Rides,dbo.Volunteer.IsBooster,dbo.volunteer.IsBabyChair
FROM            dbo.Volunteer INNER JOIN
                         dbo.VolunType_Volunteer ON dbo.Volunteer.Id = dbo.VolunType_Volunteer.VolunteerId

GO


-- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- -- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- -- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- 
-- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- -- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- -- IMPORTANT --  -- IMPORTANT --  -- IMPORTANT -- 



---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spDeleteUnityRide]    Script Date: 17/11/2024 18:39:49 ******/
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


		IF(@DriverId IS not NULL)
		update Volunteer
		SET NoOfDocumentedRides = NoOfDocumentedRides-1
		where Id = @DriverId

		IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @dest =Destination and @pickupTime = pickupTime and MainDriver = @DriverId)
		update Volunteer
		SET No_of_Rides = No_of_Rides-1
		Where Id = @DriverId

		
		IF(@DriverId IS NULL and @isAnonymous = 1)

		BEGIN
		DELETE FROM UnityRide
		WHERE ridepatnum = @unityRideID;
		select @unityRideID*-1 as 'RidePatNum'
		END
		
		ELSE

		BEGIN
		UPDATE UnityRide
		set Status = N'נמחקה', lastModified = GETDATE(),Coordinator = @CoorName , CoordinatorID = @CoorId
		where ridepatnum = @unityRideID
		--return the update ride
		Select * from UnityRide where  ridepatnum = @unityRideID
		END



END




---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spAssignDriverMobile]    Script Date: 17/11/2024 18:49:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <21/01/24>
-- Description: assign Driver to unityRide Table for mobile service
-- =============================================
ALTER PROCEDURE [dbo].[spAssignDriverMobile]
(
	@unityRideID INT,
	@userId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
	DECLARE @driverId int = @userId
	DECLARE @hasAlreadyDriver bit = case when (select MainDriver from UnityRide where ridePatNum = @unityRideID) is null then 0 else 1 end
	DECLARE @hasRemove bit = case when (select status from UnityRide where ridePatNum = @unityRideID) like N'נמחקה' then 1 else 0 end
	DECLARE @isExist bit = case when (select ridePatNum from UnityRide where ridePatNum = @unityRideID) is null then 0 else 1 end

	DECLARE @origin Nvarchar(55) = (select origin from UnityRide where RidePatNum = @unityRideID)
	DECLARE @dest Nvarchar(55) = (select Destination from UnityRide where RidePatNum = @unityRideID)
	DECLARE @pickupTime dateTime = (select pickupTime from UnityRide where  RidePatNum = @unityRideID)

	IF @driverId is null
	begin
		SELECT 1 AS IsError, N'בעיה במספר הנייד של הנהג' AS Message
		RETURN;
	end

	IF @hasAlreadyDriver = 1
	begin
		SELECT 1 AS IsError, N'הנסיעה אליה נרשמת כבר מלאה' AS Message
		RETURN;
	end
	IF @hasRemove = 1 or @isExist = 0
	begin
		SELECT 1 AS IsError, N'נסיעה זו בוטלה, תודה על הרצון לעזור' AS Message
		RETURN;
	end

	IF @hasAlreadyDriver = 0 and @hasRemove = 0 and @isExist = 1
		begin
		
		DECLARE @driverName nvarchar(255) = (select displayname from volunteer where Id=@driverId)
		DECLARE @isNewDriver bit = case when (select count(*) from UnityRide where pickupTime<=GETDATE() and MainDriver=@driverId)<=3 then 1 else 0 end
		
		update Volunteer
		Set NoOfDocumentedRides = NoOfDocumentedRides+1
		where id = @driverId

		IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @dest =Destination and @pickupTime = pickupTime and MainDriver = @driverId)
		update Volunteer
		SET No_of_Rides = No_of_Rides+1
		Where Id = @driverId

		DECLARE @NoOfDocumentedRides int = (select NoOfDocumentedRides from volunteer where id= @driverId)
		
		update UnityRide
		Set DriverCellPhone = (select cellphone from Volunteer where id = @userId),
			DriverName = @driverName,
			IsNewDriver = @isNewDriver,
			MainDriver = @driverId,
			NoOfDocumentedRides = @NoOfDocumentedRides,
			lastModified = GETDATE(),
			status = N'שובץ נהג'
		where RidePatNum = @unityRideID

		--SELECT 0 AS IsError, N'הנסיעה עודכנה בהצלחה' AS Message, @unityRideID AS RideId
		select *,  0 AS IsError, N'הנסיעה עודכנה בהצלחה' AS Message, @unityRideID AS RideId
		 from UnityRide where RidePatNum = @unityRideID
		end
END



---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spDriverLeaveUnityRide]    Script Date: 17/11/2024 18:52:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <22/01/24>
-- Description: <driver want to leave a ride from mobile>
-- =============================================
ALTER PROCEDURE [dbo].[spDriverLeaveUnityRide]
(
    -- Add the parameters for the stored procedure here
   @driverId int,
   @UnityRideID int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
		DECLARE @origin Nvarchar(55) = (select origin from UnityRide where RidePatNum = @unityRideID)
		DECLARE @dest Nvarchar(55) = (select Destination from UnityRide where RidePatNum = @unityRideID)
		DECLARE @pickupTime dateTime = (select pickupTime from UnityRide where  RidePatNum = @unityRideID)
    -- Insert statements for procedure here
	IF NOT EXISTS (select 1 from UnityRide where Origin = @origin and @dest =Destination and @pickupTime = pickupTime and MainDriver = @driverId)
		update Volunteer
		SET No_of_Rides = No_of_Rides-1
		Where Id = @driverId



	IF EXISTS (select 1 from UnityRide where RidePatNum = @UnityRideID and MainDriver =@driverId)
		begin
			update Volunteer
			Set NoOfDocumentedRides = NoOfDocumentedRides -1
			where Id = @driverId

			update UnityRide
			Set DriverName = NULL,
			MainDriver = NULL,
			DriverCellPhone = NULL,
			NoOfDocumentedRides = NULL,
			IsNewDriver = 1,
			lastModified = GETDATE(),
			Status = N'ממתינה לשיבוץ'
			where RidePatNum = @UnityRideID and MainDriver = @driverId

			select * from UnityRide where RidePatNum = @UnityRideID 
		end
	ELSE
		select -1 as RidePatNum
END


---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spSetNewUnityRide]    Script Date: 17/11/2024 18:55:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <09/12/23>
-- Description: <this sp is for insert data to unity ride and [PatientEscort_PatientInRide (RidePat)] if need to. >
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

---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spUpdateDriverUnityRide]    Script Date: 17/11/2024 18:57:59 ******/
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
		lastModified = GETDATE(),
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
		lastModified = GETDATE(),
		Coordinator = @CoorName,
		CoordinatorID = @coorId,
		Status = N'ממתינה לשיבוץ'
	where RidePatNum = @unityRideID

select * from UnityRide where RidePatNum = @unityRideID
END



---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spUpdateRideInUnityRide]    Script Date: 17/11/2024 19:09:09 ******/
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
	lastModified = GETDATE(),
	IsNewDriver = @isNewDriver,
	AmountOfEquipments = @AmountOfEquipments,
	status = case when @driverId is null then N'ממתינה לשיבוץ' else N'שובץ נהג' end
	WHERE RidePatNum=@unityRideId;
	
	ELSE 
	return -1

END



---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------


---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

update Location 
set EnglishName = 'Sheeba'
where Name like N'שיבא'

---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spVolunteer_ToggleIsDrive]    Script Date: 16/01/2025 11:36:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 14/12/2022 @night
-- Description: Active Or Deactivate Volunteer's isDriving
-- Returns IsSuccesfullOperation bit, and optional VolunteerWithFutureRidesIncludedToday
-- when try to deactivate volunteer
--ALTER by Gilad - there is a bug --> SET isActive=@isDriving, cause to confusing in volunteer active status.
-- it is change to be --> SET isDriving=@isDriving,
-- =============================================
ALTER    PROCEDURE [dbo].[spVolunteer_ToggleIsDrive]
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
			SELECT COUNT(*) FROM UnityRide 
			WHERE MAINDRIVER=@volunteerId
			AND  GETDATE() <= pickupTime
		   ) = 0
			BEGIN
				UPDATE Volunteer 
				SET isDriving=@isDriving, 
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


---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spVolunteerTypeView_GetVolunteersList_Gilad]    Script Date: 05/02/2025 21:44:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:       Gilad
-- ALTER Date:	 16/08/2023
-- ALTER Reason: try to add addition of absence to this sp 
-- ALTER Reason : add status != 'נמחקה' cause the lastDrive Date was incorrecet
-- =============================================
ALTER procedure [dbo].[spVolunteerTypeView_GetVolunteersList_Gilad] 

@IsActive bit
as
begin
		--Gilad addioton
       UPDATE Absence
    SET AbsenceStatus = CASE
                         WHEN GETDATE() BETWEEN FromDate AND DATEADD(d,1,UntilDate) THEN 1
                         ELSE 0
                       END;

select r.MainDriver, r.Origin, r.Destination, r.pickupTime
into #tempNotDeletedOnly from  UnityRide r

			select VolunteerId,AbsenceStatus
			into #tempAbsence
			from Absence
			where  GETDATE() BETWEEN FromDate AND DATEADD(d,1,UntilDate) and isDeleted = 0
			group by VolunteerId,AbsenceStatus

--before the unity
--SELECT v.id, MAX(r.date) AS latestDrive into #tempLatesetDrives
--					FROM Volunteer v
--					JOIN Ride r ON v.id = r.MainDriver
--					GROUP BY v.id
--after the unity
SELECT mainDriver AS id, MAX(PickupTime) AS latestDrive into #tempLatesetDrives
FROM UnityRide
where [Status] not like N'נמחקה'
GROUP BY mainDriver
--Id,
--DisplayName,
--FirstNameA,
--FirstNameH,
--LastNameH,
--LastNameA,
--CellPhone,
--CellPhone2,
--HomePhone,
--Remarks,
--CityCityName,
--Address,
--VolunTypeType,
--Email,
--device,
--NoOfDocumentedCalls,
--NoOfDocumentedRides,
--NumOfRides_last2Months,
--mostCommonPath,
--latestDrive,
--JoinDate,
--isAssistant,
--IsActive,
--KnowsArabic,
--Gender,
--pnRegId,
--englishName,
--lastModified,
--isDriving,
if (@IsActive = 0)
	begin
				select vtv.Id,vtv.DisplayName,vtv.FirstNameA,vtv.FirstNameH,
				vtv.LastNameH,vtv.LastNameA,vtv.CellPhone,vtv.CellPhone2,
				vtv.HomePhone,vtv.Remarks,vtv.CityCityName,vtv.Address,
				vtv.VolunTypeType,vtv.Email,vtv.device,vtv.NoOfDocumentedCalls,
				vtv.NoOfDocumentedRides,vtv.No_of_Rides,vtv.JoinDate,vtv.isAssistant,vtv.IsActive,
				vtv.KnowsArabic,vtv.Gender,vtv.pnRegId,vtv.EnglishName,DATEADD(HOUR, -2, vtv.LastModified) as LastModified,vtv.isDriving,vtv.AvailableSeats,vtv.IsBooster,vtv.IsBabyChair,
				(select count(*)
					from UnityRide
					where maindriver = vtv.Id  and Status != N'נמחקה'
					and pickuptime between DATEADD(Month, -2, GETDATE()) and  GETDATE()) as NumOfRides_last2Months
					--gilad addition vvv
					,(
					select abse.AbsenceStatus
					from #tempAbsence abse
					where abse.VolunteerId=vtv.Id
					) as AbsenceStatus,
					--gilad addition ^^^
					(
				select origin + '-' + destination from
											(
												select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove
												FROM (
													select top 50 *
													from #tempNotDeletedOnly t
													where t.MainDriver = vtv.Id
													order by pickupTime desc
												) last50Rides
												group by maindriver, origin, destination
												order by numberOfTimesDrove desc
											) t
					) 
											mostCommonPath, tld.latestDrive
		from VolunteerTypeView vtv
		left join #tempLatesetDrives tld on tld.Id=vtv.Id
		where IsActive = @IsActive --or IsActive = 1
		order by firstNameH

	end
else
	begin
	select vtv.Id,vtv.DisplayName,vtv.FirstNameA,vtv.FirstNameH,
				vtv.LastNameH,vtv.LastNameA,vtv.CellPhone,vtv.CellPhone2,
				vtv.HomePhone,vtv.Remarks,vtv.CityCityName,vtv.Address,
				vtv.VolunTypeType,vtv.Email,vtv.device,vtv.NoOfDocumentedCalls,
				vtv.NoOfDocumentedRides,vtv.No_of_Rides,vtv.JoinDate,vtv.isAssistant,vtv.IsActive,
				vtv.KnowsArabic,vtv.Gender,vtv.pnRegId,vtv.EnglishName,DATEADD(HOUR, -2, vtv.LastModified)  as LastModified,vtv.isDriving,vtv.AvailableSeats,vtv.IsBooster,vtv.IsBabyChair,
				(select count(*)
					from UnityRide
					where maindriver = vtv.Id and Status != N'נמחקה'
					and pickuptime between DATEADD(Month, -2, GETDATE()) and  GETDATE()) as NumOfRides_last2Months
					--gilad addition vvv
					,(
					select abse.AbsenceStatus
					from #tempAbsence abse
					where abse.VolunteerId=vtv.Id
					) as AbsenceStatus,
					--gilad addition ^^^
					(
				select origin + '-' + destination from
										(
											select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove
											FROM (
												select top 50 *
												from #tempNotDeletedOnly t
												where t.MainDriver = vtv.Id
												order by pickupTime desc
											) last50Rides
											group by maindriver, origin, destination
											order by numberOfTimesDrove desc
										) t
					) mostCommonPath, tld.latestDrive
		from VolunteerTypeView vtv
		left join #tempLatesetDrives tld on tld.Id=vtv.Id 
		where IsActive = @IsActive
		order by firstNameH
	end

	drop table #tempNotDeletedOnly, #tempLatesetDrives,#tempAbsence

end
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[sp_getALLCandidateUnityRideV2]    Script Date: 11/02/2025 9:28:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad Meirson>
-- Create Date: <17/09/2024>
-- Description: <get all type of volunteer to unit Table for candidate moudle>
-- =============================================
ALTER PROCEDURE [dbo].[sp_getALLCandidateUnityRideV2]
(
    -- Add the parameters for the stored procedure here
    @ridePatNum INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    

    -- Insert statements for procedure here
    --declare basic vars 
DECLARE @R_origin NVARCHAR(255) = (SELECT origin FROM unityRide WHERE ridePatNum = @ridePatNum);
DECLARE @R_dest NVARCHAR(255) = (SELECT destination FROM unityRide WHERE ridePatNum = @ridePatNum);
DECLARE @R_time DateTIme = (SELECT pickupTime FROM unityRide WHERE ridePatNum = @ridePatNum);
DECLARE @R_isAfterNoon bit = case when (DATEPART(HOUR, @R_time) >=15) then 1 else 0 end
DECLARE @R_area NVARCHAR(255) = (SELECT area FROM unityRide WHERE ridePatNum = @ridePatNum);
DECLARE @dayInWeek NVARCHAR(20);
DECLARE @R_regionID_origin INT = (select RegionId from location where Name Like @R_origin)
DECLARE @R_regionID_dest INT = (select RegionId from location where Name Like @R_dest)

--distance vars
DECLARE @R_origin_lat FLOAT = (SELECT lat from Location where Name like @R_origin) -- x
DECLARE @R_origin_lng FLOAT = (SELECT lng from Location where Name like @R_origin) -- y
DECLARE @R_destination_lat FLOAT = (SELECT lat from Location where Name like @R_dest) --x
DECLARE @R_destination_lng FLOAT = (SELECT lng from Location where Name like @R_dest) --y





--declare Geo Points
DECLARE @Start_point GEOGRAPHY = GEOGRAPHY::Point(@R_origin_lat, @R_origin_lng, 4326); 
DECLARE @End_point GEOGRAPHY = GEOGRAPHY::Point(@R_destination_lat, @R_destination_lng, 4326); 

SET @dayInWeek = 
    CASE DATENAME(WEEKDAY, @R_time)
        WHEN 'Sunday' THEN N'ראשון'
        WHEN 'Monday' THEN N'שני'
        WHEN 'Tuesday' THEN N'שלישי'
        WHEN 'Wednesday' THEN N'רביעי'
        WHEN 'Thursday' THEN N'חמישי'
        WHEN 'Friday' THEN N'שישי'
        WHEN 'Saturday' THEN N'שבת'
    END;



-- Attempt to optimize the query

select *
into #tempUnity
from UnityRide
where pickuptime >=DATEADD(year, -1, GETDATE()) and pickuptime<= DATEADD(DAY, 30, GETDATE()) AND status not like N'נמחקה'


-- Attempt to optimize the query
	select * 
	into #ShortenDriversTable
	from Volunteer v
	--only volunteers that active and driving.
	where isActive =1 AND isDriving = 1 
	--only volunteers that not in absence
	AND v.Id NOT IN (SELECT volunteerId FROM Absence WHERE @R_time >= FromDate AND @R_time <= UntilDate)
	--volunteers that didnt talk with them today*
    AND v.Id NOT IN (SELECT DriverId FROM documentedCall WHERE CAST(GETDATE() AS DATE) = CallRecordedDate)
	--volunteers who isnt drive in this day (ride day)
    AND v.Id NOT IN (SELECT DISTINCT mainDriver FROM #tempUnity WHERE CAST(pickupTime AS DATE) = CAST(@R_time AS DATE) AND mainDriver IS NOT NULL )
	--volunteers who isnt drive day before the ride day
	AND v.Id NOT IN (SELECT DISTINCT MainDriver FROM #tempUnity WHERE CAST(pickupTime AS DATE)= DATEADD(day, -1, CAST(@R_time AS DATE)) AND MainDriver is NOT NULL  )
	--volunteers who isnt drive day after the ride day
	AND v.Id NOT IN (SELECT DISTINCT MainDriver FROM #tempUnity WHERE CAST(pickupTime AS DATE)= DATEADD(day, 1, CAST(@R_time AS DATE)) AND MainDriver is NOT NULL  )
	--volunteers who isnt managers
	AND v.Id NOT IN (select VolunteerId from VolunType_Volunteer where VolunTypeType not like N'מתנדב');




	--NEWBIS
	select * ,'NEWBIS' as Vtype
	into #newbis
	from #ShortenDriversTable
	where DATEDIFF(DAY,JoinDate,GETDATE())<=60 AND No_of_Rides<=5

	--Regular
	select * ,'REGULAR' as Vtype
	into #regular
	from #ShortenDriversTable
	where NoOfDocumentedRides >3 AND NoOfDocumentedRides<100 AND  DATEDIFF(DAY,JoinDate,GETDATE())>=60


	--SUPER
	select * ,'SUPER' as Vtype
	into #super
	from #ShortenDriversTable
	where NoOfDocumentedRides>=100;



-- this is query to calc the points before the main query
WITH VolunteerCityPoints AS (
    SELECT 
        v.Id,
        GEOGRAPHY::Point(ISNULL(c.lat, 32.0853), ISNULL(c.lng, 34.7818), 4326) AS VolunteerCityPoint
    FROM 
        #ShortenDriversTable v
    JOIN 
        City c ON v.CityCityName = c.CityName
),


newbisCTE AS (

	SELECT  Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides,Vtype,
			(DATEDIFF(day, JoinDate, GETDATE())) / 365.0 as 'SeniorityInYears',
			(SELECT TOP 1 CAST(CallRecordedDate AS DATETIME) + CAST(CallRecordedTime AS DATETIME) 
			FROM documentedCall 
			WHERE driverId = n.Id 
			ORDER BY CallRecordedDate DESC) as 'LastCallDateTime',
			DATEDIFF(day, (SELECT TOP 1 pickupTime 
                       FROM #tempUnity 
                       WHERE MainDriver = n.Id AND pickupTime < GETDATE()   
                       ORDER BY pickupTime DESC), GETDATE()) as 'LastRideInDays',

			DATEDIFF(day, GETDATE(), (SELECT TOP 1 pickupTime 
                                  FROM #tempUnity 
                                  WHERE MainDriver = n.Id AND pickupTime >= GETDATE()   
                                  ORDER BY pickupTime)) as 'NextRideInDays',

			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE MainDriver = n.Id  AND pickupTime BETWEEN DATEADD(MONTH, -2, GETDATE()) AND GETDATE()) as 'NumOfRidesLast2Month',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE origin LIKE @R_origin AND destination LIKE @R_dest AND mainDriver = n.Id ) as 'AmountOfRidesInThisPath',
			 (SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = n.Id AND tu.destination LIKE @R_dest AND tu.Origin NOT LIKE @R_origin   AND l.RegionId = @R_regionID_origin) AS 'AmountOfRidesFromRegionToDest',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE destination LIKE @R_origin AND origin LIKE @R_dest AND mainDriver = n.Id ) as 'AmountOfRidesInOppositePath',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = n.Id AND tu.Origin LIKE @R_origin AND tu.Destination NOT LIKE @R_dest   AND l.RegionId = @R_regionID_dest) as 'AmountOfRides_OriginToArea',
			 (CASE WHEN @R_isAfterNoon = 1 
              THEN (SELECT COUNT(*) FROM #tempUnity WHERE n.Id = mainDriver AND DATEPART(HOUR, pickupTime) >= 15 ) 
              ELSE (SELECT COUNT(*) FROM #tempUnity WHERE n.Id = mainDriver AND DATEPART(HOUR, pickupTime) < 15 ) END) as 'AmountOfRidesAtThisTime',
			 (SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE DATENAME(WEEKDAY, @R_time) = DATENAME(WEEKDAY, pickupTime) AND MainDriver = n.Id ) as 'AmountOfRidesAtThisDayWeek',

			 (SELECT(
			 (SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = n.Id).STDistance(@Start_point)
			 + 
             @Start_point.STDistance(@End_point)
			 + 
             @End_point.STDistance((SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = n.Id))
			 )/1000
			 ) as 'SumOfKM'

	FROM #newbis n

),


RegularCTE AS (

	SELECT  Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides,Vtype,
			(DATEDIFF(day, JoinDate, GETDATE())) / 365.0 as 'SeniorityInYears',
			(SELECT TOP 1 CAST(CallRecordedDate AS DATETIME) + CAST(CallRecordedTime AS DATETIME) 
			FROM documentedCall 
			WHERE driverId = r.Id 
			ORDER BY CallRecordedDate DESC) as 'LastCallDateTime',
			DATEDIFF(day, (SELECT TOP 1 pickupTime 
                       FROM #tempUnity 
                       WHERE MainDriver = r.Id AND pickupTime < GETDATE()   
                       ORDER BY pickupTime DESC), GETDATE()) as 'LastRideInDays',

			DATEDIFF(day, GETDATE(), (SELECT TOP 1 pickupTime 
                                  FROM #tempUnity 
                                  WHERE MainDriver = r.Id AND pickupTime >= GETDATE()   
                                  ORDER BY pickupTime)) as 'NextRideInDays',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = r.Id AND tu.destination LIKE @R_dest AND tu.Origin NOT LIKE @R_origin   AND l.RegionId = @R_regionID_origin) AS 'AmountOfRidesFromRegionToDest',

			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE MainDriver = r.Id  AND pickupTime BETWEEN DATEADD(MONTH, -2, GETDATE()) AND GETDATE()) as 'NumOfRidesLast2Month',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE origin LIKE @R_origin AND destination LIKE @R_dest AND mainDriver = r.Id ) as 'AmountOfRidesInThisPath',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE destination LIKE @R_origin AND origin LIKE @R_dest AND mainDriver = r.Id ) as 'AmountOfRidesInOppositePath',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = r.Id AND tu.Origin LIKE @R_origin AND tu.Destination NOT LIKE @R_dest   AND l.RegionId = @R_regionID_dest) as 'AmountOfRides_OriginToArea',
			 (CASE WHEN @R_isAfterNoon = 1 
              THEN (SELECT COUNT(*) FROM #tempUnity WHERE r.Id = mainDriver AND DATEPART(HOUR, pickupTime) >= 15 ) 
              ELSE (SELECT COUNT(*) FROM #tempUnity WHERE r.Id = mainDriver AND DATEPART(HOUR, pickupTime) < 15  ) END) as 'AmountOfRidesAtThisTime',
			 (SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE DATENAME(WEEKDAY, @R_time) = DATENAME(WEEKDAY, pickupTime) AND MainDriver = r.Id ) as 'AmountOfRidesAtThisDayWeek',
			 (SELECT (SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = r.Id).STDistance(@Start_point) / 1000 + 
             (@Start_point.STDistance(@End_point) / 1000) + 
             (@End_point.STDistance((SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = r.Id)) / 1000)) as 'SumOfKM'
	FROM #regular r

),


SuperCTE AS (

	SELECT  Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides,Vtype,
			(DATEDIFF(day, JoinDate, GETDATE())) / 365.0 as 'SeniorityInYears',
			(SELECT TOP 1 CAST(CallRecordedDate AS DATETIME) + CAST(CallRecordedTime AS DATETIME) 
			FROM documentedCall 
			WHERE driverId = s.Id 
			ORDER BY CallRecordedDate DESC) as 'LastCallDateTime',
			DATEDIFF(day, (SELECT TOP 1 pickupTime 
                       FROM #tempUnity 
                       WHERE MainDriver = s.Id AND pickupTime < GETDATE()   
                       ORDER BY pickupTime DESC), GETDATE()) as 'LastRideInDays',

			DATEDIFF(day, GETDATE(), (SELECT TOP 1 pickupTime 
                                  FROM #tempUnity 
                                  WHERE MainDriver = s.Id AND pickupTime >= GETDATE()   
                                  ORDER BY pickupTime)) as 'NextRideInDays',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = s.Id AND tu.destination LIKE @R_dest AND tu.Origin NOT LIKE @R_origin   AND l.RegionId = @R_regionID_origin) AS 'AmountOfRidesFromRegionToDest',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE MainDriver = s.Id  AND pickupTime BETWEEN DATEADD(MONTH, -2, GETDATE()) AND GETDATE()) as 'NumOfRidesLast2Month',
			(SELECT COUNT(*) 
			 FROM #tempUnity tu
			 WHERE tu.Origin LIKE @R_origin AND tu.Destination LIKE @R_dest AND tu.MainDriver = Id ) as 'AmountOfRidesInThisPath',
			(SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE destination LIKE @R_origin AND origin LIKE @R_dest AND mainDriver = s.Id ) as 'AmountOfRidesInOppositePath',
			(SELECT COUNT(*)
				 FROM #tempUnity tu
				 JOIN Location l ON tu.Origin = l.Name
				 WHERE tu.mainDriver = s.Id AND tu.Origin LIKE @R_origin AND tu.Destination NOT LIKE @R_dest   AND l.RegionId = @R_regionID_dest) as 'AmountOfRides_OriginToArea',
			(CASE WHEN @R_isAfterNoon = 1 
              THEN (SELECT COUNT(*) FROM #tempUnity WHERE s.Id = mainDriver AND DATEPART(HOUR, pickupTime) >= 15 ) 
              ELSE (SELECT COUNT(*) FROM #tempUnity WHERE s.Id = mainDriver AND DATEPART(HOUR, pickupTime) < 15  ) END) as 'AmountOfRidesAtThisTime',
			 (SELECT COUNT(*) 
			 FROM #tempUnity 
			 WHERE DATENAME(WEEKDAY, @R_time) = DATENAME(WEEKDAY, pickupTime) AND MainDriver = s.Id ) as 'AmountOfRidesAtThisDayWeek',
			 (SELECT (SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = s.Id).STDistance(@Start_point) / 1000 + 
             (@Start_point.STDistance(@End_point) / 1000) + 
             (@End_point.STDistance((SELECT VolunteerCityPoint FROM VolunteerCityPoints WHERE Id = s.Id)) / 1000)) as 'SumOfKM'
	FROM #super s

),


newbis_25_Temp AS (
    SELECT TOP 25 *
    FROM newbisCTE
    ORDER BY SumOfKM
),
reg_25_Temp AS (
    SELECT TOP 25 *
    FROM RegularCTE
    ORDER BY AmountOfRidesInThisPath DESC
),
super_25_Temp AS (
    SELECT TOP 25 *
    FROM SuperCTE
    ORDER BY AmountOfRidesInThisPath DESC
)

SELECT Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides, Vtype,
       SeniorityInYears, LastCallDateTime, LastRideInDays, NextRideInDays,
       AmountOfRidesFromRegionToDest, NumOfRidesLast2Month, AmountOfRidesInThisPath,
       AmountOfRidesInOppositePath, AmountOfRides_OriginToArea, AmountOfRidesAtThisTime,
       AmountOfRidesAtThisDayWeek, SumOfKM,(select  count(*) from DocumentedCall where DriverId = Id) as 'NoOfDocumentedCalls'
FROM newbis_25_Temp
UNION ALL
SELECT Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides, Vtype,
       SeniorityInYears, LastCallDateTime, LastRideInDays, NextRideInDays,
       AmountOfRidesFromRegionToDest, NumOfRidesLast2Month, AmountOfRidesInThisPath,
       AmountOfRidesInOppositePath, AmountOfRides_OriginToArea, AmountOfRidesAtThisTime,
       AmountOfRidesAtThisDayWeek, SumOfKM,(select  count(*) from DocumentedCall where DriverId = Id) as 'NoOfDocumentedCalls'
FROM reg_25_Temp
UNION ALL
SELECT Id, DisplayName, CellPhone, JoinDate, CityCityName, AvailableSeats, NoOfDocumentedRides, Vtype,
       SeniorityInYears, LastCallDateTime, LastRideInDays, NextRideInDays,
       AmountOfRidesFromRegionToDest, NumOfRidesLast2Month, AmountOfRidesInThisPath,
       AmountOfRidesInOppositePath, AmountOfRides_OriginToArea, AmountOfRidesAtThisTime,
       AmountOfRidesAtThisDayWeek, SumOfKM,(select  count(*) from DocumentedCall where DriverId = Id) as 'NoOfDocumentedCalls'
FROM super_25_Temp;





	--drop tables
	drop table #ShortenDriversTable,#newbis,#regular,#super,#tempUnity
END




---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------