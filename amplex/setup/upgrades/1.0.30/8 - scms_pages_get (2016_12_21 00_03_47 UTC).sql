if exists ( select [id] from sysobjects where xtype = 'p' and [name] = 'scms_pages_get' )
  drop procedure scms_pages_get
go

create procedure scms_pages_get
(
	@siteId int,
	@rootPageId int,
	@includeChildren bit,
	@hideParentNodes bit,
	@pageNumber int,
	@pageSize int,
	@ascending bit,
	@count int out
)
as
begin

	set @ascending = isnull(@ascending, 1)

	set @pageNumber = isnull(@pageNumber,0)

	declare @tblPages table
	(
		pageId int primary key not null,
		ordinal varchar(1024) default null,
		[set] int not null 
	)

	insert @tblPages 
	select id, 
		right( '00000' + convert(varchar(5),isnull(ordinal,0)), 5),
		0 from scms_page
	where (((parentId is null) and (@rootPageId is null)) or (parentId = @rootPageId))
	and siteId = @siteId
	and deleted = 0
	and visible = 1
	
	declare @rowcount int

	if @includeChildren = 1
	begin
		declare @set int
		set @set = 1
		
		declare @done bit
		set @done = 0
		while @done = 0
		begin
			print 'set: ' + convert(varchar(12), @set)
			insert @tblPages
			select	id, 
					tmp.ordinal + '.' + right( '00000' + convert(varchar(5),isnull(p.ordinal,0)), 5),
					@set
			from scms_page p, @tblPages tmp
			where p.parentId = tmp.pageid 
			and p.deleted = 0
			and visible = 1
			and tmp.[set] = @set - 1
			
			set @RowCount = @@RowCount
			
			if isnull(@RowCount,0) <= 0 
			begin
				set @done = 1
			end
			else
			begin
				set @set = @set + 1
			end
	
		end
	end

	
	if @hideParentNodes = 1
	begin
		declare @tblPagesChildCount table
		(
			pageId int primary key not null,
			childCount int null
		)
		
		insert @tblPagesChildCount
		select p.parentId, count(p.id)
		from scms_page p, @tblPages tmp
		where tmp.pageid = p.parentId
		and p.deleted = 0
		group by p.parentId
		
		delete @tblPages
		from @tblPages p
		where p.pageId in 
		( 
			select pageId 
			from @tblPagesChildCount 
		)
		

	end


	declare @tblPagesFiltered table
	(
		id int primary key not null identity,
		pageId int not null
	)
	
	insert @tblPagesFiltered
	select tmp.pageId
	from @tblPages tmp
	order by tmp.Ordinal
	
	
	
	
	declare @minIndex int
	declare @maxIndex int
	select 
		@minIndex = min(id),
		@maxIndex = max(id),
		@count = count(id)
	from @tblPagesFiltered

-- select @minIndex '@minIndex' , @maxIndex  '@maxIndex' 



	declare @startIndex int
	declare @endIndex int

	if isnull(@pageSize,0) = 0
	begin
		set @startIndex = @minIndex
		set @endindex = @maxIndex	
	end
	else
	begin
		set @startIndex = @pageSize * @pageNumber + @minIndex
		set @endIndex = @startIndex + @pageSize - 1
	end	
	if isnull(@ascending,0) = 0
	begin
		declare @tmpIndex int
		set @tmpIndex = @startIndex
		set @startIndex = @maxIndex - @endIndex + 1
		set @endIndex = @maxIndex - @tmpIndex + 1
	end
	
	
	
	declare @tblIndex table
	(
		id int primary key not null identity,
		[index] int not null
	)
	
	
	declare @index int

	if isnull(@ascending,0) = '1'
	begin
		set @index = @startIndex
		while @index <= @endIndex
		begin
			insert @tblIndex select @index
			set @index = @index + 1
		end
	end
	else
	begin
		set @index = @endindex
		while @index >= @startIndex
		begin
			insert @tblIndex select @index
			set @index = @index - 1
		end
	end
	
	
	
	
	select p.* 
	from @tblIndex idx,
	@tblPagesFiltered tmp,
	scms_page p
	where idx.[index] = tmp.id
	and tmp.pageId = p.id
	
	
	
	
	
	/*
	select tmp.id, p.* 
	from @tblPagesFiltered tmp, scms_page p
	where tmp.pageId = p.id
	order by tmp.id
	
*/

end

/*
select * from scms_page

declare @count int
exec scms_pages_get
	@siteId = 1,
	@rootPageId = 17,
	@includeChildren = 1,
	@hideParentNodes = 1,
	@pageNumber = null,
	@pageSize = 2,
	@ascending = 1,
	@count = @count out



*/