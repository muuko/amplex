begin tran
select @@trancount
go
-- commit
-- rollback

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

/*
drop table scms_slideshow_pager
create table scms_slideshow_pager
(
	id int not null identity primary key,
	instanceId int not null,
	slideShowId int not null,
	constraint FK_scms_slideshow_pager_instance foreign key (instanceId) references scms_plugin_module_instance(id),
	constraint FK_scms_slideshow_pager_slideshow foreign key (slideShowId) references scms_slideshow(id) 
)
*/
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

commit

