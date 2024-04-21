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
CREATE PROCEDURE spGetReturnRide_UnityRide
(
    -- Add the parameters for the stored procedure here
    -- the originals parametes without switch!!! 
	@origin nvarchar(55),
	@dest nvarchar(55),
	@patinetName nvarchar(55),
	@pickupTime datetime
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    select * from unityRide where destination like @origin
	and origin like @dest
	and CONVERT(date, pickupTime) like CONVERT(date, @pickupTime)
	and patientName like @patinetName
END
GO
