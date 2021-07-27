alter table scms_page
add associatedDate datetime null
go

if exists (select [id] from sysobjects where name = 'scms_navigation_pagelist' )
	drop table scms_navigation_pagelist
go



create table scms_navigation_pagelist
(
	id int primary key not null identity,
	instanceId int not null,
	rootPageId int null,
	includeChildren bit not null,
	hideParentNodes bit not null,
	templateEnabled bit not null,
	templateHtml ntext null,
	templateSeparatorHtml ntext null,
	templateHeaderHtml ntext null,
	templateFooterHtml ntext null,
	titleEnabled bit not null,
	titleAsLink bit null,
	linkTextEnabled bit not null,
	linkTextAsLink bit null,
	associatedDateEnabled bit not null,
	associatedDateFormat nvarchar(64) null,
	thumbnailEnabled bit not null,
	thumbnailAsLink bit null,
	thumbnailHideIfEmpty bit null,
	thumbnailWidth int null,
	thumbnailHeight int null,
	descriptionEnabled bit not null,
	descriptionTruncated bit null,
	descriptionTruncateLength int null,
	itemReadMoreEnabled bit not null,
	itemReadMoreText nvarchar(256) null,
	listLimitItems bit not null,
	listMaxItems int null,
	listReadMoreEnabled bit not null,
	listReadMoreText nvarchar(256) null,
	listReadMorePageId int null,
	pagingEnabled bit not null,
	pagingItemsPerPage int null,
	-- order by ?
	constraint FK_scms_navigation_list_plugin_instance foreign key (instanceId) references scms_plugin_module_instance(id) on delete cascade,
	constraint FK_scms_navigation_list_page foreign key (rootPageId) references scms_page(id),
	constraint FK_scms_navigation_list_page2 foreign key (listReadMorePageId) references scms_page(id)
)	
go

declare @appId int
select @appId = id from scms_plugin_application where [name] = 'navigation'
delete scms_plugin_module where pluginappid = @appid and [name] = 'pagelist'

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
	@appid,
	'pagelist',
	'displays list of pages',
	'/scms/modules/navigation/pagelist/edit.ascx',
	'/scms/modules/navigation/pagelist/view.ascx'
)

