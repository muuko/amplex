begin tran
select @@trancount
go

alter table scms_form
add summaryValidationEnabled bit not null default 0


alter table scms_form
add summaryValidationCssClass nvarchar(64) null


alter table scms_form
add summaryValidationHeaderText nvarchar(128) null

alter table scms_form
add summaryValidationDisplayMode nvarchar(32) null
			
alter table scms_form
add summaryValidationEnableClientScript bit null

alter table scms_form
add summaryValidationShowSummary bit null

alter table scms_form
add summaryValidationShowMessageBox bit null


alter table scms_form_field
add validationDisplay nvarchar(32) null

commit