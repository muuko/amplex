begin tran
select @@TRANCOUNT -- rollback

update scms_config 
set value = 'http://amplex.com/?full=1' 
where name = 'full-site-url'
go

alter table scms_special_form_eventhandler
add siteId int null
go

alter table scms_special_form_eventhandler
add constraint fk_special_feh_site foreign key (siteId) references scms_site(id)


commit