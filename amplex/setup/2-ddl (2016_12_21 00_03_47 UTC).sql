begin tran -- rollback -- commit
select @@trancount
go

-- security
if exists ( select [id] from sysobjects where [name] = 'scms_organization_users' and xtype = 'u' )
	drop table scms_organization_users
go

if exists ( select [id] from sysobjects where [name] = 'scms_organization' and xtype = 'u' )
	drop table scms_organization
go

if exists ( select [id] from sysobjects where [name] = 'scms_security_settings' and xtype = 'u' )
	drop table scms_security_settings
go
if exists ( select [id] from sysobjects where [name] = 'scms_page_role' and xtype = 'u' )
	drop table scms_page_role
go
if exists ( select [id] from sysobjects where [name] = 'scms_user' and xtype = 'u' )
	drop table scms_user
go





-- search
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

-- submissions
if exists ( select [id] from sysobjects where [name] = 'scms_submission_submission' and xtype = 'u' )
 drop table scms_submission_submission
go

if exists ( select [id] from sysobjects where [name] = 'scms_submission_form_eventhandler' and xtype = 'u' )
 drop table scms_submission_form_eventhandler
go

if exists ( select [id] from sysobjects where [name] = 'scms_submission_module' and xtype = 'u' )
 drop table scms_submission_module
go


-- forms
if exists ( select [id] from sysobjects where [name] = 'scms_form_eventhandler' and xtype = 'u' )
	drop table scms_form_eventhandler

if exists (select [id] from sysobjects where name = 'scms_form_submission_optionfieldvalue')
  drop table scms_form_submission_optionfieldvalue

if exists (select [id] from sysobjects where name = 'scms_form_submission_fieldvalue' )
	drop table scms_form_submission_fieldvalue

if exists (select [id] from sysobjects where name = 'scms_form_submission' )
	drop table scms_form_submission
 
if exists (select [id] from sysobjects where name = 'scms_form_instance' )
	drop table scms_form_instance

if exists (select [id] from sysobjects where name = 'scms_form_field_option' )
	drop table scms_form_field_option

if exists (select [id] from sysobjects where name = 'scms_form_field' )
	drop table scms_form_field

if exists (select [id] from sysobjects where name = 'scms_form' )
	drop table scms_form
	



-- navigation

if exists (select [id] from sysobjects where name = 'scms_navigation_pagelist' )
	drop table scms_navigation_pagelist

if exists (select [id] from sysobjects where name = 'scms_navigation_subnav' )
	drop table scms_navigation_subnav

if exists (select [id] from sysobjects where name = 'scms_navigation_pagedetail' )
  drop table scms_navigation_pagedetail

if exists (select [id] from sysobjects where name = 'scms_content' )
	drop table scms_content

if exists (select [id] from sysobjects where name = 'scms_page_plugin_module' )
	drop table scms_page_plugin_module

if exists (select [id] from sysobjects where name = 'scms_template_plugin_module' )
	drop table scms_template_plugin_module


if exists (select [id] from sysobjects where name = 'scms_plugin_module_instance' )
	drop table scms_plugin_module_instance

if exists (select [id] from sysobjects where name = 'scms_plugin_module' )
	drop table scms_plugin_module

if exists (select [id] from sysobjects where name = 'scms_plugin_application' )
	drop table scms_plugin_application


if exists (select [id] from sysobjects where name = 'fk_scms_site_page' )
	alter table scms_site drop constraint fk_scms_site_page

if exists (select [id] from sysobjects where name = 'fk_scms_site_template' )
	alter table scms_site drop constraint fk_scms_site_template

if exists (select [id] from sysobjects where name = 'scms_config' )
	drop table scms_config

if exists (select [id] from sysobjects where name = 'scms_page' )
	drop table scms_page

if exists (select [id] from sysobjects where name = 'scms_template' )
	drop table scms_template

if exists (select [id] from sysobjects where name = 'scms_master' )
	drop table scms_master

if exists (select [id] from sysobjects where name = 'scms_site' )
	drop table scms_site


create table scms_config
(
	name nvarchar(32) not null primary key,
  value ntext null
)

