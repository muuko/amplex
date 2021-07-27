/*
declare @count int
exec scms_search_searchword @wordId = 1831, @maxResults = 100

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
		targetId int not null primary key,
		count int not null
	)
	
	insert @tblResults 
	(
		targetId,
		count
	)
	
	select t.id, sum(i.[count])
		
	from scms_search_target t

	left join scms_search_source s
	on t.id = s.targetId

	left join scms_search_index i
	on s.id = i.searchSourceId
	
	where i.wordid = @wordId
	and t.pageId not in (select id from scms_page
						 where searchInclude = 0
						 or deleted = 1)
	group by t.id
	
	
	

	declare @tblResultsIndexed table
	(
		id int not null identity primary key,
		targetId int not null,
		count int not null
	)	
	insert @tblResultsIndexed ( targetId, count )
	select targetId, [count]
	from @tblResults
	order by [count] desc
	
	select targetId, count
	from @tblResultsIndexed
	where ( (id <= @maxResults) or (isnull(@maxResults,0) = 0) )
	
	
end


	
