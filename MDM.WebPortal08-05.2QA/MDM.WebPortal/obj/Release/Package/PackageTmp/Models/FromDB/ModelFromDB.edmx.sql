
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/21/2016 08:36:57
-- Generated from EDMX file: C:\Dev\Projects\MedProMDM\MedProMDM\Models\DBModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MedProDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[MasterUserList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MasterUserList];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'MasterUserLists'
CREATE TABLE [dbo].[MasterUserLists] (
    [id] int IDENTITY(1,1) NOT NULL,
    [ADP_ID] varchar(15)  NOT NULL,
    [EmployeeName] nvarchar(255)  NULL,
    [ADP_ID_] bigint  NULL,
    [Edgemed_UserName] nvarchar(255)  NULL,
    [ZoomServer] bigint  NULL,
    [EdgeMed_ID] bigint  NULL,
    [Job_Title] nvarchar(255)  NULL,
    [Staff_Manager] nvarchar(255)  NULL,
    [User_Active] char(1)  NULL,
    [FirstName] nvarchar(255)  NULL,
    [Middle_Init] nvarchar(1)  NULL,
    [LastName] nvarchar(255)  NULL,
    [NTUser] varchar(80)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'MasterUserLists'
ALTER TABLE [dbo].[MasterUserLists]
ADD CONSTRAINT [PK_MasterUserLists]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------