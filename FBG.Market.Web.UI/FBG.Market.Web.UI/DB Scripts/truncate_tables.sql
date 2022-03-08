USE [fbgmaket_dev_fbgmarket]
GO

ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Vendor]
ALTER TABLE [dbo].[Brand] DROP CONSTRAINT [FK_Brand_Vendor]
--ALTER TABLE [dbo].[ClientLocation] DROP CONSTRAINT [T_ClientLocation$T_ClientsT_ClientLocation]
--ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [DF__Clients__CStatus__5FB337D6]
--ALTER TABLE [dbo].[OrderCR] DROP CONSTRAINT [T_OrderCR$T_OrdersT_OrderCR]
ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [T_OrderDetails${7AF3CB47-96B7-4178-B4BB-FBE0C413F5B1}]
ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [T_OrderDetails$T_ProductsT_OrderDetails]
--ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [T_Orders$Ref_OrderTypeT_Orders]
--ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [T_Orders$T_ClientsT_Orders]
--ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [T_Orders$T_EmployeesT_Orders]
ALTER TABLE [dbo].[ProductImage] DROP CONSTRAINT [FK_ProductImage_Products]
ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_Products_Brand]
ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_Products_ProductStatus]
ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_Products_RefNRFColorCodes]
ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_Products_Vendor]
--ALTER TABLE [dbo].[RefNRFColorCodes] DROP CONSTRAINT [FK__RefNRFCol__Color__0E6E26BF]
--ALTER TABLE [dbo].[Vendor] DROP CONSTRAINT [DF_Vendor_UserId]

