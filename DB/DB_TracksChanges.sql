













--------------------------------------------------------------------------TRIGGER--------------------------------------------------------------------------------------------------


/****** Object:  Trigger [dbo].[UnityRideTrigger_AFTER_UPDATE_LOGGER]    Script Date: 20/03/2024 18:22:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  CREATE TRIGGER [dbo].[UnityRideTrigger_AFTER_UPDATE_LOGGER] 
  ON [dbo].[UnityRide] FOR UPDATE AS
BEGIN
    SET NOCOUNT ON;

    -- Creating a temporary table to store updated columns
    CREATE TABLE #updatedCols (Id INT IDENTITY(1, 1), updateCol NVARCHAR(500));

    -- Finding all columns names that were updated and writing them to the temp table
    INSERT INTO #updatedCols (updateCol)     
    SELECT column_name     
    FROM information_schema.columns        
    WHERE table_name = 'UnityRide'
    AND convert(VARBINARY, REVERSE(columns_updated())) & POWER(convert(BIGINT, 2), ordinal_position - 1) > 0;

    -- Creating temporary tables to hold inserted and deleted rows
    SELECT TOP 1 * INTO #tempInserted FROM inserted ORDER BY RidePatNum;
    SELECT TOP 1 * INTO #tempDeleted FROM deleted ORDER BY RidePatNum;

    DECLARE @counter INT = 1;
    DECLARE @n INT;
    DECLARE @columnName NVARCHAR(255);
    DECLARE @query NVARCHAR(MAX);
    DECLARE @coordinator NVARCHAR(255);
    DECLARE @unityRideNum NVARCHAR(255);

    -- Extracting coordinator and UnityRideNum values from the inserted table
    SELECT @coordinator = Coordinator, @unityRideNum = CAST(RidePatNum AS NVARCHAR) FROM #tempInserted;

    -- Handling single quotes in coordinator value
    IF CHARINDEX('''', @coordinator) > 0 
    BEGIN
        SET @coordinator = REPLACE(@coordinator, '''', ''''''); -- Replacing single quote with two single quotes
    END

    -- Counting the number of updated columns
    SELECT @n = COUNT(*) FROM #updatedCols;

    -- Executing insert statement for each updated column    
    WHILE @counter <= @n    
    BEGIN
        SELECT @columnName = updateCol FROM #updatedCols WHERE Id = @counter;
        
        IF @columnName <> 'Coordinator' AND @columnName <> 'lastModified'
        BEGIN    
            SET @query = 'IF(ISNULL(CAST((SELECT d.' + QUOTENAME(@columnName) + ' FROM #tempDeleted d) AS NVARCHAR), ''(null)'')    
                            <>     
                            ISNULL(CAST((SELECT i.' + QUOTENAME(@columnName) + ' FROM #tempInserted i) AS NVARCHAR), ''(null)'')) 
                            BEGIN       
                                INSERT INTO LogTable_AutoTrackChanges (WhoChanged, TableName, Recorde_UniqueId, ColumnName, OldValue, NewValue)    
                                VALUES (N''' + @coordinator + ''', ''UnityRide'', ' + @unityRideNum + ', ''' + @columnName + ''',    
                                    CASE   
                                        WHEN ISNULL(CAST((SELECT d.' + QUOTENAME(@columnName) + ' FROM #tempDeleted d) AS NVARCHAR), ''(null)'') = ''?????? ??????''
                                            THEN ''(null)''  
                                        ELSE ISNULL(CAST((SELECT d.' + QUOTENAME(@columnName) + ' FROM #tempDeleted d) AS NVARCHAR), ''(null)'')      
                                    END,
                                    ISNULL(CAST((SELECT i.' + QUOTENAME(@columnName) + ' FROM #tempInserted i) AS NVARCHAR), ''(null)''))  
                            END'; 
            EXEC sp_executesql @query;
        END    
        
        SET @counter = @counter + 1;
    END;

    -- Dropping temporary tables
    DROP TABLE #updatedCols;
    DROP TABLE #tempInserted;
    DROP TABLE #tempDeleted;
END;





----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Gilad
-- Create Date: 01/04/2024
-- Description: this is for weekly thanks for volunteer like drivers and coordinators.
-- =============================================
CREATE PROCEDURE spGetWeeklyThanksVol
(
    -- Add the parameters for the stored procedure here
	@thisSunday dateTime,
	@endDate dateTime
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	

    -- Insert statements for procedure here
    select coordinatorID,mainDriver, DriverName,Coordinator
	from UnityRide 
	where pickuptime >= @thisSunday AND
	pickuptime < @endDate and
	Status <> N'נמחקה' and 
	status<> N'ממתינה לשיבוץ' and 
	mainDriver is not null
	group by coordinatorID,mainDriver, DriverName,Coordinator

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
-- Create Date: <12/03/2024>
-- Description: <before post absence for volunteer check if he\she got rides in this dates>
-- =============================================
CREATE PROCEDURE spCheckFutureRideBeforeAbsence
(
    -- Add the parameters for the stored procedure here
    @volunteerId int,
	@startTime DateTime,
	@endTime DateTime

)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
	DECLARE @rideNumber INT;

	select top 1 @rideNumber = RidePatNum
	from UnityRide
	WHERE mainDriver = @volunteerID
	AND pickupTime between  @startTime and @endTime

	 IF (@rideNumber IS NOT NULL)
        SELECT @rideNumber AS RidePatNum;
    ELSE
        SELECT -1 AS RidePatNum;

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
-- Author:     Gilad
-- Create Date: 11/03/2024
-- Description: to check if some driver has future rides - this is for warning in the client-side.
-- =============================================
CREATE PROCEDURE sp_VolunteerHasFutureRides
(
    -- Add the parameters for the stored procedure here
  @volunteerId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM unityRide 
            WHERE mainDriver = @volunteerId
            AND pickupTime >= CURRENT_TIMESTAMP
        ) THEN 1
        ELSE 0
    END AS hasFutureRides;




END
GO



----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[spDriverLeaveUnityRide]    Script Date: 23/01/2024 12:12:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <22/01/24>
-- Description: <driver want to leave a ride from mobile>
-- =============================================
CREATE PROCEDURE [dbo].[spDriverLeaveUnityRide]
(
    -- Add the parameters for the stored procedure here
   @driverId int,
   @UnityRideID int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
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
-- Create Date: <22/01/24>
-- Description: <Gets the equipment needed for future rides per driver after united>
-- =============================================
CREATE PROCEDURE spGetFutureEquipmentPerPatientPerDriver_unityRide
(
    -- Add the parameters for the stored procedure here
  @driverId int

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
	select rp.PatientName 
	from UnityRide rp 
	where rp.MainDriver = @driverId
	and rp.pickuptime >= GETDATE() and status <> N'נמחקה'   
	)
END
GO







----------------------------------------------------------------------------------------------------------------------------------------------------------------------------




/****** Object:  StoredProcedure [dbo].[spGetFutureUnityRidesOfVolunteer]    Script Date: 23/01/2024 12:03:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <22/01/24>
-- Description: <Get all the future ride of spesific volunteer after united>
-- =============================================
CREATE PROCEDURE [dbo].[spGetFutureUnityRidesOfVolunteer]
(
    -- Add the parameters for the stored procedure here
    @driverId int

)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT rp.ridepatnum, rp.PatientName as patient, rp.PatientCellPhone as Cellphone, rp.IsAnonymous,
	       rp.origin, rp.destination, rp.Area, rp.pickuptime, 1 as RideNum, rp.OnlyEscort,rp.AmountOfEscorts
	FROM UnityRide rp
    WHERE rp.maindriver = @driverId and rp.pickuptime >= GETDATE() AND status <> N'נמחקה' 
	order by rp.pickuptime desc
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
-- Create Date: <22/01/24>
-- Description: <to custom old sp after united get the equipment per patient to spesific driver>
-- =============================================
CREATE PROCEDURE spGetPastEquipmentPerPatientPerDriver_UnityRide
(
    -- Add the parameters for the stored procedure here
   @driverId int

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
	select rp.patientName as patient 
	from UnityRide rp 
	where rp.MainDriver = 13937
	and rp.pickuptime < GETDATE() and status <> N'נמחקה'   
	)
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
-- Create Date: <22/01/24>
-- Description: <Returns all the past rides of volunteer>
-- =============================================
CREATE PROCEDURE spGetPastRideOfVol_unityRide
(
    -- Add the parameters for the stored procedure here
   @driverId int

)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
		SELECT rp.ridepatnum, rp.patientName as patient, rp.PatientCellPhone as CellPhone , rp.IsAnonymous,
	       rp.origin, rp.destination, rp.pickuptime, rp.Area,rp.OnlyEscort, 1 as RideNum , rp.AmountOfEscorts
	FROM UnityRide rp
    WHERE rp.maindriver = @driverId and rp.pickuptime < GETDATE() AND status <> N'נמחקה' 
	order by rp.pickuptime desc
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
-- Create Date: <21/01/24>
-- Description: <get basic data on unityride to mobile service>
-- =============================================
CREATE PROCEDURE spGetBasicDataUnityRide
(
    @daysAhead int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
		SELECT rp.ridepatnum, rp.PatientName as patient, rp.PatientCellPhone as CellPhone, rp.IsAnonymous,
	       rp.Area, rp.origin, rp.destination, rp.pickuptime, rp.OnlyEscort ,rp.AmountOfEscorts
		FROM UnityRide rp
		WHERE rp.pickuptime > GETDATE() AND status = N'ממתינה לשיבוץ' AND rp.pickuptime < DATEADD(day, @daysAhead, GETDATE())  
		order by rp.pickuptime asc
END
GO







----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[spAssignDriverMobile]    Script Date: 23/01/2024 11:14:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <21/01/24>
-- Description: assign Driver to unityRide Table for mobile service
-- =============================================
CREATE PROCEDURE [dbo].[spAssignDriverMobile]
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

GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------





/****** Object:  StoredProcedure [dbo].[spVolunteerTypeView_GetVolunteersList_Gilad]    Script Date: 18/01/2024 16:23:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:       Gilad
-- ALTER Date:	 16/08/2023
-- ALTER Reason: try to add addition of absence to this sp 
-- =============================================
ALter procedure [dbo].[spVolunteerTypeView_GetVolunteersList_Gilad]

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
into #tempNotDeletedOnly from  ridepat rp
inner join ride r
on r.RideNum=rp.RideId

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

GO











----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spUnityRide_GetVolunteersRideHistory]    Script Date: 15/01/2024 16:30:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <15/01/24>
-- Description: <to get the rides of spesific volunteer for viewvolunteer.html (car icon) documentedRides>
-- =============================================
CREATE PROCEDURE [dbo].[spUnityRide_GetVolunteersRideHistory]
(
    -- Add the parameters for the stored procedure here
  @volunteerId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
	select ridepatNum , patientName,Remark,origin,destination,pickupTime
	from UnityRide
	where mainDriver = @volunteerId and status not like N'נמחקה'
	order by pickupTime desc

END
GO
















----------------------------------------------------------------------------------------------------------------------------------------------------------------------------




/****** Object:  StoredProcedure [dbo].[spRecoverUnityRide]    Script Date: 15/01/2024 15:21:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <02/01/24>
-- Description: <To recover a ride with status deleted>
-- =============================================
CREATE PROCEDURE [dbo].[spRecoverUnityRide]
(
    -- Add the parameters for the stored procedure here
		@unityRideID INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
		DECLARE @driverID INT = (select MainDriver from UnityRide where RidePatNum =  @unityRideID)

		IF(@driverID is null)
		Update UnityRide
		SET Status  = N'ממתינה לשיבוץ',
		lastModified = GETDATE()
		where RidePatNum = @unityRideID

		ELSE
		begin
		Update UnityRide
		SET Status  = N'שובץ נהג',
		lastModified = GETDATE()
		where RidePatNum = @unityRideID
		Update Volunteer
		SET NoOfDocumentedRides = NoOfDocumentedRides + 1
		Where Id = @driverID
		end


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
-- Create Date: <01/01/24>
-- Description: <because of the united tables need to get this data for viewRidePats.html >
-- =============================================
CREATE PROCEDURE spGet_UnityRide_ByTimeRange
(
    -- Add the parameters for the stored procedure here
		@from int,
		@to int,
		@isDeletedtoShow bit
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
 	if @isDeletedtoShow = 1
		begin
	select *
	from UnityRide  
	where PickupTime between
	CAST( GETDATE()-@from AS Date ) and
	CAST( GETDATE()+@to AS Date ) and status  like N'נמחקה'
		end
	else
		begin
	select *
	from UnityRide  
	where PickupTime between
	CAST( GETDATE()-@from AS Date ) and
	CAST( GETDATE()+@to AS Date ) and status not like N'נמחקה'
		end
END
GO




----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spDeleteUnityRide]    Script Date: 15/01/2024 16:28:29 ******/
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
CREATE PROCEDURE [dbo].[spDeleteUnityRide]
(
    -- Add the parameters for the stored procedure here
   @unityRideID INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
		DECLARE @DriverId INT = (select MainDriver from unityRide where ridepatnum = @unityRideID)
		DECLARE @isAnonymous bit = (select IsAnonymous from UnityRide where ridepatnum = @unityRideID)

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
		set Status = N'נמחקה', lastModified = GETDATE()
		where ridepatnum = @unityRideID
		--return the update ride
		Select * from UnityRide where  ridepatnum = @unityRideID
		END



END
GO





----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[spCheckValidDrive]    Script Date: 02/01/2024 14:38:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <27/12/23>
-- Description: <To check if is there any ride with same time but diffrent places for same driver >
-- =============================================
CREATE PROCEDURE [dbo].[spCheckValidDrive]
(
    -- Add the parameters for the stored procedure here
	@UnityRideID INT,
	@DriverName nvarchar(255),
	@pickupTime dateTime
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	Declare @driverId INT = (select Id from Volunteer where DisplayName like @DriverName)
    -- Insert statements for procedure here
	IF EXISTS(select 1
	from UnityRide
	where RidePatNum != @UnityRideID and
		  pickupTime = @pickupTime and 
		  MainDriver = @driverId)
	select 1 as 'res'
	ELSE
	select 0 as 'res'
END
GO




----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spUpdateDriverUnityRide]    Script Date: 17/01/2024 14:58:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <25/12/23>
-- Description: <update a driver to spesific unity ride also to delete driver from ride>
-- =============================================
CREATE PROCEDURE [dbo].[spUpdateDriverUnityRide]
(
    -- Add the parameters for the stored procedure here
	@driverID int,
	@unityRideID int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

    -- Insert statements for procedure here
	DECLARE @driverName nvarchar(255) = (select displayname from volunteer where Id=@driverID)
	DECLARE @driverPhone varchar(11) = (select cellphone from volunteer where id= @driverID)
	DECLARE @isNewDriver bit = case when (select count(*) from UnityRide where pickupTime<=GETDATE() and MainDriver=@driverID)<=3 then 1 else 0 end
	DECLARE @oldDriver int = (select MainDriver from UnityRide where RidePatNum = @unityRideID)

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
	UPDATE UnityRide
	set MainDriver = @driverID,
		DriverName = @driverName,
		DriverCellPhone = @driverPhone,
		NoOfDocumentedRides = @NoOfDocumentedRides,
		IsNewDriver = @isNewDriver,
		lastModified = GETDATE(),
		Status = N'שובץ נהג'
		
	where RidePatNum = @unityRideID
	
	-- for delete driver from unity ride
	ELSE 
		UPDATE UnityRide
	set MainDriver = NULL,
		DriverName = NULL,
		DriverCellPhone = NULL,
		NoOfDocumentedRides = NULL,
		IsNewDriver = 1,
		lastModified = GETDATE(),
		Status = N'ממתינה לשיבוץ'
	where RidePatNum = @unityRideID

select * from UnityRide where RidePatNum = @unityRideID
END
GO











----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[spUpdatePatientStatusUnityRide]    Script Date: 14/01/2024 13:25:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <24/12/23>
-- Description: <for update the status of patiet in a ride update in 2 tables because of the unity>
-- =============================================
CREATE PROCEDURE [dbo].[spUpdatePatientStatusUnityRide]
(
    -- Add the parameters for the stored procedure here
	@PatientId INT,
	@RidePatNum INT,
	@PatientStatus nvarchar(55),
	@EditTimeStamp datetime
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

		UPDATE UnityRide
		SET LastModified=GETDATE(),PatientStatus=@PatientStatus,patientStatusTime=@EditTimeStamp
		WHERE RidePatNum=@RidePatNum

		select * 
		from UnityRide
		where RidePatNum=@RidePatNum
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
-- Create Date: <24/12/23>
-- Description: <according to unity rides need to update remark in the same way like ridepat>
-- =============================================
CREATE PROCEDURE spUnityRide_updateRemark
(
    -- Add the parameters for the stored procedure here
	@ridePatNum int,
	@newRemark nvarchar(255)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	
    -- Insert statements for procedure here
	UPDATE UnityRide
	SET Remark=@newRemark
	WHERE RidePatNum=@ridePatNum

	if @@ROWCOUNT>0
	select * from UnityRide where RidePatNum=@ridePatNum
	else
	select -1 as 'RidePatNum'
	
END
GO













----------------------------------------------------------------------------------------------------------------------------------------------------------------------------




/****** Object:  StoredProcedure [dbo].[spUpdateRideInUnityRide]    Script Date: 17/01/2024 14:51:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <11/12/23>
-- Description: <this sp is for update spesific ride in the table (edit in the client)>
-- =============================================
CREATE PROCEDURE [dbo].[spUpdateRideInUnityRide]
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
	where PatientId = @patientId and pickupTime = @pickupTime and Origin like @origin and Destination like @destination and IsAnonymous = 0 and RidePatNum!=@unityRideId )

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

END
GO


----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[spSetNewUnityRide]    Script Date: 15/01/2024 17:06:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Gilad>
-- Create Date: <09/12/23>
-- Description: <this sp is for insert data to unity ride and [PatientEscort_PatientInRide (RidePat)] if need to. >
-- =============================================
CREATE PROCEDURE [dbo].[spSetNewUnityRide]
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

	IF(@driverId is not null)
	Update Volunteer
	SET NoOfDocumentedRides = NoOfDocumentedRides + 1
	where Id = @driverId


	DECLARE @FlagVar int =-1
	set @FlagVar= (select top 1 RidepatNum
	from unityRide
	where PatientId = @patientId and pickupTime = @pickupTime and Origin like @origin
	and Destination like @destination and IsAnonymous = 0 and Status not like N'נמחקה' )
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
GO





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
CREATE PROCEDURE [dbo].[spUnityRide_UpdateDateAndTime]
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





