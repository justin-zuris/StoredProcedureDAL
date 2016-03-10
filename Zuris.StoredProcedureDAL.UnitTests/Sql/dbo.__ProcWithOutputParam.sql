IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.__ProcWithOutputParam') AND type IN ( N'P', N'PC' )) 
	drop procedure dbo.__ProcWithOutputParam;
go

CREATE PROCEDURE dbo.__ProcWithOutputParam
    @id int,
	@outputId uniqueidentifier OUTPUT
AS 
begin
	set @outputId = newid()
	select @id + 100;
end
GO

declare @id int = 12;
declare @outputId uniqueidentifier
exec dbo.__ProcWithOutputParam @id, @outputId output

select @outputId