
if exists ( select [id] from sysobjects where [name] = 'scms_submissions_get' and xtype = 'p' )
	drop procedure scms_submissions_get
go

create procedure [dbo].[scms_submissions_get]
(
	@submissionModuleId int,
	@submissionId int,
	@pageSize int,
	@pageNumber int,
	@orderby nvarchar(64), -- ignored
	@ascending bit, -- ignored
	@approved bit,
	@featured bit,
	@dateFrom datetime,
	@dateTo datetime,
	@includeDeleted bit,		
	@count int out
)
as

set @count = 0
set @pageNumber = isnull(@pageNumber,0)

declare @includedSubmissions table
(
	submissionId int primary key not null
)

if @submissionId is not null
begin
	insert @includedSubmissions	( submissionId ) values ( @submissionId )
end
else
begin


insert @includedSubmissions
select id 
from scms_submission_submission
where submissionModuleId = @submissionModuleId
and ((deleted = 0) or (@includeDeleted <> 0))
and submissionTime >= isnull(@dateFrom,'1-1-1900')
and submissionTime <= isnull(@dateTo,'1-1-2100')
and ((@approved is null) or ((@approved <> 0) and (approvedTime is not null)) or ((@approved = 0) and (approvedTime is null)))
and ((@featured is null) or ((@featured <> 0) and (featuredTime is not null)) or ((@featured = 0) and (featuredTime is null)))
order by id


declare @includedSubmissionsFilterOrdered table
(
	id int identity not null primary key,
	submissionId int not null
)

if isnull(@featured,0) = 1
begin
	insert @includedSubmissionsFilterOrdered
	select tmp.submissionId
	from @includedSubmissions tmp,
		 scms_submission_submission s
	where tmp.submissionid = s.id
	order by featuredTime
end
else if isnull(@approved,0) = 1
begin
	insert @includedSubmissionsFilterOrdered
	select tmp.submissionId
	from @includedSubmissions tmp,
		 scms_submission_submission s
	where tmp.submissionid = s.id
	order by approvedTime
end
else
	insert @includedSubmissionsFilterOrdered
	select tmp.submissionId
	from @includedSubmissions tmp
	order by tmp.submissionId
end


-- select 'ids', * from @@includedSubmissionsFilterOrdered
declare @minIndex int
declare @maxIndex int
select 
	@minIndex = min(id),
	@maxIndex = max(id),
	@count = count(id)
from @includedSubmissionsFilterOrdered

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

-- select @startIndex '@startIndex', @endIndex '@endIndex'

declare @orderedPageSubmissions table
(
	ordinal int not null primary key identity,
	id int not null
)


declare @index int

if isnull(@ascending,0) = '1'
begin
	set @index = @startIndex
	while @index <= @endIndex
	begin
		insert @orderedPageSubmissions select @index
		set @index = @index + 1
	end
end
else
begin
	set @index = @endindex
	while @index >= @startIndex
	begin
		insert @orderedPageSubmissions select @index
		set @index = @index - 1
	end
end

-- select 'orderedids', * from @orderedPageSubmissions



select s.*
from 
	scms_submission_submission s,
	@orderedPageSubmissions tmp,
	@includedSubmissionsFilterOrdered tmp2
where tmp.id = tmp2.id
and tmp2.submissionId = s.id
order by tmp.ordinal


GO



/*
select * from scms_submission_module
select * from scms_submission_submission

declare @count int
exec [scms_submissions_get]
	@submissionModuleId = 1,
	@submissionId = null,
	@pageSize = 5,
	@pageNumber = null,
	@orderby = null,
	@ascending = 1,
	@approved = null,
	@featured = null,
	@dateFrom = null,
	@dateTo = null,
	@includeDeleted = 0,
	@count = @count out
select @count


declare @approved bit
declare @featured bit
declare @includeDeleted bit
declare @dateFrom datetime
declare @dateTo datetime
set @approved = 0
set @featured = 0
set @includeDeleted = 0

select id 
from scms_submission_submission
where submissionModuleId = 1
and ((deleted = 0) or (@includeDeleted <> 0))
and submissionTime >= isnull(@dateFrom,'1-1-1900')
and submissionTime <= isnull(@dateTo,'1-1-2100')
and ((@approved is null) or ((@approved <> 0) and (approvedTime is not null)) or ((@approved = 0) and (approvedTime is null)))
and ((@featured is null) or ((@featured <> 0) and (featuredTime is not null)) or ((@featured = 0) and (featuredTime is null)))
order by id


*/