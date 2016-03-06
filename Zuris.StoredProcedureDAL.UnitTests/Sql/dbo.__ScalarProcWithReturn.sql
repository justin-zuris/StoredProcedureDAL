IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.__ScalarProcWithReturn') AND type IN ( N'P', N'PC' )) 
	drop procedure dbo.__ScalarProcWithReturn;
go

CREATE PROCEDURE dbo.__ScalarProcWithReturn
    @id int = 66
AS 
begin
	return @id + 100;
end
GO

declare @RETURN_VALUE int;
exec @RETURN_VALUE = dbo.__ScalarProcWithReturn 

select @RETURN_VALUE