IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.__SingleResultSetTest') AND type IN ( N'P', N'PC' )) 
	drop procedure dbo.__SingleResultSetTest;
go

CREATE PROCEDURE dbo.__SingleResultSetTest
    @id int = 66,
    @name nvarchar(50) = 'Justin Greenwood', 
    @enabled bit = 1, 
    @cost float = 0.0
AS 
begin
	set @id = isnull(@id, 1)
	set @enabled = isnull(@enabled, 1)
	select @id as Id, @name as Name, @enabled as Enabled, @cost as Cost, newid() as UniqueId, checksum(@id, @name, @enabled, @cost) as [Checksum]
	union
	select @id+1, 'Joe Shmoe', 0, 123.32, newid(), checksum(@id+1, @name, @enabled, @cost)
	union
	select @id+2, 'Crapola Smith', 1, 99.98, newid(), checksum(@id+2, @name, @enabled, @cost);
end
/*
exec dbo.__SingleResultSetTest
*/
GO
