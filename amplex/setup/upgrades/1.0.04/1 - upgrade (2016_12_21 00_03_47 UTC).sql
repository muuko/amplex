if exists ( select [id] from sysobjects where xtype = 'u' and [name] = 'scms_search_results_module' )
  drop table scms_search_results_module
go
if exists ( select [id] from sysobjects where xtype = 'u' and [name] = 'scms_search_input_module' )
  drop table scms_search_input_module
go
if exists( select [id] from sysobjects where xtype = 'u' and [name] = 'scms_search_settings' )
  drop table scms_search_settings
go
if exists ( select [id] from sysobjects where xtype = 'u' and [name] = 'scms_search_index' )
  drop table scms_search_index
go
if exists ( select [id] from sysobjects where xtype = 'u' and [name] = 'scms_search_wordindex' )
  drop table scms_search_wordindex
go
if exists ( select [id] from sysobjects where xtype = 'u' and [name] = 'scms_search_ignore' )
  drop table scms_search_ignore
go
if exists ( select [id] from sysobjects where xtype = 'u' and [name] = 'scms_search_moduletext' )
  drop table scms_search_moduletext
go




create table scms_search_ignore
(
  word nvarchar(16) primary key not null
)
go

create table scms_search_moduletext
(
 instanceId int primary key not null,
 [text] text,
 constraint FK_scms_search_moduletext_instance foreign key (instanceId) references scms_plugin_module_instance (id)
)
go

create table scms_search_wordindex
(
 id int not null primary key identity,
 word nvarchar(32) not null
)
go
create index IX_scms_search_wordindex on scms_search_wordindex(word)
go

create table scms_search_index
(
 wordid int not null,
 instanceId int not null,
 count int not null,
 constraint PK_scms_search_index primary key  (wordid, instanceid),
 constraint FK_scms_search_index_word foreign key (wordid) references scms_search_wordindex (id),
 constraint FK_scms_search_index_instance foreign key (instanceId) references scms_plugin_module_instance (id)
)
go
create index IX_scms_search_index on scms_search_index(wordid, instanceid, count)
go


create table scms_search_settings
(
	siteId int not null primary key,
	searchResultsPageId int null,
	constraint FK_scms_search_config_page foreign key (searchResultsPageId) references scms_page (id)
)
go

create table scms_search_input_module
(
	id int not null primary key identity,
	instanceId int not null,
	CssClass nvarchar(64) null,
	CssClassTextInputActive nvarchar(64) null,
	CssClassTextInputInactive nvarchar(64) null,
    DefaultText nvarchar(64) null,
    ValidationErrorMessage nvarchar(256) null,
    CssClassButtonInput nvarchar(64) null,
    ButtonText nvarchar(64) null,
	UseImageButton bit not null default 0,
    ButtonImagePath nvarchar(1024) null,
    resultsPageId int null,
    constraint FK_scms_search_input_module foreign key (instanceId) references scms_plugin_module_instance(id),
    constraint FK_scms_search_input_page foreign key (resultsPageId) references scms_page(id)
)
go

create table scms_search_results_module
(
	id int not null primary key identity,
	instanceId int not null,
	maxResultCount int null,
	pageSize int null,
	maxKeywords int null,
	showThumbnail bit not null,
	thumbnailWidth int null,
	thumbnailHeight int null,
	showUrl bit not null,
	showReadMore bit not null,
	showPrevNext bit not null,
    constraint FK_scms_search_results_module foreign key (instanceId) references scms_plugin_module_instance(id)
)

insert scms_plugin_application
(
	[name],
	[description],
	controlPathSettings
)
values
(
	'search',	-- [name] 
	'Keyword search.',
	'/scms/modules/search/settings.ascx' -- controlPathSettings nvarchar(1024) null
)



declare @appid_search int
select @appid_search = scope_identity()

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
	@appid_search,
	'input',
	'performs keyword search and displays results ',
	'/scms/modules/search/searchinput/edit.ascx',
	'/scms/modules/search/searchinput/view.ascx'
)


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
	@appid_search,
	'results',
	'performs keyword search and displays results ',
	'/scms/modules/search/searchresults/edit.ascx',
	'/scms/modules/search/searchresults/view.ascx'
)
go




drop table scms_site_plugin_application
go

alter table scms_page
add searchInclude bit not null default 1
go

insert scms_config values ( 'search-cache-timeout-seconds', '3600')
go