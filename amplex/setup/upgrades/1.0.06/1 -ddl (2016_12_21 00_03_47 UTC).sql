if exists ( select [id] from sysobjects where [name] = 'scms_user' and xtype = 'u' )
	drop table scms_user
go

create table scms_user
(
	userid uniqueidentifier primary key not null,
	firstName nvarchar(128),
	lastName nvarchar(128),
	constraint FK_scms_user_aspnet_users foreign key (userid) references aspnet_users(UserId) on delete cascade 
)
go

begin tran
select @@trancount

alter table scms_page
add securityInherit bit not null default 1
go

alter table scms_page
add securityLoginRequired bit not null default 0
go

alter table scms_page
add securityRestrictToRoles bit not null default 0
go


if exists ( select [id] from sysobjects where [name] = 'scms_page_role' and xtype = 'u' )
	drop table scms_page_role
go

create table scms_page_role
(
	pageid int not null,
	RoleId uniqueidentifier not null,
	constraint scms_page_role_pk primary key (pageid, roleid),
	constraint scms_page_role_page foreign key (pageid) references scms_Page(id),
	constraint scms_page_role_role foreign key (roleid) references aspnet_roles(roleid) on delete cascade
)
go

if exists ( select [id] from sysobjects where [name] = 'scms_security_settings' and xtype = 'u' )
	drop table scms_security_settings
go

create table scms_security_settings
(
	siteid int not null primary key,
	pageIdLogin int null,
	constraint scms_security_settings_fk_site foreign key (siteid) references scms_site(id)
)
go

commit

