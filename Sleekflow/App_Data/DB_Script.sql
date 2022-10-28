USE [Sleekflow]
GO

/****** Object:  Table [dbo].[Todo]    Script Date: 10/28/2022 1:31:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Todo](
	[id] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[description] [nvarchar](1000) NULL,
	[duedate] [datetime] NULL,
	[status] [nvarchar](50) NULL,
 CONSTRAINT [PK_Todo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_get_stored_proc_parameter_list]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[usp_get_stored_proc_parameter_list]
GO

CREATE PROCEDURE [dbo].[usp_get_stored_proc_parameter_list]
(
@sp_name nvarchar(100)
)
AS

SELECT a.*, b.name AS typename 
  FROM syscolumns a
  INNER JOIN systypes b ON a.xtype = b.xtype AND a.xusertype = b.xusertype
 WHERE id = (SELECT id FROM sysobjects WHERE xtype = 'P' AND [name] = @sp_name)
 ORDER BY colorder

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_delete_todo]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[usp_delete_todo]
GO

CREATE PROCEDURE [dbo].[usp_delete_todo]
(
	@id uniqueidentifier
)
AS

DELETE FROM [dbo].[Todo] 
WHERE id = @id

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_filter_todo]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[usp_filter_todo]
GO

CREATE PROCEDURE [dbo].[usp_filter_todo]
(
	@filter_col varchar(50),
	@filter_val nvarchar(100),
	@is_date varchar(1),
	@sort_by varchar(50)
)
AS	

IF (LTRIM(RTRIM(ISNULL(@filter_col, ''))) <> '' AND LTRIM(RTRIM(ISNULL(@filter_val, ''))) <> '' AND LTRIM(RTRIM(ISNULL(@is_date, ''))) <> '')
	BEGIN
		DECLARE @SQL nvarchar(MAX)

		SET @SQL = 'SELECT [id], [name] '
		SET @SQL += ',ISNULL([description], '''') AS [description] '
		SET @SQL += ',CONVERT(varchar(20), ISNULL([duedate], ''''), 106) AS [duedate] '
		SET @SQL += ',ISNULL([status], '''') AS [status] '
		SET @SQL += 'FROM [dbo].[Todo] '

		IF LTRIM(RTRIM(ISNULL(@is_date, ''))) = 'Y'
			SET @SQL += 'WHERE CONVERT(nvarchar, ' + @filter_col + ', 112) = ''' + @filter_val + ''' '
		ELSE 
			SET @SQL += 'WHERE ' + @filter_col + ' LIKE ''%' + @filter_val + '%'' '
		
		IF LTRIM(RTRIM(ISNULL(@sort_by, ''))) <> ''
			SET @SQL += 'ORDER BY ' + @sort_by

		EXEC(@SQL)
	END

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_get_todo]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[usp_get_todo]
GO

CREATE PROCEDURE [dbo].[usp_get_todo]
(
	@id uniqueidentifier
)
AS

SELECT [id]
      ,[name]
      ,[description]
      ,[duedate]
      ,[status]
  FROM [dbo].[Todo]
WHERE [id] = @id

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_insert_todo]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[usp_insert_todo]
GO

CREATE PROCEDURE [dbo].[usp_insert_todo]
(
	@name nvarchar(100),
	@description nvarchar(1000),
	@duedate datetime,
	@status nvarchar(50),
	@id uniqueidentifier OUT
)
AS

SET @id = NEWID()

INSERT INTO [dbo].[Todo]
           ([id]
           ,[name]
           ,[description]
           ,[duedate]
           ,[status])
     VALUES
           (@id
           ,@name
           ,@description
           ,@duedate
           ,@status)

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[[usp_update_todo]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[usp_update_todo]
GO

CREATE PROCEDURE [dbo].[usp_update_todo]
(
	@id uniqueidentifier,
	@name nvarchar(100),
	@description nvarchar(1000),
	@duedate datetime,
	@status nvarchar(50)
)
AS

UPDATE [dbo].[Todo]
   SET [name] = @name
      ,[description] = @description
      ,[duedate] = @duedate
      ,[status] = @status
 WHERE id = @id

GO