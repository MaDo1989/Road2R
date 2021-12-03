/*NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD*/

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
	--Patient --Escorted --Volunteer 
	set LastUpdateBy = N'אדמין מערכת' 
	where id in   (  
	select id from RidePat
	--Patient --Escorted --Volunteer   
	where LastUpdateBy is null   )

	    create procedure  spEscorted_ChangeLastUpdateBy  
		@lastUpdateBy nvarchar(255),  
		@id int   
		as 
		begin  
			update Escorted  
			set LastUpdateBy=@lastUpdateBy 
			where Id=@id  
		end    

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

	/****** Object:  StoredProcedure [dbo].[spVolunteerTypeView_GetVolunteersList]    Script Date: 11/27/2021 6:27:34 PM ******/
ALTER procedure [dbo].[spVolunteerTypeView_GetVolunteersList]
@IsActive bit
as
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
				select top 1 maindriver, origin, destination, count(*) as numberOfTimesDrove FROM RIDE
				where maindriver = id
				group by maindriver, origin, destination
				order by numberOfTimesDrove desc
				) t
			) mostCommonPath
	from VolunteerTypeView vtv
	where IsActive = @IsActive
	order by firstNameH
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


