if exists ( select [id] from sysobjects where xtype = 'p' and [name] = 'scms_search_clear')
	drop procedure scms_search_clear
go
create procedure scms_search_clear
(
  @pageid int,
  @pageModuleInstanceId int,
  @itemId int
)
as
begin
	if (@pageid is null) and (@pageModuleInstanceId is null)
	begin
		-- print '1'
		delete from scms_search_index
		-- print '1.1'
		delete from scms_search_sourcetext
		-- print '1.2'
		
		
		delete scms_search_source
		-- print '1.3'
		delete from scms_search_target
		-- print '1.4'
	end
	else
	begin
		-- print '2'
		if (@pageModuleInstanceId is null )
		begin
			-- print '4'
			delete scms_search_source 
			where pageid = @pageId
					
			-- print '3'
			delete scms_search_target
			from scms_search_target st,
			scms_search_source ss
			where ss.pageid = @pageid
			and ss.targetid = st.id
		end
		else
		begin
			-- print '5'
			if(@itemId is null)
			begin
				-- print '7'
				delete scms_search_source
				where pageid = @pageid
				and ppmi = @pageModuleInstanceId
				
				-- print '6'
				delete scms_search_target
				from scms_search_target st,
				scms_search_source ss
				where ss.pageid = @pageid
				and ss.targetid = st.id
				and ss.ppmi = @pageModuleInstanceId
			end
			else
			begin
				-- print '9'
				delete scms_search_source
				where pageid = @pageid
				and ppmi = @pageModuleInstanceId				
				and itemid = @itemId
							
				-- print '8'
				delete scms_search_target
				from scms_search_target st,
				scms_search_source ss
				where ss.pageid = @pageid
				and ss.targetid = st.id
				and ss.ppmi = @pageModuleInstanceId
				and ss.itemid = @itemId
			end
		end
	end
end
go

/*
begin tran
select @@trancount -- rollback
exec scms_search_clear
  @pageid = null,
  @pageModuleInstanceId = null,
  @itemId = null
  
  

*/