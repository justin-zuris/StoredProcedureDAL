IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.__ExecuteNonQueryWithOptionalError') AND type IN ( N'P', N'PC' )) 
	drop procedure dbo.__ExecuteNonQueryWithOptionalError;
go

CREATE PROCEDURE dbo.__ExecuteNonQueryWithOptionalError
    @throwError bit = 0
AS 
begin
	declare @test table (Test1 int, Test2 varchar(234))
	insert into @test values (123, 'test2')
	insert into @test values (124, 'test3')
	update @test set Test1 = Test1 + 10

	if (@throwError = 1) 
	begin
		exec sp_executesql 'select * from buttcheeks where smutt = 123'
	end

end
GO

declare @returnValue int
exec @returnValue = dbo.__ExecuteNonQueryWithOptionalError

select @returnValue