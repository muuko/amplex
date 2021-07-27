begin tran
select @@trancount --rollback
go

if exists ( select [id] from sysobjects where xtype = 'u' and [name]  = 'cat_part' )
	drop table cat_part
go

create table cat_part
(
	id int not null identity primary key,
	sage_SKICPart int not null,
	sage_price decimal(8,2) null,	
	sage_ID varchar(21) null,
	sage_Description1 varchar(29),
	sage_Description2 varchar(29),
	sage_Description3 varchar(29),
	sage_LongDescription varchar(1000),
	[hash] varchar(256),
	lastUpdated datetime,
	imageUrl nvarchar(1024),
	pageId int null,
	instanceId int null,
	constraint FK_cat_part_page foreign key (pageId) references scms_page(id),
	constraint FK_cat_part_instance foreign key (instanceId) references scms_plugin_module_instance(id)
)
go

create index IX_cat_part_ids on cat_part(id, sage_SKICPart)
go

create index IX_cat_part_checksum on cat_part(sage_SKICPart, [hash])
go

create index IX_cat_part_sage_id on cat_part(id, sage_ID)
go

create table cat_settings
(
	siteId int primary key not null,
	searchResultsPageId int null,
	searchResultsPageModuleInstanceId int null
)
go

insert scms_plugin_application
( name, description, controlPathSettings, type )
values
( 'parts', 'Enable Sage part administration', '/scms/modules/parts/settings.ascx', 'scms.modules.parts.partsApplication' )
go


declare @appId int
select @appId = [id] 
from scms_plugin_application
where name = 'parts'

insert scms_plugin_module
(
	pluginAppId,
	[name],
	[description],
	controlPathEditModule,
	controlPathViewModule
)
values
(
	@appId,
	'part',
	'Display Sage part',
	'/scms/modules/parts/part/edit.ascx',
	'/scms/modules/parts/part/view.ascx'
)
go



declare @appId int
select @appId = [id] 
from scms_plugin_application
where name = 'parts'

insert scms_plugin_module
(
	pluginAppId,
	[name],
	[description],
	controlPathEditModule,
	controlPathViewModule
)
values
(
	@appId,
	'catalog',
	'Display Sage Part Catalog',
	'/scms/modules/parts/catalog/edit.ascx',
	'/scms/modules/parts/catalog/view.ascx'
)
go


commit