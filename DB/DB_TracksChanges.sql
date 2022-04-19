/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

/* ↓ NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD ↓ */

/*---------------------------------------------------------*/
/*---------------------------------------------------------*/

-- =============================================
-- Author:      Yogev Strauber
-- Create Date: 08/04/2022
-- Description: Gets volunteer data by his cellphone number
-- =============================================
CREATE PROCEDURE spVolunteer_GetVolunteerByCellphone
(
	@cellphone NVARCHAR(20)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	SELECT * FROM Volunteer WHERE CellPhone=@cellphone
END
GO
