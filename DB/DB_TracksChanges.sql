/****** Object:  StoredProcedure [dbo].[spVolunteerTypeView_GetVolunteersList_Gilad]    Script Date: 05/07/2024 10:19:57 ******/
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

select r.MainDriver, r.Origin, r.Destination
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
				vtv.NoOfDocumentedRides,vtv.JoinDate,vtv.isAssistant,vtv.IsActive,
				vtv.KnowsArabic,vtv.Gender,vtv.pnRegId,vtv.EnglishName,vtv.LastModified,vtv.isDriving,
				(select count(*)
					from UnityRide
					where maindriver = vtv.Id
					and pickuptime between DATEADD(Month, -2, GETDATE()) and  GETDATE()) as NumOfRides_last2Months
					--gilad addition vvv
					,(
					select abse.AbsenceStatus
					from #tempAbsence abse
					where abse.VolunteerId=vtv.Id
					) as AbsenceStatus,
					--gilad addition ^^^
					(
				select origin + '-'+destination from
													(
														select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
														where t.MainDriver = vtv.Id
														group by maindriver, origin, destination
														order by numberOfTimesDrove desc
														) t
					) mostCommonPath, tld.latestDrive
		from VolunteerTypeView vtv
		left join #tempLatesetDrives tld on tld.Id=vtv.Id
		where IsActive = @IsActive or IsActive = 1
		order by firstNameH

	end
else
	begin
	select vtv.Id,vtv.DisplayName,vtv.FirstNameA,vtv.FirstNameH,
				vtv.LastNameH,vtv.LastNameA,vtv.CellPhone,vtv.CellPhone2,
				vtv.HomePhone,vtv.Remarks,vtv.CityCityName,vtv.Address,
				vtv.VolunTypeType,vtv.Email,vtv.device,vtv.NoOfDocumentedCalls,
				vtv.NoOfDocumentedRides,vtv.JoinDate,vtv.isAssistant,vtv.IsActive,
				vtv.KnowsArabic,vtv.Gender,vtv.pnRegId,vtv.EnglishName,vtv.LastModified,vtv.isDriving,
				(select count(*)
					from UnityRide
					where maindriver = vtv.Id
					and pickuptime between DATEADD(Month, -2, GETDATE()) and  GETDATE()) as NumOfRides_last2Months
					--gilad addition vvv
					,(
					select abse.AbsenceStatus
					from #tempAbsence abse
					where abse.VolunteerId=vtv.Id
					) as AbsenceStatus,
					--gilad addition ^^^
					(
				select origin + '-'+destination from
													(
														select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
														where t.MainDriver = vtv.Id
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