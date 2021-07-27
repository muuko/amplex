
begin tran
select @@trancount
go

alter table scms_search_target
alter column queryString nvarchar(256) null

create index IX_scms_search_target_pq on scms_search_target(pageid,querystring)
go

create index IX_scms_source_ppmi_item on scms_search_source(ppmi,itemId)
go

create index IX_scms_source_page on scms_search_source(pageId)
go

create index IX_scms_search_index on scms_search_index(searchSourceId)
go

commit

/*
update cat_part set hash=null
*/