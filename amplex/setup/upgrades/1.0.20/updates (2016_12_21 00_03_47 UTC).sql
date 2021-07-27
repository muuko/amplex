begin tran
select @@trancount
go

drop table scms_search_index
go

drop table scms_search_sourcetext
go

drop table scms_search_source
go








create table scms_search_source
(
 id int identity primary key not null,
 moduleInstanceId int null,
 pageId int null,
 constraint FK_scms_search_source_moduleInstance foreign key (moduleInstanceId) references scms_plugin_module_instance (id),
 constraint FK_scms_search_source_pageId foreign key (pageId) references scms_page(id)
)
go



create table scms_search_index
(
 wordId int not null,
 searchSourceId int not null,
 count int not null,
 constraint PK_scms_search_index primary key  (wordid, searchSourceId),
 constraint FK_scms_search_index_word foreign key (wordid) references scms_search_wordindex (id) on delete cascade ,
 constraint FK_scms_search_index_instance foreign key (searchSourceId) references scms_search_source (id) on delete cascade 
)
go


create table scms_search_sourcetext
(
	id int not null primary key identity,
	sourceid int not null,
	[text] text null,
	constraint FK_scms_search_sourcetext_source foreign key (sourceid) references scms_search_source(id) on delete cascade
)

alter table scms_navigation_subnav
add maxChildrenPerNode int null
go


commit