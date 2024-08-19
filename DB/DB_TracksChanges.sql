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





/****** Object:  StoredProcedure [dbo].[spVolunteerTypeView_GetVolunteersList_Gilad]    Script Date: 07/07/2024 12:49:18 ******/
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

--Update the NoOfDocumentedRides in Volunteer Table !!!! 




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



/****** Object:  StoredProcedure [dbo].[spVolunteerTypeView_GetVolunteersList_Gilad]    Script Date: 15/07/2024 16:37:26 ******/
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
				vtv.NoOfDocumentedRides,vtv.JoinDate,vtv.isAssistant,vtv.IsActive,
				vtv.KnowsArabic,vtv.Gender,vtv.pnRegId,vtv.EnglishName,vtv.LastModified,vtv.isDriving,
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
				select origin + '-'+destination from
													(
														select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove
														FROM #tempNotDeletedOnly t
														where t.MainDriver = vtv.Id AND t.pickupTime>=DATEADD(MONTH, -6, GETDATE())
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
				select origin + '-'+destination from
													(
														select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
														where t.MainDriver = vtv.Id AND t.pickupTime>=DATEADD(MONTH, -6, GETDATE())
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
/****** Object:  StoredProcedure [dbo].[spUnityRide_UpdateDateAndTime]    Script Date: 21/07/2024 21:51:03 ******/
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