create table scms_site
(
	id int identity not null primary key,
	name nvarchar(256) not null unique,
	homePageId int null,
	hostNameRegex nvarchar(1024),
	defaultTemplateId int null,
	xmlSitemapEnabled bit not null,
	xmlSitemapLocation nvarchar(1024),
	filesLocation nvarchar(1024) not null, 
	deleted bit not null default 0
)

create table scms_master
(
	id int identity not null primary key,
	siteId int not null,
	name nvarchar(32) not null,
	path nvarchar(1024) not null,
	deleted bit not null default 0,

	constraint FK_scms_master_site foreign key (siteid) references scms_site(id),
	constraint UN_scms_master_name unique (siteid,name)
)

create table scms_template
(
	id int identity not null primary key,
	masterId int not null,
	siteId int not null,
	name nvarchar(32) not null,
	deleted bit not null default 0,

  constraint FK_scms_template_master foreign key (masterId) references scms_master(id),
	constraint FK_scms_template_site foreign key (siteid) references scms_site(id)
)

alter table scms_site 
add constraint FK_scms_site_template 
foreign key (defaultTemplateId) 
references scms_template(id)

create table scms_page
(
	id int identity not null primary key,
	type char not null,  -- P:Page, R:Redirect, A:Alias, I:Internal
	siteid int not null,
	parentId int null,
	fragment nvarchar(256) not null,
	linktext nvarchar(256) not null,
	title nvarchar(256) not null,
	description nvarchar(1024) null,
	keywords nvarchar(1024) null,
	summary ntext null,
	thumbnail nvarchar(1024) null,
	associatedDate datetime null,
	visible bit not null,
	searchInclude bit not null,
	
	-- control
	lastUpdated datetime not null,
	url nvarchar(1024),
	ordinal int not null,
	deleted bit not null,

	-- canonical
	canonicalPageId int null,
	canonicalUrl nvarchar(1024) null,

	-- security
	securityInherit bit not null,
	securityLoginRequired bit null,
	securityRestrictToRoles bit null,

	-- sitemap
	sitemapInclude bit not null,
	sitemapLinkText nvarchar(256) null,

	-- xml sitemap
	xmlSitemapInclude bit not null,
	xmlSitemapPriority decimal(5,2)  null,
	xmlSitemapUpdateFrequency nvarchar(32) null,

  -- page only settings
	masterId int null,
	templateId int null,
	
	-- redirect only settings
	redirectUrl nvarchar(1024),
	redirectPageId int null,
	redirectPermanent bit null,

	-- alias only settings
	aliasPageId int null,

	-- internal only settings
	internalUrl nvarchar(1024),
	
	constraint FK_scms_page_parent foreign key (parentId) references scms_page(id),
	constraint FK_scms_page_master foreign key (masterId) references scms_master(id),
	constraint FK_scms_page_template foreign key (templateId)references scms_template(id),
	constraint FK_scms_page_site foreign key (siteId) references scms_site(id),
	constraint FK_scms_page_canonical foreign key(canonicalPageId) references scms_page(id),
	constraint FK_scms_page_alias foreign key(aliasPageId) references scms_page(id),
)


alter table scms_site add constraint FK_scms_site_page foreign key(homePageId) references scms_page(id)

/*
create table scms_page_index
(
	pageId int not null,
	idx int not null,
	idxLastChild int null
)
*/


create table scms_page_role
(
	pageid int not null,
	RoleId uniqueidentifier not null,
	constraint scms_page_role_pk primary key (pageid, roleid),
	constraint scms_page_role_page foreign key (pageid) references scms_Page(id),
	constraint scms_page_role_role foreign key (roleid) references aspnet_roles(roleid) on delete cascade
)
go

create table scms_security_settings
(
	siteid int not null primary key,
	pageIdLogin int null,
	constraint scms_security_settings_fk_site foreign key (siteid) references scms_site(id)
)
go


create table scms_plugin_application
(
	id int not null identity primary key,
	[name] nvarchar(32) not null unique,
	[description] nvarchar(1024) null,
	controlPathSettings nvarchar(1024) null,
	[type] nvarchar(1024) null
)

create table scms_plugin_module
(
	id int not null identity primary key,
	pluginAppId int not null,
	[name] nvarchar(32) not null,
	[description] nvarchar(1024) not null,
	controlPathEditModule nvarchar(1024) null,
	controlPathViewModule nvarchar(1024) null,

	constraint FK_scms_plugin_module_app foreign key (pluginAppId) references scms_plugin_application(id),
	constraint UQ_scms_plugin_module_name unique(pluginAppId, name)
)

