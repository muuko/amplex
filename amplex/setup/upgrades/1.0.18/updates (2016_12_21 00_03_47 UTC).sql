--sp_help scms_navigation_pagelist
begin tran
select @@trancount
go

alter table scms_navigation_pagelist
add templateGroupHeaderHtml nvarchar(max)

alter table scms_navigation_pagelist
add templateGroupFooterHtml nvarchar(max)

alter table scms_navigation_pagelist
add groupingEnabled bit null

alter table scms_navigation_pagelist
add groupingItemsPerGroup int null

commit