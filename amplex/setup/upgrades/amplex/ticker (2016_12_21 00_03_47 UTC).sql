begin tran
select @@trancount
go


create table ticker_entry
(
	id int identity not null primary key,
	label nvarchar(256) null,
	value nvarchar(1024) null,
	pageId int null,
	ordinal int not null,
	constraint fk_ticker_entry_page foreign key (pageId) references scms_page(id) on delete cascade
)
go

insert scms_plugin_application
( name, description, controlPathSettings, type )
values
( 'ticker', 'Ticker administration', '/scms/modules/ticker/settings.ascx', null )
go



commit