create table scms_plugin_module_instance
(
	id int not null identity primary key,
	siteId int not null,
	pluginAppId int not null,
	pluginModuleId int not null,
	wrapModule bit not null,
	cssClassWrap nvarchar(64),
	deleted bit not null,

	constraint FK_scms_plugin_module_instance_site foreign key (siteId) references scms_site(id),
	constraint FK_scms_plugin_module_instance_app foreign key (pluginAppId) references scms_plugin_application(id),
	constraint FK_scms_plugin_module_instance_module foreign key (pluginModuleId) references scms_plugin_module(id)
)


create table scms_template_plugin_module
(
	id int not null identity primary key,
	instanceId int not null,
	siteId int not null,
	templateId int not null,
	[name] nvarchar(128) not null,
	placeHolder nvarchar(256) not null,
	ordinal int not null,
	owner bit not null,
	deleted bit not null,

	constraint FK_scms_template_plugin_module_instance foreign key (instanceId) references scms_plugin_module_instance(id),
	constraint FK_scms_template_plugin_module_site foreign key (siteId) references scms_site(id),
	constraint FK_scms_template_plugin_module_template foreign key (templateId) references scms_template(id)
)

create table scms_page_plugin_module
(
	id int identity not null primary key,
	instanceId int not null,
	siteId int not null,
	pageId int not null,
	[name] nvarchar(128) not null,
	placeHolder nvarchar(256) not null,
	ordinal int not null,
	owner bit not null,
	overrideTemplate bit not null,
	deleted bit not null,
	
	constraint FK_scms_page_plugin_module_instance foreign key (instanceId) references scms_plugin_module_instance(id),
	constraint FK_scms_page_plugin_module_site foreign key (siteId) references scms_site(id),
	constraint FK_scms_page_plugin_module_page foreign key (pageId) references scms_page(id),
)


create table scms_user
(
	userid uniqueidentifier primary key not null,
	firstName nvarchar(128),
	lastName nvarchar(128),
	constraint FK_scms_user_aspnet_users foreign key (userid) references aspnet_users(UserId) on delete cascade 
)

create table scms_organization
(
	id int not null identity primary key,
	[name] nvarchar(256)
)

create table scms_organization_users
(
	organization_id int not null,
	user_id uniqueidentifier not null,
	constraint pk_organization_users primary key (organization_id, user_id),
	constraint fk_organization_users_organization foreign key (organization_id) references scms_organization(id), 
	constraint fk_orginization_users_user foreign key (user_id) references aspnet_users(UserId),
)

create table scms_registration_pending
(
	userId  uniqueidentifier not null,
	[key] uniqueidentifier not null,
	dtCreated DateTime not null default(getdate()),
	constraint fk_registration_pending_user foreign key (userId) references aspnet_users(UserId) on delete cascade,
	constraint pk_registration_pending primary key (userId,[key]) 
)






