/****** Object:  StoredProcedure [dbo].[spUpdateRideInUnityRide]    Script Date: 12/12/2023 12:35:18 ******/
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
	DECLARE @NoOfDocumentedRides int = (select NoOfDocumentedRides from volunteer where id= @driverId)
	DECLARE @isNewDriver bit = case when (select count(*) from UnityRide where pickupTime<=GETDATE() and MainDriver=@driverId)<=3 then 1 else 0 end

	DECLARE @AmountOfEquipments int =0
	SET @AmountOfEquipments  = (select count(PatientId) from Equipment_Patient where PatientId = @patientId group by PatientId)
	set @driverName = case when @driverName='' then null else @driverName end

	DECLARE @FlagVar int =-1
	select @FlagVar= RidepatNum
	from unityRide
	where PatientName like @patientName and pickupTime = @pickupTime and Origin like @origin and Destination like @destination
	if (@FlagVar = -1 or @FlagVar=@unityRideId)
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
	IsNewDriver = @isNewDriver,
	AmountOfEquipments = @AmountOfEquipments
	WHERE RidePatNum=@unityRideId;

END









----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spSetNewUnityRide]    Script Date: 18/12/2023 10:52:05 ******/
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
DECLARE @driverId int = case when @driverName is null then null else (select id from volunteer where displayName like @driverName) end
DECLARE @driverPhone varchar(11) = (select cellphone from volunteer where id= @driverId)
DECLARE @NoOfDocumentedRides int = (select NoOfDocumentedRides from volunteer where id= @driverId)
DECLARE @isNewDriver bit = case when (select count(*) from UnityRide where pickupTime<=GETDATE() and MainDriver=@driverId)<=3 then 1 else 0 end

DECLARE @AmountOfEquipments int = 0
SET @AmountOfEquipments  = case when(select count(PatientId) from Equipment_Patient where PatientId = @patientId group by PatientId) is null then 0 ELSE 
(select count(PatientId) from Equipment_Patient where PatientId = @patientId group by PatientId)
end


DECLARE @FlagVar int =-1
select @FlagVar= RidepatNum
from unityRide
where PatientName like @patientName and pickupTime = @pickupTime and Origin like @origin and Destination like @destination
if @FlagVar = -1
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
IsNewDriver
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
@isNewDriver
)
Select SCOPE_IDENTITY() 'RidePatNum'
END
else
select -1 'RidePatNum'

END


----------------------------------------------------------------------------------------------------------------------------------------------------------------------------



