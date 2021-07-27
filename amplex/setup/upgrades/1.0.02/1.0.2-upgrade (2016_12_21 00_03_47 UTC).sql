/* 
	release notes

version 1.0.2
	
1 - add system events plugin application
*/

begin tran
select @@trancount -- rollback
go

select * from scms_plugin_application
declare @pluginApplicationId int

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

select @pluginApplicationId = scope_identity()
select @pluginApplicationId '@pluginApplicationId'

insert scms_config values ( 'show-admin-edit-links', 'true' )

commit

