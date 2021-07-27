begin tran
select @@trancount
go

update scms_plugin_application set type = 'scms.modules.forms.formsApplication' where name = 'forms'

if exists ( select [id] from sysobjects where xtype = 'u' and name = 'scms_forms_autoresponder_eventhandler' )
 drop table scms_forms_autoresponder_eventhandler
go

create table scms_forms_autoresponder_eventhandler
(
  id int not null primary key identity,
  formid int not null,
  eventHandlerId int not null,
  [from] nvarchar(1024) null,
  cc nvarchar(1024) null,
  bcc nvarchar(1024) null,
  [subject] nvarchar(256) not null,
  [body] nvarchar(max) not null,
  bodyIsHtml bit not null,
  emailAddressFieldId int not null,
  constraint FK_scms_forms_ar_eh_form foreign key (formid) references scms_form(id),
  constraint FK_scms_forms_ar_eh_eventHander foreign key (eventHandlerId) references scms_form_eventhandler(id)
)

commit
