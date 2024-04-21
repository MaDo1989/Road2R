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