/****** Object:  StoredProcedure [dbo].[spUnityRide_UpdateDateAndTime]    Script Date: 20/12/2023 13:57:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Gilad
-- Create Date: 08/12/23
-- Description: to update the time of spesific ride in unityride
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







----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <07/12/23>
-- Description: <to get all the escorts of spesific patient by his id>
-- =============================================
CREATE PROCEDURE spGetEscortsByPatientId
(
    -- Add the parameters for the stored procedure here
	@patientId int 
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    select E.Id,E.DisplayName 
	from patientEscort PE inner join Escorted E on PE.EscortId = E.Id
	where patientId = @patientId
END
GO







----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <06/12/23>
-- Description: <To get the Escorts name and id for spesific Ride>
-- =============================================
CREATE PROCEDURE spGetEscortByUnityRideId
(
    -- Add the parameters for the stored procedure here
   @UnityRideId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    
select Id,DisplayName
from [PatientEscort_PatientInRide (RidePat)] PER inner join 
	Escorted E on E.id = PER.PatientEscortEscortId
where [PatientInRide (RidePat)RidePatNum] = @UnityRideId
END
GO







----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <6/12/23>
-- Description: <for ridePat Edit form need to get one ride according id>
-- =============================================
CREATE PROCEDURE GetUnityRide
(
    -- Add the parameters for the stored procedure here
	@UnityRideId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
select * ,
(select EnglishName from patient where id = UR.PatientId) 'PatientEnglishName',
(select PatientIdentity from patient where id =  UR.PatientId) 'PatientIdentity',
(select DisplayNameA from patient where id =  UR.PatientId) 'DisplayNameArabic'
from unityRide UR
where UR.RidePatNum = @UnityRideId
END
GO










----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad >
-- Create Date: <03/10/2023>
-- Description: <Because of the United Tables try to Re-orgenaize this query>
-- =============================================
CREATE PROCEDURE spGetUnitedRides
(
    -- Add the parameters for the stored procedure here
	@days int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
select * from UnityRide where DATEDIFF(day,getdate(),pickuptime)<=@days and Convert(date,pickuptime)>=CONVERT(date, getdate()) and Status <> N'נמחקה'; 
END
GO




----------------------------------------------------------------------------------------------------------------------------------------------------------------------------





-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <04/10/2023>
-- Description: <To seperate the dataset in getRidePatView need to perform this query to know which Equipments need spesific patient>
-- =============================================
CREATE PROCEDURE spGetEquipmentsForSpesificPatient
(
    -- Add the parameters for the stored procedure here
  @patientId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
	select EquipmentName
	from EquipmentForPatientView
	where id = @patientId
END
GO



-------------------------------------------------------------------------------CREATE TABLE-------------------------------------------------------------------------------


	Create Table UnityRide (
	RidePatNum int identity(1,1) primary KEY,
	PatientName nvarchar(255),
	PatientCellPhone varchar(50),
	PatientId int,
	PatientGender nvarchar(50),
	PatientStatus nvarchar(55),
	patientStatusTime datetime,
	PatientBirthDate date,
	AmountOfEscorts int,
	AmountOfEquipments int,
	Origin nvarchar(255),
	Destination nvarchar(255),
	pickupTime DateTime,
	Coordinator nvarchar(255),
	Remark nvarchar(255),
	Status nvarchar(50) DEFAULT N'ממתינה לשיבוץ',
	Area nvarchar(50),
	Shift nvarchar(50),
	OnlyEscort bit,
	lastModified datetime,
	CoordinatorID int,
	MainDriver int,
	DriverName nvarchar (255),
	DriverCellPhone varchar(11),
	NoOfDocumentedRides int,
	IsAnonymous bit,
	IsNewDriver bit,
    FOREIGN KEY (CoordinatorID) REFERENCES volunteer(Id),
	FOREIGN KEY (MainDriver) REFERENCES volunteer(Id),
	FOREIGN KEY (PatientName) REFERENCES patient(DisplayName),
	FOREIGN KEY (PatientId) REFERENCES patient(Id),
)



SET IDENTITY_INSERT UnityRide ON;
INSERT INTO UnityRide
(RidePatNum,
PatientName,
PatientId,
PatientGender,
PatientCellPhone,
PatientStatus,
patientStatusTime,
PatientBirthDate,
AmountOfEscorts,
AmountOfEquipments,
Origin,
Destination,
pickupTime,
Coordinator,
CoordinatorID,
Remark,
Status,
Area,
Shift,
OnlyEscort,
lastModified,
MainDriver,
DriverName,
DriverCellPhone,
NoOfDocumentedRides,
IsAnonymous,
IsNewDriver)

select       
			  rp.RidePatNum,
			  rp.DisplayName ,
			  rp.Id as 'patientID',
			  rp.Gender,
			  CASE when rp.IsAnonymous=1 then 'No CellPhone' ELSE rp.CellPhone END as 'PatientCellPhone' ,
			  rp.PatientStatus,
			  rp.editTimeStamp,
			  rp.BirthDate,
			  (select count(*) from ridepatEscortView where ridepatNum = rp.RidePatNum) as 'AmountOfEscorts',
			  (select count(id) from EquipmentForPatientView where id = rp.Id) as 'AmountOfEquipments',
			  rp.Origin ,
			  rp.Destination ,
			  rp.pickupTime, 
			  rp.Coordinator ,
			  (select id from volunteer where DisplayName like rp.Coordinator) as 'CoorID',
			  rp.Remark,
			  rp.Status ,
			  rp.Area ,
			  rp.Shift,
			  rp.OnlyEscort,
			  rp.lastModified,
			  rp.MainDriver ,
			  (select displayName from volunteer where id=rp.MainDriver) as 'DriverName',
			  (select CellPhone from volunteer where id=rp.MainDriver) as 'DriverCellPhone',
			  rp.NoOfDocumentedRides ,
			  rp.IsAnonymous,
			  rp.IsNewDriver
from  rpview rp
where rp.rideNum is not null
union
select 
              rp.RidePatNum,
			  rp.DisplayName ,
			  rp.Id as 'patientID',
			  rp.Gender,
			  CASE when rp.IsAnonymous=1 then 'No CellPhone' ELSE rp.CellPhone END as 'PatientCellPhone' ,
			  rp.PatientStatus,
			  rp.editTimeStamp,
			  rp.BirthDate,
			  (select count(*) from ridepatEscortView where ridepatNum = rp.RidePatNum) as 'AmountOfEscorts',
			  (select count(id) from EquipmentForPatientView where id = rp.Id) as 'AmountOfEquipments',
			  rp.Origin ,
			  rp.Destination ,
			  rp.pickupTime, 
			  rp.Coordinator ,
			  (select id from volunteer where DisplayName like rp.Coordinator) as 'CoorID',
			  rp.Remark,
			  rp.Status ,
			  rp.Area ,
			  rp.Shift,
			  rp.OnlyEscort,
			  rp.lastModified,
			  rp.MainDriver ,
			  (select displayName from volunteer where id=rp.MainDriver) as 'DriverName',
			  (select CellPhone from volunteer where id=rp.MainDriver) as 'DriverCellPhone',
			  rp.NoOfDocumentedRides ,
			  rp.IsAnonymous,
			  rp.IsNewDriver
from rpview rp
where rp.rideNum is null and status like N'ממתינה לשיבוץ' and (CAST(GETDATE() AS DATE))<=rp.pickupTime
SET IDENTITY_INSERT UnityRide OFF;



