delete scms_config
insert scms_config values ( 'scms-directory', '/scms' )
insert scms_config values ( 'files', '/files' )
insert scms_config values ( 'basepage', '/scms/page.aspx' )
insert scms_config values ( 'config-cache-timeout-seconds', '900' )
insert scms_config values ( 'template-cache-timeout-seconds', '3600' )
insert scms_config values ( 'plugin-cache-timeout-seconds', '3600' )
insert scms_config values ( 'content-cache-timeout-seconds', '3600' )
insert scms_config values ( 'search-cache-timeout-seconds', '3600')
insert scms_config values ( 'recaptcha-public-key', '6LddNQkAAAAAAL_coAOfzy2lIcz9V3OyT_xxlh_U ' )
insert scms_config values ( 'recaptcha-private-key', '6LddNQkAAAAAAHJV1KiHqZR0laISivtOJIUSKzJp ' )
insert scms_config values ( 'show-admin-edit-links', 'true' )
insert scms_config values ( 'image-not-available-path', '/scms/client/images/image-not-available.jpg')
insert scms_config values ( 'events-max-events', '1000')
insert scms_config values ( 'events-trim-events', '800')
-- insert scms_config values ( 'host', 'localhost:

-- select * from scms_config
-- select * from scms_template
-- delete scms_template
-- begin tran 
-- select @@trancount -- rollback 

insert scms_site ( name, xmlSitemapEnabled, xmlSitemapLocation, filesLocation ) values ( 'default', 1, '/sitemap.xml', '/sites/amplex/' )
declare @siteId int
select @siteid = scope_identity()

insert scms_master ( siteid, name, path ) values ( @siteid, 'basic', '/sites/amplex/_masters/BasicMaster.master' )
declare @masterId int 
select @masterId = scope_identity()

insert scms_template ( siteid, name, masterId ) values ( @siteid, 'basic', @masterId )
declare @templateId int 
select @templateId = scope_identity()

update scms_site 
set defaultTemplateId = @templateId
where id = @siteid

-- scripted project base
insert scms_page
(
	type, 
	siteid,
	parentId,
	fragment,
	linktext,
	title,
	description,
	keywords,
	summary,
	thumbnail,
	visible,
	searchInclude,
	
	-- control
	lastUpdated,
	url,
	ordinal,
	deleted,

	-- canonical
	canonicalPageId,
	canonicalUrl,
	
	-- security
	securityInherit,
	securityLoginRequired,

	-- sitemap
	sitemapInclude,
	sitemapLinkText,

	-- xml sitemap
	xmlSitemapInclude,
	xmlSitemapPriority,
	xmlSitemapUpdateFrequency,

  -- page only settings
	masterId,
	templateId,
	
	-- redirect only settings
  redirectUrl,
	redirectPageId,
	redirectPermanent,

	-- alias only settings
	aliasPageId,

	-- internal only settings
  internalUrl
  
)
values 
(
	'P',
	@siteId, -- siteid int not null,
	null, -- parentId int null,
	'', -- fragment nvarchar(256) not null,
	'home', --linktext nvarchar(256) not null,
	'Home', --title nvarchar(256) not null,
	null, --description nvarchar(1024) null,
	null, --keywords nvarchar(1024) null,
	null, --summary ntext null,
	null, --thumbnail nvarchar(1024) null,
	1, -- visible bit not null,
	1, -- search include
	
	-- control
	GetDate(), -- lastUpdated datetime not null,
	'/', -- url nvarchar(1024),
	1, --ordinal int not null,
	0, --deleted bit not null,

	-- canonical
	null, --canonicalPageId int null,
	null, --canonicalUrl nvarchar(1024) null,

	-- security
	0, -- securityInherit
	0, -- securityLoginRequired

	-- sitemap
	1, --sitemapInclude bit not null,
	'Home Page', -- sitemapLinkText nvarchar(256) null,

	-- xml sitemap
	1, -- xmlSitemapInclude bit not null,
	0.9, -- xmlSitemapPriority decimal  null,
	'weekly', --xmlSitemapUpdateFrequency nvarchar(32) null,

  -- page only settings
	@masterId, -- masterId int null,
	@templateId, -- templateId int null,
	
	-- redirect only settings
  null, --redirectUrl nvarchar(1024),
	null, --redirectPageId int null
	null, -- redirectPermanent bit,

	-- alias only settings
	null, --aliasPageId int null,

	-- internal only settings
  null -- internalUrl nvarchar(1024),
	
)	


declare @homePageId int
select @homePageId = scope_identity()
select @homePageId

-- select * from scms_page 
update scms_site set homepageid = @homepageid where id = @siteid
-- select * from scms_site

create table scms_content
(
	id int primary key not null identity,
	instanceId int not null,
	[content] ntext null,
	literal bit not null,
	dtVersion datetime not null,
	constraint FK_scms_content_page_plugin_instance foreign key  (instanceId) references scms_plugin_module_instance(id)
)



insert scms_plugin_application
(
	[name],
	[description],
	controlPathSettings
)
values
(
	'content',	-- [name] 
	'Place html formatted content on web pages.',
	'/scms/modules/content/controls/settings.ascx' -- controlPathSettings nvarchar(1024) null
)
declare @appid int
select @appid = scope_identity()


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
	'content',
	'displays html formatted content',
	'/scms/modules/content/controls/edit.ascx',
	'/scms/modules/content/controls/view.ascx'
)


