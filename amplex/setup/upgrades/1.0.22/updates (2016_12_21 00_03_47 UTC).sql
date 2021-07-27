begin tran
select @@trancount -- rollback
go


alter table scms_search_index drop constraint fk_scms_search_index_instance
go

alter table scms_search_sourcetext drop constraint fk_scms_search_sourcetext_source
go

drop table scms_search_source
go

drop table scms_search_target
go


drop index scms_search_source.IX_scms_search_source
go



create table scms_search_target
(
	id int not null primary key identity,
	pageId int not null,
	queryString nvarchar(max) null,
	titleOverride nvarchar(256) null,
	summaryOverride ntext null ,
	thumbnailOverride nvarchar(1024) null,
	constraint FK_scms_search_target_page foreign key  (pageId) references scms_page(id)
)
go

create table scms_search_source
(
	id int not null primary key identity,
	pageId int null,
	ppmi int null,
	targetId int null,
	itemId int null,
	constraint UN_scms_search_source unique (ppmi,pageId,ItemId),
	constraint FX_scms_search_source_target foreign key (targetId) references scms_search_target (id),
	constraint FX_scms_search_source_page foreign key (pageId) references scms_page (id)
)
go	


create index IX_scms_search_source_target on scms_search_source(id,targetId)
go

create index IX_scms_search_source on scms_search_source(id,ppmi,itemid)
go



commit


