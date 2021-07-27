/* 
	release notes

version 1.0.1
	
1 - subnav mods including pin depth
*/

begin tran
select @@trancount
go


alter table scms_navigation_subnav
add pinDepth int null


commit

