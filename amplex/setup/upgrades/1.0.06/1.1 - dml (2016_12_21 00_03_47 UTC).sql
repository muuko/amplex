begin tran
select @@trancount

insert scms_plugin_application
(
	[name],
	[description],
	controlPathSettings
)
values
(
	'security',	-- [name] 
	'User acess controls.',
	'/scms/modules/security/settings.ascx' -- controlPathSettings nvarchar(1024) null
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
	'login',
	'enable user to login',
	'/scms/modules/security/login/edit.ascx',
	'/scms/modules/security/login/view.ascx'
)

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
	'welcome',
	'show welcome message to user, enable logout',
	'/scms/modules/security/welcome/edit.ascx',
	'/scms/modules/security/welcome/view.ascx'
)


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
	'password',
	'permits logged in user to change password',
	'/scms/modules/security/password/edit.ascx',
	'/scms/modules/security/password/view.ascx'
)


/*
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
	'register',
	'user registration',
	'/scms/modules/security/welcome/edit.ascx',
	'/scms/modules/security/welcome/view.ascx'
)
*/

go


commit