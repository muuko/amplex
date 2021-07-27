begin tran -- rollback
select @@trancount
go

-- specials
/*
if exists ( select [id] from sysobjects where [name] = 'scms_special_special' and xtype = 'u' )
 drop table scms_special_special
*/ 
go


if exists ( select [id] from sysobjects where [name] = 'scms_special_form_eventhandler' and xtype = 'u' )
 drop table scms_special_form_eventhandler
go


/*
if exists ( select [id] from sysobjects where [name] = 'scms_special_module' and xtype = 'u' )
 drop table scms_special_module
*/ 
go



/* specials */
/*
create table scms_special_module
(
	id int not null identity primary key,
	instanceId int not null,

	constraint FK_scms_special_module_module foreign key (instanceid) references scms_plugin_module_instance(id)
)
*/
go

create table scms_special_form_eventhandler
(
	id int not null identity primary key,
	formId int not null,
	eventHandlerId int not null,
	parentPageId int not null,
	templateId int null,
	titleFieldId int null,
	imageFieldId int null,
	descriptionFieldId int null,
	associatedDateFieldId int null,
	imageTranslate bit null,
	imageTranslationMode nvarchar(32),
	imageTranslationHeight int null,
	imageTranslationWidth int null,
	imageTranslationBackgroundColor int null,
	constraint FK_scms_special_form_eventhandler_form foreign key (formId) references scms_form (id),
	constraint FK_scms_special_form_eventhandler_form_eventhandler foreign key (eventHandlerId) references scms_form_eventhandler(id),
	constraint FK_scms_special_form_eventhandler_page foreign key (parentPageId)  references scms_page(id),
	constraint FK_scms_special_form_eventhandler_template foreign key (templateId) references scms_template(id),
	constraint FK_scms_special_form_eventhandler_titleField foreign key (titleFieldId) references scms_form_field(id),
	constraint FK_scms_special_form_eventhandler_imageField foreign key (imageFieldId) references scms_form_field(id),
	constraint FK_scms_special_form_eventhandler_descriptionField foreign key (descriptionFieldId) references scms_form_field(id)
)
go


insert scms_plugin_application
( name, description, controlPathSettings, type )
values
( 'special', 'Enable specials', '/scms/modules/special/setting.ascx', 'scms.modules.special.specialApplication' )
go

/*


declare @appId int
select @appId = [id] 
from scms_plugin_application
where name = 'special'

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
	'special',
	'Accept and manage specials',
	'/scms/modules/special/special/edit.ascx',
	'/scms/modules/special/special/view.ascx'
)
*/
go

/*
alter table scms_special_form_eventhandler
add imageTranslate bit null

alter table scms_special_form_eventhandler
add imageTranslationMode nvarchar(32)

alter table scms_special_form_eventhandler
add imageTranslationHeight int null

alter table scms_special_form_eventhandler
add imageTranslationWidth int null

alter table scms_special_form_eventhandler
add imageTranslationBackgroundColor int null
*/

commit
go

