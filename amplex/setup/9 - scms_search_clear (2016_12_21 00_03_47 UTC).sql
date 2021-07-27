if exists ( select [id] from sysobjects where xtype = 'p' and [name] = 'scms_search_clear')
	drop procedure scms_search_clear
go
create procedure scms_search_clear
(
  @pageid int,
  @moduleInstanceId int
)
as
begin
	if (@pageid is null) and (@moduleInstanceId is null)
	begin
		delete from scms_search_index
		delete from scms_search_sourcetext
		delete scms_search_source
	end
	else
	begin
		if (@moduleInstanceId is null )
		begin
			delete scms_search_source 
			where pageid = @pageId
		end
		else
		begin
			delete scms_search_source
			where pageid = @pageid
			and moduleinstanceid = @moduleInstanceId
		end
	end
end
go