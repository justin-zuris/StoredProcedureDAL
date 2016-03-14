IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.__ScalarProcWithSelect') AND type IN ( N'P', N'PC' )) 
	drop procedure dbo.__ScalarProcWithSelect;
go

CREATE PROCEDURE dbo.__ScalarProcWithSelect
    @id int = 66
AS 
begin
	select @id + 100;
end
/*
exec dbo.__ScalarProcWithSelect 
*/
GO
