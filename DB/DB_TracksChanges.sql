/****** Object:  StoredProcedure [dbo].[spUpdateRideInUnityRide]    Script Date: 30/06/2024 13:34:13 ******/
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
	IsNewDriver = @isNewDriver,
	AmountOfEquipments = @AmountOfEquipments,
	status = case when @driverId is null then N'ממתינה לשיבוץ' else N'שובץ נהג' end
	WHERE RidePatNum=@unityRideId;
	
	ELSE 
	return -1

END