TRUNCATE TABLE [dbo].[AspNetRoles];	
TRUNCATE TABLE [dbo].[AspNetUserRoles];	
TRUNCATE TABLE [dbo].[AspNetUserClaims]
TRUNCATE TABLE [dbo].[AspNetUserLogins]
TRUNCATE TABLE [dbo].[AspNetUserRoles]
TRUNCATE TABLE [dbo].[AspNetUsers];
TRUNCATE TABLE [dbo].[Brand];
--TRUNCATE TABLE [dbo].[ClientLocation];
--TRUNCATE TABLE [dbo].[Clients];
--TRUNCATE TABLE [dbo].[ColorCategory];
TRUNCATE TABLE [dbo].[dest];
--TRUNCATE TABLE [dbo].[Employees];
--TRUNCATE TABLE [dbo].[Inventory];
--TRUNCATE TABLE [dbo].[OrderCR];
--TRUNCATE TABLE [dbo].[OrderDetails];
--TRUNCATE TABLE [dbo].[OrderProcessForce];
--TRUNCATE TABLE [dbo].[OrderProcessLog];	
--TRUNCATE TABLE [dbo].[Orders];
--TRUNCATE TABLE [dbo].[prod_data];
TRUNCATE TABLE [dbo].[prod_desc_dup];
TRUNCATE TABLE [dbo].[ProductImage];
TRUNCATE TABLE [dbo].[Products];
TRUNCATE TABLE [dbo].[ProductsImport];
--TRUNCATE TABLE [dbo].[ProductStatus];
--TRUNCATE TABLE [dbo].[RefCategory];
--TRUNCATE TABLE [dbo].[RefClientCategories];
--TRUNCATE TABLE [dbo].[RefNRFColorCodes];
--TRUNCATE TABLE [dbo].[RefOrderType];
--TRUNCATE TABLE [dbo].[RefProductFamily];
TRUNCATE TABLE [dbo].[RefSeason];
--TRUNCATE TABLE [dbo].[RefStates];
--TRUNCATE TABLE [dbo].[RefWarehouse];
TRUNCATE TABLE [dbo].[Sales Reports];
--TRUNCATE TABLE [dbo].[shopify_prod_desc];
TRUNCATE TABLE [dbo].[Vendor];

ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Vendor] FOREIGN KEY([VID])
REFERENCES [dbo].[Vendor] ([VID])
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Vendor]
ALTER TABLE [dbo].[Brand]  WITH CHECK ADD  CONSTRAINT [FK_Brand_Vendor] FOREIGN KEY([VID])
REFERENCES [dbo].[Vendor] ([VID])
ALTER TABLE [dbo].[Brand] CHECK CONSTRAINT [FK_Brand_Vendor]
--ALTER TABLE [dbo].[ClientLocation]  WITH NOCHECK ADD  CONSTRAINT [T_ClientLocation$T_ClientsT_ClientLocation] FOREIGN KEY([CClientID])
--REFERENCES [dbo].[Clients] ([CID])
--ALTER TABLE [dbo].[ClientLocation] CHECK CONSTRAINT [T_ClientLocation$T_ClientsT_ClientLocation]
--ALTER TABLE [dbo].[Clients] ADD  DEFAULT ('A') FOR [CStatus]
--ALTER TABLE [dbo].[OrderCR]  WITH CHECK ADD  CONSTRAINT [T_OrderCR$T_OrdersT_OrderCR] FOREIGN KEY([OID])
--REFERENCES [dbo].[Orders] ([OID])
--ALTER TABLE [dbo].[OrderCR] CHECK CONSTRAINT [T_OrderCR$T_OrdersT_OrderCR]
ALTER TABLE [dbo].[OrderDetails]  WITH NOCHECK ADD  CONSTRAINT [T_OrderDetails$T_ProductsT_OrderDetails] FOREIGN KEY([ODProductID])
REFERENCES [dbo].[Products] ([PID])
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [T_OrderDetails$T_ProductsT_OrderDetails]
ALTER TABLE [dbo].[OrderDetails]  WITH NOCHECK ADD  CONSTRAINT [T_OrderDetails${7AF3CB47-96B7-4178-B4BB-FBE0C413F5B1}] FOREIGN KEY([ODOID])
REFERENCES [dbo].[Orders] ([OID])
ON DELETE CASCADE
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [T_OrderDetails${7AF3CB47-96B7-4178-B4BB-FBE0C413F5B1}]
--ALTER TABLE [dbo].[Orders]  WITH NOCHECK ADD  CONSTRAINT [T_Orders$T_EmployeesT_Orders] FOREIGN KEY([OEmpID])
--REFERENCES [dbo].[Employees] ([EID])
--ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [T_Orders$T_EmployeesT_Orders]
--ALTER TABLE [dbo].[Orders]  WITH NOCHECK ADD  CONSTRAINT [T_Orders$T_ClientsT_Orders] FOREIGN KEY([OClientID])
--REFERENCES [dbo].[Clients] ([CID])
--ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [T_Orders$T_ClientsT_Orders]
--ALTER TABLE [dbo].[Orders]  WITH NOCHECK ADD  CONSTRAINT [T_Orders$Ref_OrderTypeT_Orders] FOREIGN KEY([OType])
--REFERENCES [dbo].[RefOrderType] ([OTID])
--ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [T_Orders$Ref_OrderTypeT_Orders]
ALTER TABLE [dbo].[ProductImage]  WITH CHECK ADD  CONSTRAINT [FK_ProductImage_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([PID])
ALTER TABLE [dbo].[ProductImage] CHECK CONSTRAINT [FK_ProductImage_Products]
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Vendor] FOREIGN KEY([VID])
REFERENCES [dbo].[Vendor] ([VID])
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Vendor]
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_RefNRFColorCodes] FOREIGN KEY([ColorCategoryId])
REFERENCES [dbo].[RefNRFColorCodes] ([NRFColorCodeID])
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_RefNRFColorCodes]
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_ProductStatus] FOREIGN KEY([ProductStatusId])
REFERENCES [dbo].[ProductStatus] ([Id])
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_ProductStatus]
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Brand] FOREIGN KEY([BID])
REFERENCES [dbo].[Brand] ([BID])
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Brand]
--ALTER TABLE [dbo].[RefNRFColorCodes]  WITH CHECK ADD FOREIGN KEY([ColorCategoryID])
--REFERENCES [dbo].[ColorCategory] ([ColorCategoryID])
--ALTER TABLE [dbo].[Vendor] ADD  CONSTRAINT [DF_Vendor_UserId]  DEFAULT (N'cd8e2fb3-31c0-4519-9d91-98e1325c94ee') FOR [UserId]