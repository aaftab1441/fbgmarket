USE [fbgmaket_dev_fbgmarket]
GO

--Drop
ALTER TABLE [dbo].[AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Vendor]

USE [fbgmaket_dev_fbgmarket]
GO
--Add
ALTER TABLE [dbo].[AspNetUsers] WITH CHECK ADD CONSTRAINT [FK_AspNetUsers_Vendor] FOREIGN KEY([VID])
REFERENCES [dbo].[Vendor] ([VID])
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Vendor]

