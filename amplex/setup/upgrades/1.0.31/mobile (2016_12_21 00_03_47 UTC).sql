begin tran
select @@TRANCOUNT --rollback
go


insert scms_site ( name, xmlSitemapEnabled, xmlSitemapLocation, filesLocation ) values ( 'mobile', 1, '/sitemap.xml', '/sites/mobile/' )
declare @siteId int
select @siteid = scope_identity()

insert scms_master ( siteid, name, path ) values ( @siteid, 'basic', '/sites/mobile/_masters/BasicMaster.master' )
declare @masterId int 
select @masterId = scope_identity()

insert scms_template ( siteid, name, masterId ) values ( @siteid, 'basic', @masterId )
declare @templateId int 
select @templateId = scope_identity()

update scms_site 
set defaultTemplateId = @templateId
where id = @siteid
commit
go

begin tran
select @@TRANCOUNT
declare @siteid int
select @siteId = id from scms_site where name='mobile'

declare @masterid int
select @masterId = id from scms_master where siteId = @siteId and name = 'basic'

declare @templateId int
select @templateId = id from scms_template where siteid= @siteid and name = 'basic'

select @siteId, @masterId, @templateId

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

insert scms_config 
(
	name,
	value
) 
values 
( 
	'full-site-url', 
	'http://merlin.dev.coinbug.com/?full=1' 
)

commit
go

begin tran
select @@trancount --rollback

declare @siteId int
select @siteid = id from scms_site where name = 'mobile'
select @siteid '@siteid'

insert scms_master ( siteid, name, path ) values ( @siteid, 'subpage', '/sites/mobile/_masters/SubpageMaster.master' )
declare @masterid int
select @masterId = id from scms_master where name = 'subpage'
select @masterid  '@masterid'

insert scms_template ( siteid, name, masterId ) values ( @siteid, 'subpage', @masterId )
declare @templateId int 
select @templateId = scope_identity()
select @templateId '@templateId'

update scms_site
set defaultTemplateId = @templateId
where id = @siteId



select * from scms_site

commit

-- select * from scms_config
-- 
update scms_config set value = 'http://merlinlawgr.web711.discountasp.net/?full=1'  where name = 'full-site-url'