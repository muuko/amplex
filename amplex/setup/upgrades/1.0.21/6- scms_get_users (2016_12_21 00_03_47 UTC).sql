if exists ( select [id] from sysobjects where [name] = 'scms_get_users' and xtype = 'p' )
	drop procedure scms_get_users
go

create procedure scms_get_users
(
	@username nvarchar(128),
	@emailAddress nvarchar(128),
	@firstName nvarchar(128),
	@lastName nvarchar(128),
	@role nvarchar(32),
	@orgId int,
	@pagenumber int,
	@pagesize int, 
	@sortby nvarchar(32),
	@ascending bit,
	@count int out
)
as
begin

/*
declare @gen table
(
	userid uniqueidentifier not null primary key,
	username nvarchar(512),
	email nvarchar(512),
	firstName nvarchar(256),
	lastName nvarchar(256)
)
select * from @gen
*/

	-- all users that match
	declare @tblMatchingUsers table
	(
		userid uniqueidentifier not null primary key
	)
	
	insert @tblMatchingUsers
	select userid from aspnet_users
	
	-- remove those not in requested role
	if( isnull(@role,'') <> '' )
	begin
		declare @roleId uniqueidentifier
		select @roleId = RoleId 
		from aspnet_Roles
		where RoleName = @role
		if @@rowcount = 1
		begin
			delete from @tblMatchingUsers
			from @tblMatchingUsers tmp
			where tmp.UserId not in ( select uir.UserId
				  from aspnet_UsersInRoles uir
				  where uir.RoleId = @roleId )
		end
	end
	
	-- remove those not in organization
	if( @orgId is not null)
	begin
	
		delete @tblMatchingUsers
		from @tblMatchingUsers tmp
		where tmp.userId not in (	select userId from scms_user where orgId = @orgId )
	end

	
	-- remove those not matching user name
	if( isnull(@username,'') <> '' )
	begin
		delete from @tblMatchingUsers
		from @tblMatchingUsers tmp
		where tmp.UserId not in ( select u.UserId
			  from aspnet_Users u
			  where u.UserName like '%' + @username + '%')
	end
	
	-- remove those not matching email address
	if( isnull(@emailAddress,'') <> '' )
	begin
		delete from @tblMatchingUsers
		from @tblMatchingUsers tmp
		where tmp.UserId not in ( select m.UserId
			  from aspnet_Membership m
			  where m.email like '%' + @emailAddress + '%')
	end
	
	-- remove those not matching first name
	if( isnull(@firstName,'') <> '' )
	begin
		delete from @tblMatchingUsers
		from @tblMatchingUsers tmp
		where tmp.UserId not in ( select u.UserId
			  from scms_user u
			  where u.firstname like '%' + @firstName + '%')
	end
	
	
	-- remove those not matching last name
	if( isnull(@lastName,'') <> '' )
	begin
		delete from @tblMatchingUsers
		from @tblMatchingUsers tmp
		where tmp.UserId not in ( select u.UserId
			  from scms_user u
			  where u.lastname like '%' + @lastName + '%')
	end
	
	-- sorted matching users
	declare @tblOrderedMatchinUsers table
	(
		id int not null identity,
		userid uniqueidentifier not null
	)
	
	insert @tblOrderedMatchinUsers
	(
		userid
	)
	select tmp.userid
	from @tblMatchingUsers tmp, aspnet_users u
	where tmp.userid = u.userid
	order by u.username
	
	

declare @minIndex int
declare @maxIndex int
select 
	@minIndex = min(id),
	@maxIndex = max(id),
	@count = count(id)
from @tblOrderedMatchinUsers

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

-- resultSet contains on the items to be selected
declare @resultSet table
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
		insert @resultSet select @index
		set @index = @index + 1
	end
end
else
begin
	set @index = @endindex
	while @index >= @startIndex
	begin
		insert @resultSet select @index
		set @index = @index - 1
	end
end
	
	
	-- select the data
	select 
		tmp2.userid,
		a.username,
		m.email,
		u.firstName,
		u.lastName
	from  @resultSet tmp1
	left join @tblOrderedMatchinUsers tmp2
	on tmp1.id = tmp2.id
	left join aspnet_users a
	on tmp2.userid = a.userid
	left join aspnet_membership m
	on tmp2.userid = m.userid
	left outer join scms_user u
	on tmp2.userid = u.userid
	order by tmp1.ordinal
	

end


go



/*
sp_help scms_user

declare @count int
exec scms_get_users
	@username = null,
	@emailAddress = null,
	@firstName = null,
	@lastName = null,
	@role = null,
	@orgId = 1,
	@pagenumber = 0,
	@pagesize = null,
	@sortby = null,
	@ascending = 0,
	@count = @count out

select * from aspnet_users
*/