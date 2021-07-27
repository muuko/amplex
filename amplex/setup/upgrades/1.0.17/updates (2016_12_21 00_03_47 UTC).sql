begin tran
select @@trancount
go

alter table scms_page
add securityProtocolForce bit null

alter table scms_page
add securityProtocolSecure bit null
`
alter table scms_security_settings
add sslPublicEnabled bit null


go
commit