create table scms_navigation_subnav
(
	id int primary key not null identity,
	instanceId int not null,
	maxDepth int null,
	showChildren bit not null,
	pinNavigationToHomePage bit not null,
	pinDepth int null,
	showSiblingsIfNoChildren bit not null,
	cssClassActive nvarchar(64) default 'active',
	constraint FK_scms_navigation_subnav_plugin_instance foreign key (instanceId) references scms_plugin_module_instance(id)
)

create table scms_navigation_pagedetail
(
	id int primary key not null identity,
	instanceId int not null,
	wrapDetailInHtmlElement bit not null,
	wrapElementType nvarchar(64),
	cssClassWrap nvarchar(64),
	detailType nvarchar(64) -- title, linktext
	constraint FK_scms_navigation_pagedetail_plugin_instance foreign key (instanceId) references scms_plugin_module_instance(id)
)


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
	ascending bit null default 1,
	-- order by ?
	constraint FK_scms_navigation_list_plugin_instance foreign key (instanceId) references scms_plugin_module_instance(id) on delete cascade,
	constraint FK_scms_navigation_list_page foreign key (rootPageId) references scms_page(id),
	constraint FK_scms_navigation_list_page2 foreign key (listReadMorePageId) references scms_page(id)
)	

insert scms_plugin_application
(
	[name],
	[description],
	controlPathSettings
)
values
(
	'navigation',	-- [name] 
	'Place navigation elements on web pages.',
	'/scms/modules/navigation/settings.ascx' -- controlPathSettings nvarchar(1024) null
)
declare @appid_navigation int
select @appid_navigation = scope_identity()


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
	@appid_navigation,
	'sitemap',
	'displays sitemap',
	'/scms/modules/navigation/sitemap/edit.ascx',
	'/scms/modules/navigation/sitemap/view.ascx'
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
	@appid_navigation,
	'subnav',
	'displays secondary navigation menu',
	'/scms/modules/navigation/subnav/edit.ascx',
	'/scms/modules/navigation/subnav/view.ascx'
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
	@appid_navigation,
	'breadcrumbs',
	'displays breadcrumbs',
	'/scms/modules/navigation/breadcrumbs/edit.ascx',
	'/scms/modules/navigation/breadcrumbs/view.ascx'
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
	@appid_navigation,
	'pagedetail',
	'displays detail item from page: title, linktext, etc.',
	'/scms/modules/navigation/pagedetail/edit.ascx',
	'/scms/modules/navigation/pagedetail/view.ascx'
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
	@appid_navigation,
	'pagelist',
	'displays list of pages',
	'/scms/modules/navigation/pagelist/edit.ascx',
	'/scms/modules/navigation/pagelist/view.ascx'
)

create table scms_form
(
	id int identity primary key not null,
	siteid int not null,
	name nvarchar(64) not null,
	thankYouPageId int null,

	-- form generation
	generateForm bit not null,
	generationType nvarchar(64) null, -- table, div, manual
	submitText nvarchar(64) null,
	cssClassContainer nvarchar(64) null,
	cssClassTable nvarchar(64) null,
	cssClassRow nvarchar(64) null,
	cssClassCellLabel nvarchar(64) null,
	cssClassCellValue nvarchar(64) null,
	cssClassInputRow nvarchar(64) null,
	manualFormText ntext null,
	-- spam protection
	validateDummyFields bit not null,
	validateSession bit not null,
	validateReferrer bit not null,
	validateReCaptcha bit not null,

	-- notification
	notify bit not null,
	notifyEmail nvarchar(256) null,
	post bit not null,
	postUrl nvarchar(1024) null,

	

	deleted bit not null,

	constraint FK_scms_form_site foreign key (siteid) references scms_site(id),
	constraint FK_scms_form_page foreign key (thankYouPageId) references scms_page(id)
)

