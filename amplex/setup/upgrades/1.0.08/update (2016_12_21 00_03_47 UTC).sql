begin tran
select @@trancount
go

alter table scms_plugin_application
add [type] nvarchar(1024) null
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





if exists ( select [id] from sysobjects where [name] = 'scms_submission_form_eventhandler' and xtype = 'u' )
 drop table scms_submission_form_eventhandler
go

if exists ( select [id] from sysobjects where [name] = 'scms_form_eventhandler' and xtype = 'u' )
 drop table scms_form_eventhandler
go

if exists ( select [id] from sysobjects where [name] = 'scms_submission_submission' and xtype = 'u' )
 drop table scms_submission_submission
go

if exists ( select [id] from sysobjects where [name] = 'scms_submission_module' and xtype = 'u' )
 drop table scms_submission_module
go


create table scms_form_eventhandler
(
	id int not null identity primary key,
	formid int not null,
	eventName nvarchar(256) not null,
	ordinal int not null,
	deleted bit not null,
	constraint fk_form_eventhandler_form foreign key (formid) references scms_form(id)
)
go





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

alter table scms_form_field
add fileTypes nvarchar(128) null
go

commit

