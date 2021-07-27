begin tran
select @@trancount
go

alter table scms_navigation_pagelist
add [ascending] bit not null default 1
go

commit