create table scms_form_field
(
	id int identity primary key not null,
	formid int not null,
	name nvarchar(64) not null,
	ordinal int not null,
	label nvarchar(1024) null,
	type nvarchar(32) not null, -- 'text', 'textarea', 'checkbox', 'radiolist', 'checkboxlist', 'hidden'
	-- auto:ipaddress
	-- auto:url
	-- auto:referrer
	

	-- default
	defaultText nvarchar(256) null,
	defaultChecked bit null,

	width int null, -- style
	maxlength int null, -- text
	cols int null, -- textarea
	rows int null, -- textarea
	fileTypes nvarchar(128) null, -- file


	-- post
	post bit null,
	postId nvarchar(64) null,
	
	-- validation
	required bit not null,
	validationRegex nvarchar(256) null,
	validationErrorMessage nvarchar(256) null, 
	
	-- style overrides
	cssClassOverrideRow nvarchar(64) null,
	cssClassOverrideCellLabel nvarchar(64) null,
	cssClassOverrideCellValue nvarchar(64) null,

	deleted bit not null,
	constraint FK_scms_form_field_form foreign key (formid) references scms_form(id)
)


create table scms_form_field_option
(
	id int identity primary key not null,
	fieldid int not null,
	name nvarchar(1024),
	value nvarchar(32),
	ordinal int not null,
	deleted bit not null,
	constraint FK_scms_form_field_option_field foreign key (fieldid) references scms_form_field(id)
)
create index IX_scms_form_field_option on scms_form_field_option(id,fieldid)

create table scms_form_instance
(
	id int identity primary key not null,
	formid int not null,
	instanceId int not null,
	constraint FK_scms_form_instance_form foreign key (formid) references scms_form(id),
	constraint FK_scms_form_instance_instance foreign key (instanceId) references scms_plugin_module_instance(id)
)
create index IX_scms_form_instance on scms_form_instance(id, formid, instanceid)

create table scms_form_submission
(
	id int identity primary key not null,
	formid int not null,
	forminstanceid int not null,
	submissionTime datetime not null,
	deleted bit not null, 

	constraint FK_scms_form_submission_form foreign key (formid) references scms_form(id),
	constraint FK_scms_form_submission_form_instance foreign key (forminstanceid) references scms_form_instance(id)
)
create index IX_form_submission on scms_form_submission (id,formid,forminstanceid)
create index IX_form_submission_dated on scms_form_submission (id,submissionTime)

create table scms_form_submission_fieldvalue
(
	id int identity primary key not null,
	formid int not null,
	formsubmissionid int not null,
	fieldid int not null,
	value nvarchar(1024),

	constraint FK_scms_form_submission_fieldvalue_form foreign key (formid) references scms_form(id),
	constraint FK_scms_form_submission_fieldvalue_formsubmission foreign key (formsubmissionid) references scms_form_submission(id),
	constraint FK_scms_form_submission_fieldvalue_field foreign key (fieldid) references scms_form_field(id)
)
create index IX_form_submission_fieldvalue on scms_form_submission_fieldvalue(id,formid,formsubmissionid)

create table scms_form_submission_optionfieldvalue
(
	id int identity primary key not null,
	formid int not null,
	formsubmissionid int not null,
	fieldid int not null,
	value nvarchar(1024),
	constraint FK_scms_form_submission_optionfieldvalue_form foreign key (formid) references scms_form(id),
	constraint FK_scms_form_submission_optionfieldvalue_formsubmission foreign key (formsubmissionid) references scms_form_submission(id),
	constraint FK_scms_form_submission_optionfieldvalue_field foreign key (fieldid) references scms_form_field(id)
)
create index IX_form_submission_optionfieldvalue on scms_form_submission_optionfieldvalue(id,formid,formsubmissionid)

create table scms_form_eventhandler
(
	id int not null identity primary key,
	formid int not null,
	eventName nvarchar(256) not null,
	ordinal int not null,
	deleted bit not null,
	constraint fk_form_eventhandler_form foreign key (formid) references scms_form(id)
)

insert scms_plugin_application
(
	[name],
	[description],
	controlPathSettings
)
values
(
	'forms',	-- [name] 
	'Place dynamic forms in content.',
	'/scms/modules/forms/settings.ascx' -- controlPathSettings nvarchar(1024) null
)
declare @appid_forms int
select @appid_forms = scope_identity()


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
	@appid_forms,
	'form',
	'displays form',
	'/scms/modules/forms/form/edit.ascx',
	'/scms/modules/forms/form/view.ascx'
)

insert scms_plugin_application
(
	name,
	description,
	controlPathSettings
)
values
(
	'system events',
	'View application status and error logs.',
	'/scms/modules/systemevents/settings.ascx'
)
declare @appid_events int
select @appid_events = scope_identity()


