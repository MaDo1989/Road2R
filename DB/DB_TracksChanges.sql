﻿/*NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD*/

CREATE procedure [dbo].[spEscorted_GetEscortById]
@id int
as 
begin 
	SET NOCOUNT ON; 
	select * from Escorted where Id=@id
end 

CREATE procedure  [dbo].[spEscorted_ChangeLastUpdateBy]  
		@lastUpdateBy nvarchar(255),  
		@id int   
		as 
		begin  
			update Escorted  
			set LastUpdateBy=@lastUpdateBy 
			where Id=@id  
			where Id=@id  
		end    

ALTER TABLE Escorted  ADD LastUpdateBy nvarchar(255)
GO
/**/
 CREATE  trigger [dbo].[EscortedTrigger_AFTER_UPDATE_LOGGER]
   on [dbo].[Escorted] for update  as 
   BEGIN 
		SET NOCOUNT ON; 
		create table #updatedCols (Id int identity(1, 1), updateCol nvarchar(500)) 
		--find all columns names that were updated and write them to temp table  
		
		insert into #updatedCols (updateCol)   
		select   
		column_name    
		from     
		information_schema.columns    
		where         
		table_name = 'Escorted'  
		and convert(varbinary, reverse(columns_updated())) & power(convert(bigint, 2), ordinal_position - 1) > 0    
		
		--temp tables are used because inserted and deleted tables are not available in dynamic SQL
			select top 1 * into #tempInserted from inserted order by Id
			select top 1 * into #tempDeleted from deleted order by Id  
			 
		declare @counter int = 1     
		declare @n int       
		declare @columnName nvarchar(255)    
		declare @query nvarchar(1000)      
		declare @lastUpdateBy nvarchar(255) set @lastUpdateBy = (select LastUpdateBy from #tempInserted); 
		
		if CHARINDEX('''',@lastUpdateBy) > 0 
			begin
				set @lastUpdateBy =
				SUBSTRING(@lastUpdateBy,1, charindex('''', @lastUpdateBy) - 1)
				+ '''''' + -- this is wil converts into two '
				SUBSTRING(@lastUpdateBy, charindex('''', @lastUpdateBy) + 1, len(@lastUpdateBy))
			end
		declare @Id nvarchar(255) set @Id = CAST((select Id from #tempInserted) AS NVARCHAR);    
		select @n = count(*) from #updatedCols  

	----execute insert statement for each updated column     
		while @counter <= @n    
			
			begin   
			select @columnName = updateCol from #updatedCols where id = @counter  
			if(@columnName <> 'LastUpdateBy')
				begin     
					set @query ='IF(isnull(CAST((select d.' + @columnName + ' from #tempDeleted d) AS NVARCHAR), ''(null)'')  
					<> 
					isnull(CAST((select i.' + @columnName + ' from #tempInserted i) AS NVARCHAR), ''(null)'')) 
						BEGIN     
							insert into LogTable_AutoTrackChanges (WhoChanged, TableName, Recorde_UniqueId, ColumnName, OldValue, Newvalue)     
							values (N''' + @lastUpdateBy + ''', ''Escorted'',' + @Id + ', ''' + @columnName + '''            , 
								CASE 
									WHEN isnull(CAST((select d.' + @columnName + ' from #tempDeleted d) AS NVARCHAR), ''(null)'') = ''?????? ??????'' 
										THEN ''(null)''  
									ELSE      
									isnull(CAST((select d.' + @columnName + ' from #tempDeleted d) AS NVARCHAR), ''(null)'')      
								END      
							, isnull(CAST((select i.' + @columnName + ' from #tempInserted i) AS NVARCHAR), ''(null)''))
						END';   
					exec sp_executesql @query  
				end 
				set @counter = @counter + 1 
			end  
		END  
	--© YOGEV ©-- 
	-------------  
GO

--remove nulls from the column "lastupdateby"
    update  
	Patient --Escorted --Volunteer 
	set LastUpdateBy = N'אדמין מערכת' 
	where id in   (  
	select id from RidePat
	--Patient --Escorted --Volunteer   
	where LastUpdateBy is null   )
	GO

	    create procedure  spEscorted_ChangeLastUpdateBy  
		@lastUpdateBy nvarchar(255),  
		@id int   
		as 
		begin  
			update Escorted  
			set LastUpdateBy=@lastUpdateBy 
			where Id=@id  
		end    
GO

CREATE procedure [dbo].[spVolunteer_GetActiveVolunteers_NotDriversYet] 
@daysSinceJoin int
as
	BEGIN
	select * from volunteer v
	where 
		(
			CASE
				WHEN JoinDate is not null
					THEN DATEDIFF(DAY, JoinDate, getdate())
			end
		) > @daysSinceJoin
	and	IsActive=1
	and	not exists (select distinct MainDriver from ride where MainDriver is not null and maindriver=v.id)
END

CREATE TABLE Region
(
	Id int primary key identity(1,1),
	RegionName nvarchar(255) not null
)
GO

INSERT INTO Region (RegionName)
values
( N'עזה'),
( N'תרקומיא'),
( N'אשדוד'),
( N'רחובות'),
( N'תל אביב'),
( N'מודיעין'),
( N'ירושלים - מערב'),
( N'ירושלים - מזרח'),
( N'בית לחם'),
( N'מעלה אפריים'),
( N'אריאל'),
( N'טייבה'),
( N'חדרה'),
( N'חיפה'),
( N'ג''למה'),
( N'נהריה'),
( N'בית שאן'),
( N'באר שבע')
GO

/****** Object:  StoredProcedure [dbo].[spVolunteerTypeView_GetVolunteersList]    Script Date: 12/11/2021 7:10:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[spVolunteerTypeView_GetVolunteersList]

@IsActive bit
as
begin
select r.MainDriver, r.Origin, r.Destination into #tempNotDeletedOnly from  ridepat rp
inner join ride r
on r.RideNum=rp.RideId

if (@IsActive = 0)
	begin
				select *, (select count(*)
					from ridepat rp inner join ride r
					on rp.rideid=r.ridenum
					where r.maindriver = vtv.Id
					and pickuptime between DATEADD(Month, -2, GETDATE()) and  GETDATE()) as NumOfRides_last2Months
					,
					(
				select origin + '-'+destination from
					(
					select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
					where t.MainDriver = id
					group by maindriver, origin, destination
					order by numberOfTimesDrove desc
					) t
				) mostCommonPath
		from VolunteerTypeView vtv
		where IsActive = @IsActive or IsActive = 1
		order by firstNameH

	end
else
	begin
		select *, (select count(*)
					from ridepat rp inner join ride r
					on rp.rideid=r.ridenum
					where r.maindriver = vtv.Id
					and pickuptime between DATEADD(Month, -2, GETDATE()) and  GETDATE()) as NumOfRides_last2Months
					,
					(
				select origin + '-'+destination from
					(
					select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM #tempNotDeletedOnly t
					where t.MainDriver = id
					group by maindriver, origin, destination
					order by numberOfTimesDrove desc
					) t
				) mostCommonPath
		from VolunteerTypeView vtv
		where IsActive = @IsActive
		order by firstNameH
	end

	drop table #tempNotDeletedOnly 

end
GO

ALTER trigger [dbo].[RidePatRideTrigger]
on [dbo].[RidePat]for update,insert
as

begin
	declare @RPid int = (select Top 1 RidePatNum from inserted)
	declare @time datetime = CURRENT_TIMESTAMP
	declare @Rid int
		if (select top 1 RideId from inserted) is not null --case there is a driver
			set @Rid = (select top 1 RideId from inserted)
		else
			set @Rid = (select top 1 RideId from RidePat where RidePatNum=@RPid) --Q - DOESNT IT ALWAYS BE NULL ??
-- © ↓YOGEV↓ ©--
		if (select RideID from RidePat where RidePatNum=@RPid) is not null --case there is a driver 
			begin
				if (select Status from RidePat where RidePatNum=@RPid) = N'נמחקה'
					begin --change ridepat status when there is a driver  
				
						--step 1 check if need to update driver (case drive is in the future)
						--if yes ==> fetch the maindriver (aka volenteer ID) & Coordinator
						if (select PickupTime from RidePat where RideId = @Rid) > @time
							begin
							SELECT r.MainDriver, v.DisplayName, v.pnRegId--, rp.CoordinatorID
							from ride r inner join volunteer v on r.MainDriver = v.Id 
							where r.RideNum = @Rid
							end 

						--step 2 decrease NoOfDocumentedRides
						update volunteer set NoOfDocumentedRides -= 1 where Id = (select MainDriver from Ride where RideNum = @Rid)

						--step 3 DEACTIVATE RidePat FK TO RIDE
						update RidePat set RideId = null where RidePatNum = @RPid
								
						--step 4 DEACTIVATE RidePat FK TO status_Ride
						delete status_Ride where RideRideNum = @Rid
									
						--step 5 delete ride (can not be done with out the first two steps)
						--delete ride where RideNum = @Rid

						--FINALLY can change ridepat status 
						update RidePat set status = N'נמחקה' where RidePatNum = @RPid
						--IN RIDE update TRIGER THE STATUS CHANGING TO SOMETHING ELSE AND THAT IS THE REASON NEEDED TO HANDLE THE CHANGE HERE AGAIN
					end
-- © ↑YOGEV↑ ©--
-- ALON: כרגע ביטלנו (מהקליינט) את האפשרות שיהיה סטטוס 'אין נסיעת הלוך' ולכן השורות שמתחת להערה זו לא יכנסו לפעולה באף סיטואציה.
				else if (select Status from RidePat where RidePatNum=@RPid) = N'אין נסיעת הלוך'
					begin
						update RidePat set Status=N'אין נסיעת הלוך ויש נהג משובץ' where RidePatNum=@RPid
					end
				else
					begin
						update RidePat set Status=(select TOP 1 statusStatusName from status_Ride where RideRideNum=@Rid order by [Timestamp] desc) where RideId=@Rid
					end
			end
-- © ↓YOGEV↓ ©--
		else if (select Status from RidePat where RidePatNum=@RPid) = N'נמחקה'
			begin
				update RidePat set Status=N'נמחקה' where RidePatNum=@RPid
			end
--IN RIDE update TRIGER THE STATUS CHANGING TO SOMETHING ELSE AND THAT IS THE REASON NEEDED TO HANDLE THE CHANGE HERE AGAIN
-- © ↑YOGEV↑ ©--
		else if (select Status from RidePat where RidePatNum=@RPid) <> N'אין נסיעת הלוך' 
			begin
				update RidePat set Status=N'ממתינה לשיבוץ' where RidePatNum=@RPid
			end
end

CREATE procedure 
[dbo].[spEscorted_ToggleIsActive]

@isActive bit, 
@id int 
as 
begin   
UPDATE Escorted
SET IsActive=@isActive 
WHERE Id=@id
end 

/****** Object:  StoredProcedure [dbo].[spGetRideCandidates]    Script Date: 12/3/2021 12:11:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Benny Bornfeld>
-- Create Date: <5-sep-2021 >
-- Description: <get candidates for rides >
-- =============================================
CREATE PROCEDURE [dbo].[spGetRideCandidates]
(
	@RidePatNum AS INT
    -- Add the parameters for the stored procedure here
    --<@Param1, sysname, @p1> <Datatype_For_Param1, , int> = <Default_Value_For_Param1, , 0>,
    --<@Param2, sysname, @p2> <Datatype_For_Param2, , int> = <Default_Value_For_Param2, , 0>
)
AS
BEGIN
DECLARE @lookBackDaysPeriod AS INT = 180
DECLARE @searchTime AS DATETIME = GETDATE()
DECLARE @noOfferDaysWindow AS INT = 10
DECLARE @superUserDrives AS INT = 15
--DECLARE @RidePatNum AS INT = 42106

DECLARE @origin AS NVARCHAR(50)
SELECT @origin = [origin]
FROM ridepat
WHERE RidePatNum = @RidePatNum
PRINT @origin

DECLARE @destination AS NVARCHAR(50)
SELECT @destination = [destination]
FROM ridepat
WHERE RidePatNum = @RidePatNum 
PRINT @destination

DECLARE @pickupDay AS char(10)
SELECT @pickupDay = datename(dw,[pickupTime])
FROM ridepat
WHERE RidePatNum = @RidePatNum
PRINT @pickupDay

declare @originSubArea AS INT
SELECT @originSubArea = [Remarks]
FROM Location
WHERE [Name] =  @origin
PRINT @originSubArea

declare @destinationSubArea AS INT
SELECT @destinationSubArea = [Remarks]
FROM Location
WHERE [Name] =  @destination
PRINT @destinationSubArea

--select Name from Location where remarks = @originSubArea and isActive = 1
--select Name from Location where remarks = @destinationSubArea and isActive = 1

select MainDriver, MAX(v.DisplayName) as DisplayName, 
				   MAX(v.cellPhone) as cellPhone,
				   MAX(v.cityCityName) as city,
				   MAX(v.joinYear) as joinYear,
                   MAX(pathMatch) as maxPathMatch,
				   MAX(dayMatch) as dayMatch,
				   MAX(superDriver) as superUser,
				   MIN(ABS(DATEDIFF(day, @searchTime, [Date]))) as closestRideInDays,
				   (MAX(pathMatch) + MAX(dayMatch)) as totalScore
 from
(select rideNum, MainDriver, Origin, Destination, [Date],
 CASE
    WHEN Origin = @origin and Destination = @destination THEN 3
	WHEN Destination = @destination and Origin <> @origin           and Origin in (select Name from Location where remarks = @originSubArea and isActive = 1) THEN 2
	WHEN Origin = @origin           and Destination <> @destination and Destination in (select Name from Location where remarks = @destinationSubArea and isActive = 1) THEN 2
	WHEN origin in (select Name from Location where remarks = @originSubArea and isActive = 1) and
	     destination in (select Name from Location where remarks = @destinationSubArea and isActive = 1) THEN 1
    ELSE 0
 END
as pathMatch,
 CASE
	 WHEN @pickupDay = datename(dw,[Date]) THEN 1
	 ELSE 0
 END
 as dayMatch,
 CASE
	WHEN MainDriver in	(select MainDriver from ride 
	     WHERE [Date] >= DATEADD(day,-@lookBackDaysPeriod, @searchTime) AND [Date] <= @searchTime
		 group by MainDriver having count(*) > @superUserDrives) THEN 1
	ELSE 0
END
as superDriver
from ride
WHERE [Date] >= DATEADD(day,-@lookBackDaysPeriod, @searchTime)
and MainDriver is not null) as x
join Volunteer v on v.Id = MainDriver
where v.IsActive = 1
group by MainDriver
having ( (MAX(pathMatch) > 1 or MAX(dayMatch) > 0) and MAX([Date]) < DATEADD(day,-@noOfferDaysWindow, @searchTime)) 
order by totalScore desc



--****************************************
--Get the number of Rides for each candidate in the last period
--****************************************

--DECLARE @lookBackDaysPeriod AS INT = 180
--DECLARE @searchTime AS DATETIME = GETDATE()
--DECLARE @noOfferDaysWindow AS INT = 10
--DECLARE @superUserDrives AS INT = 15

--select MainDriver, count(*) as cnt
--from ride 
--join volunteer v on v.id = MainDriver
--WHERE [Date] >= DATEADD(day,-@lookBackDaysPeriod, @searchTime) AND [Date] <= @searchTime
--and v.isActive = 1
--group by MainDriver
--order by cnt desc


END

--THE LAST PROGRAMMER WHO 

--if (select RideID from RidePat where RidePatNum=@RPid) is not null
	--update RidePat set Status=(select TOP 1 statusStatusName from status_Ride where RideRideNum=@Rid order by [Timestamp] desc) where RideId=@Rid
	--else update RidePat set Status=N'ממתינה לשיבוץ' where RidePatNum=@RPid
--end

--dont forget to back volunteer table
update volunteer 
	set NoOfDocumentedRides = (select count(*) from ride 
								where MainDriver=Id
								and exists (select * from ridepat where RideId=RideNum))

GO

--YOU HAVE TO DEACTIVATE [RideUpdateTrigger] BEFORE RUN THIS UPDATE!!!
select * into ride_backUp_PUTDATEHERE from ride
GO

update ride
set
Origin	    = 
(
isnull((select top 1 Origin from ridepat where RideId = RideNum), N'נקודת אמצע')
),
Destination = isnull((select top 1 Destination from ridepat where RideId = RideNum), N'נקודת אמצע')
GO

--TEST
select *
from ride r inner join ridepat rp
on rp.RideId=r.RideNum
where
r.Destination <> rp.Destination
or
r.Origin <> rp.Origin
GO
