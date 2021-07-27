begin tran
select @@TRANCOUNT
go


alter table scms_rss_list
add templateEnabled bit not null default 0

alter table scms_rss_list
add templateHeaderHtml nvarchar(max) null

alter table scms_rss_list
add templateHtml nvarchar(max) null

alter table scms_rss_list
add templateFooterHtml nvarchar(max) null

alter table scms_rss_list
add templateSeparatorHtml nvarchar(max) null


commit