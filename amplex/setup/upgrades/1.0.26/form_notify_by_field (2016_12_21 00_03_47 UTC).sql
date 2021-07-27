begin transaction
select @@trancount
go

if exists (select [id] from sysobjects where xtype=  'u' and name = 'scms_form_notify_by_field')
drop table scms_form_notify_by_field
go

create table scms_form_notify_by_field
(
	id int identity not null primary key,
	formId int not null,
	fieldId int not null,
	value nvarchar(64) not null,
	email nvarchar(256) not null,
	constraint FK_scms_form_notify_by_field_form foreign key (formId) references scms_form(id),
	constraint FK_scms_form_notify_by_field_field foreign key (fieldId) references scms_form_field(id)
)
go

commit



/* select * from scms_form_notify_by_field */