begin tran
select @@trancount
go

alter table scms_page
add viewStateEnabled bit null
go

commit