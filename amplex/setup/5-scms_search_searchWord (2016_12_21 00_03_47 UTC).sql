/*
declare @count int
exec scms_search_searchWord 105, 100, @count out

select * from scms_search_wordindex
select * from scms_search_index where wordid = 15
select * from scms_search_index where wordid = 105
*/

if exists ( select [id] from sysobjects where xtype = 'p' and [name] = 'scms_search_searchWord')
	drop procedure scms_search_searchWord
go
create procedure scms_search_searchWord
(
	@wordId int,
	@maxResults int
)
as

begin

	declare @tblResults table
	(
		pageId int not null primary key,
		count int not null
	)
	
	insert @tblResults 
	(
		pageId,
		count
	)
	select 
		p.id, 
		sum(i.[count])
	from 
		scms_page p,
		scms_search_index i,
		scms_page_plugin_module ppm
	where i.wordid = @wordId
	-- and p.includeInSearch = 1
	and i.instanceId = ppm.instanceid
	and p.id = ppm.pageId
	and p.deleted = 0
	group by p.id

	declare @tblResultsIndexed table
	(
		id int not null identity primary key,
		pageId int not null,
		count int not null
	)	
	insert @tblResultsIndexed ( pageId, count )
	select pageId, [count]
	from @tblResults
	order by [count] desc
	
	
	select pageId, count
	from @tblResultsIndexed
	where ( (id <= @maxResults) or (isnull(@maxResults,0) = 0) )
	
	
end


	
