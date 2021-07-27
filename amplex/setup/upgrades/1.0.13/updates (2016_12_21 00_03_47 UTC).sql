begin tran
select @@trancount
go

alter table scms_security_settings
add	requireUserEmailValidation bit not null
go

alter table scms_security_settings
add userEmailValidationPageId int null

alter table scms_security_settings
add constraint scms_security_settings_fk_regpage 
foreign key (userEmailValidationPageId) 
references scms_page(id)
go

alter table scms_navigation_pagelist
add ascending bit null default 1
go

alter table scms_site
add canonicalHostName nvarchar(1024) null
go

alter table scms_user
add emailvalidated bit not null default 1
go


commit