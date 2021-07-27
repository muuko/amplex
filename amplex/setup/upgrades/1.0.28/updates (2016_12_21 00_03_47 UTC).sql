-- mobile
if exists ( select [id] from sysobjects where xtype='u' and name ='scms_mobileRedirect' )
  drop table scms_mobileRedirect
go

create table scms_mobileRedirect
(
 siteId int not null primary key,
 [enabled] bit not null default 0,
 url nvarchar(1024),
 appendPath bit not null default 0,
 appendQueryString bit not null default 0,
 mobileToFullQueryString nvarchar(64) not null,
 constraint fk_scms_mobileRedirect_site foreign key (siteId) references scms_site(id)
)
go


-- insert scms_config (name,value) values ('debug-force-mobile','true')
-- update scms_config set value = 'false' where name = 'debug-force-mobile'