/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

ALTER TABLE City
ADD isMain bool

ALTER TABLE City
ADD mainCity nvarchar(255)

update City set ismain = 1 where CityName = N'אשקלון'
update City set ismain = 1 where CityName = N'באר שבע'
update City set ismain = 1 where CityName = N'חדרה'
update City set ismain = 1 where CityName = N'חיפה'
update City set ismain = 1 where CityName = N'ירושלים'
update City set ismain = 1 where CityName = N'כפר סבא'
update City set ismain = 1 where CityName = N'מודיעין-מכבים-רעות'
update City set ismain = 1 where CityName = N'נתניה'
update City set ismain = 1 where CityName = N'קרית גת'
update City set ismain = 1 where CityName = N'רחובות'
update City set ismain = 1 where CityName = N'תל אביב - יפו'


-- =============================================
-- Author:      Benny Bornfeld
-- Create Date: 09/12/2022
-- Description: Get All Cities From City Table that has no lat lng
-- =============================================
CREATE PROCEDURE [dbo].[spCity_GetAllUnmappedCities]
AS
BEGIN
    SET NOCOUNT ON
	SELECT * FROM City
	WHERE (lat is null or lng is null)
END
GO


-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Benny>
-- Create Date: <15-12-2022>
-- Description: <update the main city field in city>
-- =============================================
CREATE PROCEDURE spUpdateNearestMainCity
(
    -- Add the parameters for the stored procedure here
@mainCity nvarchar(255),
@cityName nvarchar(255)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    --SET NOCOUNT ON

    -- Insert statements for procedure here
    UPDATE City SET mainCity = @mainCity WHERE cityName = @cityName
END
GO

/****** Object:  StoredProcedure [dbo].[spCity_GetAllCitiesWithLocations]    Script Date: 12/15/2022 5:02:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:      Benny Bornfeld
-- Create Date: 15/12/2022
-- Description: Get All Cities From City Table that has  lat lng
-- =============================================
CREATE PROCEDURE [dbo].[spCity_GetAllCitiesWithLocations]
AS
BEGIN
    SET NOCOUNT ON
	SELECT * FROM City
	WHERE (lat is not null or lng is not null)
END
GO