/* search */

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

/* security */
insert scms_plugin_application
(
	[name],
	[description],
	controlPathSettings
)
values
(
	'security',	-- [name] 
	'User acess controls.',
	'/scms/modules/security/settings.ascx' -- controlPathSettings nvarchar(1024) null
)
go


declare @appid int
select @appid = scope_identity()

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
	'login',
	'enable user to login',
	'/scms/modules/security/login/edit.ascx',
	'/scms/modules/security/login/view.ascx'
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
	@appid,
	'welcome',
	'show welcome message to user, enable logout',
	'/scms/modules/security/welcome/edit.ascx',
	'/scms/modules/security/welcome/view.ascx'
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
	@appid,
	'password',
	'permits logged in user to change password',
	'/scms/modules/security/password/edit.ascx',
	'/scms/modules/security/password/view.ascx'
)
go


if exists (select [id] from sysobjects where xtype = 'u' and [name] = 'scms_slideshow_slide' )
	drop table scms_slideshow_slide
go

if exists (select [id] from sysobjects where xtype = 'u' and [name] = 'scms_slideshow' )
	drop table scms_slideshow
go

create table scms_slideshow
(
	id int not null identity primary key,
	instanceId int not null,
	type nvarchar(32) not null,
	headerTemplate ntext,
	footerTemplate ntext,
	itemTemplate ntext,
	headerScript ntext,
	width int null,
	height int null,
	transitionType nvarchar(32) not null,
	transitionTimeMs int null,
	pauseTimeMs int null,
	hasSelectorButtons bit null,
	random bit not null,
	hoverPause bit not null,
	clickAdvance bit not null,
	showPager bit not null,
	pagerLocation nvarchar(32) null,
	pagerClass nvarchar(32) null,
	pagerType nvarchar(32) null,
	pageThumbnailWidth int null,
	pagerThumbnailHeight int null,
	constraint FK_scms_slideshow_instance foreign key (instanceId) references scms_plugin_module_instance(id)
)

create table scms_slideshow_slide
(
	id int not null identity primary key,
	slideShowId int not null,
	ordinal int not null,
	name nvarchar(32) null,
	heading nvarchar(256) null,
	imageUrl nvarchar(1024) null,
	linkUrl nvarchar(1024) null,
	content ntext null,
	customOverride ntext null,
	constraint FK_scms_slideshow_slide_slideshow foreign key (slideShowId) references scms_slideshow(id)
)
go


/* slideshow */
insert scms_plugin_application
(
	[name],
	[description],
	controlPathSettings
)
values
(
	'slideshow',	-- [name] 
	'Slideshow controls.',
	'/scms/modules/slideshow/settings.ascx' -- controlPathSettings nvarchar(1024) null
)


declare @appid int
select @appid = scope_identity()

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
	'slideshow',
	'slideshow module',
	'/scms/modules/slideshow/slideshow/edit.ascx',
	'/scms/modules/slideshow/slideshow/view.ascx'
)
go

/* submissions */

create table scms_submission_module
(
	id int not null identity primary key,
	instanceId int not null,
	
	autoApproveSubmission bit not null,
	autoFeatureSubmission bit not null,
	
	-- comments
	commentsEnabled bit not null,
	commentsAuthenticationRequired bit null,
	commentsAutoApprove bit null,
	
	-- voting
	votingEnabled bit not null,
	votingAuthenticationRequired bit null,
	votingMethod nvarchar(32) null,
	-- UpDown
	votingUpImageUrl nvarchar(1024) null,
	votingDownImageUrl nvarchar(1024) null,
	votingUpText nvarchar(256) null,
	votingDownText nvarchar(256) null,
	-- FiveUp
	votingActiveImageUrl nvarchar(1024) null,
	votingInActiveImageUrl nvarchar(1024) null,
	votingEvenImageUrl nvarchar(1024) null,
	votingSelectText nvarchar(1024) null,
	
	-- fields
	titleEnabled bit not null,
	titleRequired bit null,
	titleCssClass nvarchar(128) null,
	
	imageEnabled bit not null,
	imageRequired bit null,
	imageCssClass nvarchar(128) null,
	imageWidth int null,
	imageHeight int null,
	
	videoEnabled bit not null,
	videoRequired bit null,
	videoCssClass nvarchar(128) null,
	
	descriptionEnabled bit not null,
	descriptionRequired bit null,
	descriptionCssClass nvarchar(128) null,
	
	linkEnabled bit not null,
	linkRequired bit null,
	linkCssClass nvarchar(128) null,
	linkText nvarchar(256) null,
	
	emailAddressEnabled bit not null,
	emailAddressRequired bit null,
	emailAddressCssClass nvarchar(128) null,
	
	userIdEnabled bit not null,
	userIdRequired bit null,
	userIdCssClass nvarchar(128) null,
	
	submitterEnabled bit not null,
	submitterRequired bit null,
	submitterCssClass nvarchar(128) null,
	
	documentCreditEnabled bit not null,
	documentCreditRequired bit null,
	documentCreditCssClass nvarchar(128) null,
	
	constraint FK_scms_submission_module_module foreign key (instanceid) references scms_plugin_module_instance(id)
)
go


