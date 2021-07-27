begin tran
select @@trancount
go

if exists ( select [id] from sysobjects where xtype = 'p' and [name] = 'cat_part_search' )
	drop procedure cat_part_search
go

create procedure cat_part_search
(
	@searchText nvarchar(128),
	@searchInDescription1 bit,
	@searchInDescription2 bit,
	@searchInDescription3 bit,
	@searchInLongDescription bit,
	@orderBy nvarchar(32), -- common, botanical, description
	@powerSearch bit,
	@pageSize int,
	@pageNumber int,
	@count int out
)
as

begin

	set @count = 0
	
	declare @tblMatching table
	(
		id int not null primary key
	)
	
	declare @searchPrefix varchar(1)
	if @powerSearch = 1
	begin
		set @searchPrefix = '%'
	end
	else
	begin
		set @searchPrefix = ''
	end
	
	declare @tblSearch table
	(
		searchText varchar(200)
	)
	
	
	insert @tblSearch select value from scms_fn_CommaSeparatedStringToTable(@searchText,1)
	
	declare searchCursor cursor 
	for select searchText from @tblSearch
	open searchCursor
	declare @searchTextCurrent varchar(200)
	fetch next from searchCursor into @searchTextCurrent
	while @@fetch_status <> -1
	begin
		if @@fetch_status <> -2
		begin
			print @searchTextCurrent

			if @searchInDescription1 = 1
			begin
				insert @tblMatching
				select id from cat_part
				where sage_Description1 like @searchPrefix + isnull(@searchTextCurrent,'') + '%'
				and id not in (select id from @tblMatching)
			end

			if @searchInDescription2 = 1
			begin
				insert @tblMatching
				select id from cat_part
				where sage_Description2 like @searchPrefix + isnull(@searchTextCurrent,'') + '%'
				and id not in (select id from @tblMatching)
			end

			if @searchInDescription3 = 1
			begin
				insert @tblMatching
				select id from cat_part
				where sage_Description3 like @searchPrefix + isnull(@searchTextCurrent,'') + '%'
				and id not in (select id from @tblMatching)
			end

			if @searchInLongDescription = 1
			begin
				insert @tblMatching
				select id from cat_part
				where sage_LongDescription like @searchPrefix + isnull(@searchTextCurrent,'') + '%'
				and id not in (select id from @tblMatching)
			end
			
			fetch next from searchCursor into @searchTextCurrent			
		end
		

	end
	close searchCursor
	deallocate searchCursor



	declare @tblMatchingOrdered table
	(
		id int not null primary key,
		ordinal int not null identity(0,1)
	)
	
	if isnull(@orderBy, 'description1') = 'description1' 
	begin
		insert @tblMatchingOrdered
		select tmp.id from @tblMatching tmp
		join cat_part p
		on tmp.id = p.id
		left outer join cat_size s
		on p.sage_productCategoryDesc2 = s.id
		order by coalesce(p.sage_ProductCategoryDesc1, p.sage_Description1), s.ordinal
	end
	
	else if @orderBy = 'description2' 
	begin
		insert @tblMatchingOrdered
		select tmp.id from @tblMatching tmp
		join cat_part
		on tmp.id = cat_part.id
		order by cat_part.sage_description2
	end
	
	else if @orderBy = 'description3' 
	begin
		insert @tblMatchingOrdered
		select tmp.id from @tblMatching tmp
		join cat_part
		on tmp.id = cat_part.id
		order by cat_part.sage_description3
	end

	else if @orderBy = 'longDescription' 
	begin
		insert @tblMatchingOrdered
		select tmp.id from @tblMatching tmp
		join cat_part
		on tmp.id = cat_part.id
		order by cat_part.sage_LongDescription
	end

	
	declare @startIndex int
	set @startIndex = 0
	
	declare @endIndex int
	select @endIndex =max([ordinal]) 
	from @tblMatchingOrdered

	if isnull(@pageSize,0) > 0 
	begin
		if isnull(@pageNumber,0) > 0 
		begin
			set @startIndex = @pageNumber * @pageSize
		end
		
		set @endIndex = @startIndex + @pageSize - 1
	end
	
	select @count = count([id]) from @tblMatchingOrdered
	
	select cp.* 
	from @tblMatchingOrdered tmp, cat_part cp
	where tmp.id = cp.id
	and tmp.ordinal >= @startIndex
	and tmp.ordinal <= @endIndex
	order by tmp.ordinal

end
go
commit