IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.__MultiResultSetTest') AND type IN ( N'P', N'PC' )) 
	drop procedure dbo.__MultiResultSetTest;
go

CREATE PROCEDURE dbo.__MultiResultSetTest
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

	declare @i int = 0;
	declare @resultSet2 table (IdColumn int identity, AmountColumn money, GuidColumn uniqueidentifier, DateTimeColumn datetime default getdate() );
	while (@i < 100) 
	begin
		insert into @resultSet2 (AmountColumn, GuidColumn) values ((rand() * 100000), newid());
		set @i = @i + 1;
	end

	select * from @resultSet2;
end
/*
exec dbo.__MultiResultSetTest
*/
GO
