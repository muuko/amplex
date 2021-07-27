begin tran
select @@trancount
go

alter table cat_part
add sage_ProductCategoryDesc1 nvarchar(11) null

alter table cat_part
add sage_ProductCategoryDesc2 nvarchar(11) null

alter table cat_part
add sage_ProductCategoryDesc3 nvarchar(11) null

alter table cat_part
add sage_ProductCategoryDesc4 nvarchar(11) null

alter table cat_part
add sage_ProductCategoryDesc5 nvarchar(11) null

create table cat_size
(
  id nvarchar(11) not null primary key,
  name nvarchar(32) null,
  ordinal int not null
)
go

create index IX_cat_size_ordinal on cat_size( id, ordinal )
go

alter table cat_part 
add constraint FK_cat_part_size 
foreign key (sage_ProductCategoryDesc2) references cat_size(id)
go

commit

/*

begin tran
select @@trancount
update cat_part set hash = null where sage_productCategoryDesc1 is not null
commit

select * from cat_part
update cat_part set hash = null where hash = 'ZnUlaWI18Ht60Ic0a14jgz99IxDwQ2d4k2l9pTiLckg='
select * from cat_size

select s.id, s.ordinal, p.sage_id, p.sage_description1, p.sage_ProductCategoryDesc1, p.sage_ProductCategoryDesc2
from cat_part p
left outer join cat_size s
on p.sage_ProductCategoryDesc2 = s.id
order by coalesce(sage_ProductCategoryDesc1, sage_Description1)

select * from cat_part order by sage_ProductCategoryDesc1 
select * from cat_part order by coalesce(sage_ProductCategoryDesc1, sage_Description1)

select s.id, s.ordinal, p.* 
from cat_part p
left outer join cat_size s
on p.sage_productCategoryDesc2 = s.id
order by coalesce(sage_ProductCategoryDesc1, sage_Description1), s.ordinal




*/