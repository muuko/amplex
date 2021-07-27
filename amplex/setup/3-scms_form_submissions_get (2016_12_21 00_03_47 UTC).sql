if exists ( select [id] from sysobjects where [name] = 'scms_form_submissions_get' and xtype = 'p' )
	drop procedure scms_form_submissions_get
go

create procedure scms_form_submissions_get
@formid int,
@pageSize int,
@pageNumber int,
@orderby nvarchar(64), -- ignored
@ascending bit, -- ignored
@dateFrom datetime,
@dateTo datetime,
@count int out
as

set @count = 0

declare @includedSubmissions table
(
	id int identity not null primary key,
	submissionId int not null
)

insert @includedSubmissions
select id 
from scms_form_submission
where formid = @formid
and deleted = 0
and submissionTime >= isnull(@dateFrom,'1-1-1900')
and submissionTime <= isnull(@dateTo,'1-1-2100')
order by id

-- select 'ids', * from @includedSubmissions

declare @minIndex int
declare @maxIndex int
select 
	@minIndex = min(id),
	@maxIndex = max(id),
	@count = count(id)
from @includedSubmissions

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



select fs.*
from 
	scms_form_submission fs,
	@orderedPageSubmissions tmp,
	@includedSubmissions tmp2
where tmp.id = tmp2.id
and tmp2.submissionId = fs.id
order by tmp.ordinal

go


