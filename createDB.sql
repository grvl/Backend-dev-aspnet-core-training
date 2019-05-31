IF  NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'WishlistDB')
    BEGIN
		CREATE DATABASE WishlistDB
	END
GO

USE WishlistDB
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
	BEGIN
		CREATE TABLE dbo.Users
			(UserId		int IDENTITY(1,1),
			Username	varchar(25) NOT NULL UNIQUE,
			Pswd		varchar(100) NOT NULL,
			UserRole	varchar(100) DEFAULT 'User',
			CONSTRAINT PK_Users PRIMARY KEY(UserId)
			)
	END
GO

IF OBJECT_ID(N'dbo.List', N'U') IS NULL
	BEGIN
		CREATE TABLE dbo.List
			(ListId		int IDENTITY(1,1),
			ListName	varchar(25) NOT NULL,
			CONSTRAINT PK_List PRIMARY KEY(ListId)
			)
	END
GO

IF OBJECT_ID(N'dbo.UserList', N'U') IS NULL
	BEGIN
		CREATE TABLE dbo.UserList
			(UserId		int NOT NULL,
			ListId		int NOT NULL,
			EditPermission	bit	DEFAULT 1,
			PRIMARY KEY(userId, ListId),
			CONSTRAINT FK_UserOwner FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
			CONSTRAINT FK_ListOwned FOREIGN KEY (ListId) REFERENCES dbo.List(ListId)
			)
	END
GO

IF OBJECT_ID(N'dbo.Item', N'U') IS NULL
	BEGIN
		CREATE TABLE dbo.Item
			(ItemId		int IDENTITY(1,1),
			ListId		int NOT NULL,
			ItemName	varchar(50) NOT NULL,
			Quantity	int DEFAULT 1,
			Price		money DEFAULT 0.00,
			Bought		int DEFAULT 0,
			CONSTRAINT FK_PartOfList FOREIGN KEY (ListId) REFERENCES dbo.List(ListId),
			PRIMARY KEY (ItemId)
			)
	END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[sp_InsertUserListInfo]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[sp_InsertUserListInfo]
END

GO

CREATE PROCEDURE [dbo].[sp_InsertUserListInfo]
    -- Add the parameters for the stored procedure here
    @UserId int,
    @ListName varchar(25)
AS
BEGIN
	DECLARE @ListId int;
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

		INSERT  INTO [WishlistDB].[dbo].[List]([ListName])
        VALUES(@ListName);

		SELECT @ListID = SCOPE_IDENTITY();

        INSERT INTO [WishlistDB].[dbo].[UserList]([UserId],[ListId])
        VALUES(@UserId, @ListId);


END