sp_help scms_form_field
begin tran
select @@trancount
go

alter table scms_form_field
add repeatColumns int null
go

alter table scms_form_field
add repeatDirection nvarchar(64)
go

alter table scms_form_field
add repeatLayout nvarchar(16)
go

alter table scms_form_field
add redactInEmail bit null
go

alter table scms_form_field
add requiredText nvarchar(256)
go

alter table scms_form_field
alter column defaultText nvarchar(max) null
go

alter table scms_form_submission_fieldvalue
alter column [value] nvarchar(max) null
go

commit

/*
select * from scms_form_field