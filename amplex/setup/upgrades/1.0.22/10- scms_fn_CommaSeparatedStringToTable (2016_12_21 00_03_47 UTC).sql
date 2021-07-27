if exists ( select [id] from sysobjects where name = 'scms_fn_CommaSeparatedStringToTable' and xtype = 'TF')
	drop function scms_fn_CommaSeparatedStringToTable
go

create function scms_fn_CommaSeparatedStringToTable
(
	@CommaSeparatedValues varchar(max),
	@IncludeEmptyStrings bit
)
returns @Item table
(
	RowId int IDENTITY(1, 1) NOT NULL, 
    Value VARCHAR(200)
)
as
begin

	declare @IndefOfComma int,
	@Value VARCHAR(200),
	@StartPos bigint,
	@EndPos bigint,
	@LengthOfString int, 
	@ReachedEnd bit

	SET @StartPos=1
	SET @EndPos=0
	SET @LengthOfString =LEN(@CommaSeparatedValues)
	SET @ReachedEnd = 0

	WHILE @ReachedEnd <> 1
	BEGIN
		SET @EndPos = CHARINDEX (',', @CommaSeparatedValues, @StartPos)
		IF @EndPos > 0
		BEGIN
			SET @Value = SUBSTRING(@CommaSeparatedValues, @StartPos, @EndPos - @StartPos) 
			SET @StartPos = @EndPos + 1
		END
		ELSE
		BEGIN
			set @ReachedEnd = 1
			SET @Value = SUBSTRING(@CommaSeparatedValues, @StartPos, @LengthOfString - (@StartPos-1) )
		END

		if( (@value <> '') or (@IncludeEmptyStrings = 1) )
		begin
			INSERT INTO @Item(Value) VALUES(@Value)
		end
	END

	RETURN

END

/*
select * from scms_fn_CommaSeparatedStringToTable( '1.4.6.4,2,3,,4,a,asdf,oo', 0)

*/