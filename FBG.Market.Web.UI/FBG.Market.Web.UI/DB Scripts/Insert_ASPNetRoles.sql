USE [fbgmaket_dev_fbgmarket]
GO


INSERT INTO [dbo].[AspNetRoles] ([Id],[Name]) 
VALUES
	('1' , 'Admin'),
	('2' , 'BrandAdmin'),
	('3' , 'BrandUser'),
	('4' , 'Sales Assistant'),
	('5' , 'SuperAdmin'),
	('6' , 'Operation'),
	('7' , 'Sale')

--TRUNCATE TABLE [dbo].[AspNetRoles];


