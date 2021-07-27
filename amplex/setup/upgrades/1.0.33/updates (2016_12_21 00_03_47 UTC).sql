sp_help scms_rss_item
begin tran
select @@TRANCOUNT
go

alter table scms_rss add categories nvarchar(max)
go

alter table scms_rss_item add categories nvarchar(max)
go

commit
