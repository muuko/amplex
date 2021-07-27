begin tran
select @@trancount -- rollback
go

if exists ( select [id] from sysobjects where xtype = 'u' and name = 'scms_org_attr_value' )
	drop table scms_org_attr_value
go

if exists ( select [id] from sysobjects where xtype = 'u' and name = 'scms_org_attr' )
	drop table scms_org_attr
go

if exists ( select [id] from sysobjects where xtype= 'F' and name = 'fk_user_org' )
	alter table scms_user drop constraint fk_user_org
go

if exists ( select [id] from sysobjects where xtype = 'u' and name = 'scms_org' )
	drop table scms_org
go


create table scms_org
(
	id int not null identity primary key,
	[name] nvarchar(256),
	deleted bit not null default 0
)
go


create table scms_org_attr
(
	id int not null identity primary key,
	[name] nvarchar(32)
)
go

create table scms_org_attr_value
(
	orgId int not null,
	attrId int not null,
	value nvarchar(max),
	constraint pk_org_attr_value primary key (orgId, attrId),
	constraint fk_org_attr_value_org foreign key (orgId) references scms_org(id),
	constraint fk_org_attr_value_attr foreign key (attrId) references scms_org_attr(id)
)
go

alter table scms_user
add orgId int null
go


alter table scms_user 
add constraint fk_user_org 
foreign key (orgId) 
references scms_org(id)
go




-- sp_help pk_org_users
-- select * from sysobjects where name = 'pk_orginization_users'

/*
create table registration_pending
(
	userId  uniqueidentifier not null,
	[key] uniqueidentifier not null,
	dtCreated DateTime not null default(getdate()),
	constraint fk_registration_pending_user foreign key (userId) references aspnet_users(UserId) on delete cascade,
	constraint pk_registration_pending primary key (userId,[key]) 
)
go

create table user_attrs
(
	userId uniqueidentifier not null primary key,
	emailValidated bit not null default 0,
	constraint fk_user_attrs_user foreign key (userId) references aspnet_users(UserId) on delete cascade,
)
go
*/

commit

/*
test data

use projectz
select * from scms_org_attr
insert scms_org_attr ( name ) values ( 'customer-id' )

select * from scms_org
insert scms_org ( name ) values ( 'test-org' )

declare @orgId int
select @orgId = id from scms_org where name = 'test-org' 
select '@orgId', @orgId

declare @orgAttrId_CustomerId int
select @orgAttrId_CustomerId = id from scms_org_attr where name = 'customer-id'
select '@orgAttrId_CustomerId', @orgAttrId_CustomerId

*/
