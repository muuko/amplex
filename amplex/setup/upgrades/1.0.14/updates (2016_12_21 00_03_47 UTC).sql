begin tran
select @@trancount
go

alter table scms_site
add cacheEnabled bit null

alter table scms_site
add cacheControl nvarchar(64) null

alter table scms_site
add cacheExpiresSeconds int null

alter table scms_site
add cacheMaxAgeSeconds int null

commit