/*NEW CODE WHICH EXIST IN TEST AND YET EXIST IN PROD*/

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

create procedure spVolunteer_GetActiveVolunteers_NotDriversYet 
@daysSinceJoin int  as  
	BEGIN 
		select * from volunteer 
		where  ( 
				CASE WHEN JoinDate is not null THEN DATEDIFF(DAY, JoinDate, getdate())
				end
			   ) > @daysSinceJoin  
			and   IsActive=1
			and   Id not in (select distinct MainDriver from ride where MainDriver is not null)
	END


