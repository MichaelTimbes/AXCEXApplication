IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20180305041844_Init')
BEGIN
    CREATE TABLE [ProjectAssignments] (
        [ID] int NOT NULL IDENTITY,
        [EmpKey] int NOT NULL,
        [ProjKey] int NOT NULL,
        [authorized_assignment] nvarchar(max) NULL,
        CONSTRAINT [PK_ProjectAssignments] PRIMARY KEY ([ID])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20180305041844_Init')
BEGIN
    CREATE TABLE [ProjectModel] (
        [ProjectID] int NOT NULL IDENTITY,
        [CustomerName] nvarchar(max) NULL,
        [EndDate] datetime2 NOT NULL,
        [ActiveProject] bit NOT NULL,
        [ProjBudget] money NOT NULL,
        [ProjCurentCost] money NOT NULL,
        [ProjectName] nvarchar(max) NULL,
        [StartDate] datetime2 NOT NULL,
        CONSTRAINT [PK_ProjectModel] PRIMARY KEY ([ProjectID])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20180305041844_Init')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20180305041844_Init', N'2.0.1-rtm-125');
END;

GO

