begin tran
select @@trancount
go

delete scms_config where name like 'welcome-email%'

insert scms_config( name, value ) values ( 'welcome-email-send', 'true' )
insert scms_config( name, value ) values ( 'welcome-email-from', 'amplexinc@gmail.com' )
insert scms_config( name, value ) values ( 'welcome-email-bcc', 'amplexinc@gmail.com,jwoodlock@yahoo.com' )
insert scms_config( name, value ) values ( 'welcome-email-subject', 'Welcome to Amplex' )


declare @crlf nvarchar(32)
set @crlf = char(13) + char(10)

insert scms_config( name, value ) values ( 'welcome-email-body', 
'Thank you for your request.  Your Amplex Login information is below:' + @crlf +
'' + @crlf +
'Username: ##USER##' + @crlf +
'Password: ##PASSWORD##' + @crlf +
'' + @crlf +
'If you have any problems, please inform us at amplexinc@gmail.com.' + @crlf +
'' + @crlf +
'Thank you again and have a great day!' + @crlf +
@crlf +
'http://aboutamplex.com' + @crlf
)

select * from scms_config where name like 'welcome%'


commit