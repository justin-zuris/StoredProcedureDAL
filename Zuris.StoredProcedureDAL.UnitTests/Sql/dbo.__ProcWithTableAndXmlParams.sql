IF not EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = '__CountryType') 
begin
	CREATE TYPE [dbo].[__CountryType] AS TABLE(
		[Code] varchar(2) NOT NULL,
		[Name] nvarchar(256) NOT NULL
	)
end
go

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.__ProcWithTableAndXmlParams') AND type IN ( N'P', N'PC' )) 
	drop procedure dbo.__ProcWithTableAndXmlParams;
go

CREATE PROCEDURE dbo.__ProcWithTableAndXmlParams
    @countries [dbo].[__CountryType] readonly,
	@xml xml
AS 
begin
	select c.*, @xml as MyXml from @countries c;
end
GO

declare @countries [__CountryType];
insert into @countries values ('US', 'United States')
insert into @countries values ('CA', 'Canada')
declare @xml xml = '<stuff><x>Hi</x><x>There</x></stuff>'
exec dbo.__ProcWithTableAndXmlParams @countries, @xml
