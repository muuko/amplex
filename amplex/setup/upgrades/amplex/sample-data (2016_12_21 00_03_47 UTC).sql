delete cat_part

insert cat_part (sage_skicpart, sage_id, hash, lastUpdated, sage_Description1) values (1, 'one', null, getdate(), 'this is one')
insert cat_part (sage_skicpart, sage_id, hash, lastUpdated, sage_Description1) values (2, 'two', null, getdate(), 'this is two')
insert cat_part (sage_skicpart, sage_id, hash, lastUpdated, sage_Description1) values (3, 'three', null, getdate(), 'this is three')


select * from cat_part