create table scms_submission_form_eventhandler
(
	id int not null identity primary key,
	formId int not null,
	eventHandlerId int not null,
	submissionModuleId int not null,
	titleFieldId int null,
	imageFieldId int null,
	videoFieldId int null,
	linkFieldId int null,
	descriptionFieldId int null,
	emailAddressFieldId int null,
	userIdFieldId int null,
	submitterFieldId int null,
	documentCreditFieldId int null,
	constraint FK_scms_submission_form_eventhandler_form foreign key (formId) references scms_form (id),
	constraint FK_scms_submission_form_eventhandler_form_eventhandler foreign key (eventHandlerId) references scms_form_eventhandler(id),
	constraint FK_scms_submission_form_eventhandler_submission foreign key (submissionModuleId) references scms_submission_module(id),
	constraint FK_scms_submission_form_eventhandler_titleField foreign key (titleFieldId) references scms_form_field(id),
	constraint FK_scms_submission_form_eventhandler_imageField foreign key (imageFieldId) references scms_form_field(id),
	constraint FK_scms_submission_form_eventhandler_videoField foreign key (videoFieldId) references scms_form_field(id),
	constraint FK_scms_submission_form_eventhandler_linkField foreign key (linkFieldId) references scms_form_field(id),
	constraint FK_scms_submission_form_eventhandler_descriptionField foreign key (descriptionFieldId) references scms_form_field(id),
	constraint FK_scms_submission_form_eventhandler_emailAddressField foreign key (emailAddressFieldId) references scms_form_field(id),
	constraint FK_scms_submission_form_eventhandler_userIdField foreign key (userIdFieldId) references scms_form_field(id)
)
go

create table scms_submission_submission
(
	id int not null identity primary key,
	submissionModuleId int not null,
	deleted bit not null,
	formSubmissionId int not null,
	submissionTime datetime not null,
	approvedTime datetime null,
	featuredTime datetime null,
	
	-- comments
	-- voting
	votes int null,
	votesUp int null,
	votesDown int null,
	votes1 int null,
	votes2 int null,
	votes3 int null,
	votes4 int null,
	votes5 int null,
	vote decimal(18,2) null,
	
	-- fields
	title nvarchar(256) null,
	imageUrl nvarchar(1024) null,	
	videoUrl nvarchar(1024) null,
	linkUrl nvarchar(1024) null,
	[description] nvarchar(1024) null,
	emailAddress nvarchar(1024) null,
	userId nvarchar(128) null,
	submitter nvarchar(128) null,
	documentCredit nvarchar(128) null
	
	constraint FK_scms_submission_module foreign key (submissionModuleId) references scms_submission_module(id),
	constraint FK_scms_submission_form_submission foreign key (formSubmissionId) references scms_form_submission(id)
	
)
go


insert scms_plugin_application
( name, description, controlPathSettings, type )
values
( 'submission', 'Enable content submissions', '/scms/modules/submission/setting.ascx', 'scms.modules.submission.submissionApplication' )
go


declare @appId int
select @appId = [id] 
from scms_plugin_application
where name = 'submission'

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
	'submission',
	'Accept and manage submissions',
	'/scms/modules/submission/submission/edit.ascx',
	'/scms/modules/submission/submission/view.ascx'
)
go


commit
go

