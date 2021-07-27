begin tran
select @@trancount
go

if exists (select [id] from sysobjects where xtype='u' and name = 'scms_rss_list')
drop table scms_rss_list
go

if exists (select [id] from sysobjects where xtype='u' and name = 'scms_rss_item')
drop table scms_rss_item
go

if exists (select [id] from sysobjects where xtype='u' and name = 'scms_rss')
drop table scms_rss
go

create table scms_rss
(
	id int identity not null primary key,
	name nvarchar(64) not null,
	feedUrl nvarchar(1024) not null,
	lastUpdate datetime null,
	expiresSeconds int not null,
	retainDropOff bit not null default 0,
	constraint UN_scms_rss_name unique (name)
)
go


create table scms_rss_item
(
	id int identity not null primary key,
	rssFeedId int not null,
	blocked bit not null default 0,
	title nvarchar(1024) null,
	[description] nvarchar(max) null,
	link nvarchar(1024) not null,
	[guid] nvarchar(1024) null,
	pubDate datetime not null,
	dc_creator nvarchar(256),
	content nvarchar(max) null,
	constraint fk_scms_rss_item foreign key (rssFeedId) references scms_rss(id)  on delete cascade
)
go


create table scms_rss_list
(
	id int identity primary key not null,
	instanceId int not null,
	rssId int not null,
--	linkExternalNotInternal bit not null default 1,
--	linkInternalPageId int null,
--	templateEnabled bit not null default 0,
--	templateHeaderHtml nvarchar(max) null,
--	templateHtml nvarchar(max) null,
--	templateFooterHtml nvarchar(max) null,
--	templateSeparatorHtml nvarchar(max) null,
	titleEnabled bit not null default 1,
	titleAsLink bit null,
--	descriptionEnabled bit not null default 1,
--	descriptionAsLink bit not null default 0,
--	itemReadMoreEnabled bit not null default 0,
--	itemReadMoreText nvarchar(256) null,
	listLimitItems bit not null default 0,
	listMaxItems int null,
	listReadMoreEnabled bit not null default 0,
	listReadMoreText nvarchar(256) null,
	listReadMorePageId int null,
--	pagingEnabled bit not null default 0,
--	pagingItemsPerPage int null,
	constraint FK_scms_rss_list_instance foreign key(instanceId) references scms_plugin_module_instance (id) on delete cascade,
	constraint FK_scms_rss_list_rss foreign key (rssId) references scms_rss (id) on delete cascade,
-- 	constraint FK_scms_rss_list_linkinternal foreign key (linkInternalPageId) references scms_page (id) on delete cascade,
	constraint FK_scms_rss_list_readmore foreign key ( listReadMorePageId ) references scms_page (id) on delete cascade
)
go

declare @appId int
select @appId = [id] 
from scms_plugin_application
where name = 'rss'

delete scms_page_plugin_module
from scms_page_plugin_module ppm, scms_plugin_module_instance pmi, scms_plugin_module pm
where ppm.instanceId = pmi.id
and pmi.pluginModuleId = pm.id
and pm.pluginAppId = @appId

delete scms_plugin_module_instance
from scms_plugin_module_instance pmi, scms_plugin_module pm
where pm.pluginAppId = @appId
and pmi.pluginModuleId = pm.id

delete scms_plugin_module where pluginAppId = @appId
delete scms_plugin_application where name = 'rss'
go

insert scms_plugin_application ( name, description, controlPathSettings, type )
values( 'rss', 'rss feed', '/scms/modules/rss/settings.ascx', null ) -- 'scms.modules.remote.remoteApplication' )
go


declare @appId int
select @appId = [id] 
from scms_plugin_application
where name = 'rss'

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
	'RSS Feed List',
	'Display rss feed list',
	'/scms/modules/rss/rssList/edit.ascx',
	'/scms/modules/rss/rssList/view.ascx'
)
go

-- select * from scms_plugin_application
-- select * from scms_plugin_module


-- rollback
-- commit