alter table scms_rss
add heading nvarchar(1024) null

alter table scms_rss_list
add headingEnabled bit not null default 1