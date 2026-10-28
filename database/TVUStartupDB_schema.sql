IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [Banner] (
        [BannerId] int NOT NULL IDENTITY,
        [Title] nvarchar(150) NULL,
        [ImageUrl] nvarchar(255) NOT NULL,
        [LinkUrl] nvarchar(255) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Banner] PRIMARY KEY ([BannerId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [Category] (
        [CategoryId] int NOT NULL IDENTITY,
        [CategoryName] nvarchar(100) NOT NULL,
        [Slug] varchar(120) NOT NULL,
        [Icon] nvarchar(100) NULL,
        [Description] nvarchar(500) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Category] PRIMARY KEY ([CategoryId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [Faculty] (
        [FacultyId] int NOT NULL IDENTITY,
        [FacultyName] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Faculty] PRIMARY KEY ([FacultyId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [Role] (
        [RoleId] int NOT NULL IDENTITY,
        [RoleName] nvarchar(50) NOT NULL,
        [Description] nvarchar(200) NULL,
        CONSTRAINT [PK_Role] PRIMARY KEY ([RoleId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [Account] (
        [AccountId] int NOT NULL IDENTITY,
        [Email] varchar(100) NOT NULL,
        [PasswordHash] varchar(200) NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [StudentCode] varchar(20) NULL,
        [ClassName] nvarchar(50) NULL,
        [Phone] varchar(15) NULL,
        [Avatar] nvarchar(255) NULL,
        [FacultyId] int NULL,
        [RoleId] int NOT NULL,
        [Status] tinyint NOT NULL DEFAULT CAST(1 AS tinyint),
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime NULL,
        CONSTRAINT [PK_Account] PRIMARY KEY ([AccountId]),
        CONSTRAINT [FK_Account_Faculty_FacultyId] FOREIGN KEY ([FacultyId]) REFERENCES [Faculty] ([FacultyId]),
        CONSTRAINT [FK_Account_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([RoleId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [News] (
        [NewsId] int NOT NULL IDENTITY,
        [Title] nvarchar(200) NOT NULL,
        [Slug] varchar(220) NOT NULL,
        [Thumbnail] nvarchar(255) NULL,
        [Summary] nvarchar(500) NULL,
        [Content] nvarchar(max) NOT NULL,
        [NewsType] tinyint NOT NULL DEFAULT CAST(1 AS tinyint),
        [EventDate] datetime NULL,
        [Location] nvarchar(200) NULL,
        [AuthorId] int NOT NULL,
        [IsPublished] bit NOT NULL DEFAULT CAST(0 AS bit),
        [ViewCount] int NOT NULL DEFAULT 0,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime NULL,
        CONSTRAINT [PK_News] PRIMARY KEY ([NewsId]),
        CONSTRAINT [FK_News_Account_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [Account] ([AccountId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [Project] (
        [ProjectId] int NOT NULL IDENTITY,
        [ProjectName] nvarchar(150) NOT NULL,
        [Slug] varchar(170) NOT NULL,
        [Slogan] nvarchar(200) NULL,
        [ShortDescription] nvarchar(500) NOT NULL,
        [Content] nvarchar(max) NULL,
        [Logo] nvarchar(255) NULL,
        [CoverImage] nvarchar(255) NULL,
        [VideoUrl] nvarchar(255) NULL,
        [WebsiteUrl] nvarchar(255) NULL,
        [CommunityUrl] nvarchar(255) NULL,
        [CategoryId] int NOT NULL,
        [FacultyId] int NOT NULL,
        [OwnerId] int NOT NULL,
        [Stage] tinyint NOT NULL DEFAULT CAST(1 AS tinyint),
        [Status] tinyint NOT NULL DEFAULT CAST(0 AS tinyint),
        [RejectReason] nvarchar(500) NULL,
        [IsFeatured] bit NOT NULL DEFAULT CAST(0 AS bit),
        [ViewCount] int NOT NULL DEFAULT 0,
        [FoundedDate] date NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime NULL,
        [ApprovedAt] datetime NULL,
        [ApprovedBy] int NULL,
        CONSTRAINT [PK_Project] PRIMARY KEY ([ProjectId]),
        CONSTRAINT [FK_Project_Account_ApprovedBy] FOREIGN KEY ([ApprovedBy]) REFERENCES [Account] ([AccountId]),
        CONSTRAINT [FK_Project_Account_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Account] ([AccountId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Project_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([CategoryId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Project_Faculty_FacultyId] FOREIGN KEY ([FacultyId]) REFERENCES [Faculty] ([FacultyId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [Comment] (
        [CommentId] int NOT NULL IDENTITY,
        [ProjectId] int NOT NULL,
        [AccountId] int NOT NULL,
        [ParentId] int NULL,
        [Content] nvarchar(1000) NOT NULL,
        [Rating] tinyint NULL,
        [IsHidden] bit NOT NULL DEFAULT CAST(0 AS bit),
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Comment] PRIMARY KEY ([CommentId]),
        CONSTRAINT [CK_Comment_Rating] CHECK ([Rating] IS NULL OR [Rating] BETWEEN 1 AND 5),
        CONSTRAINT [FK_Comment_Account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Account] ([AccountId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Comment_Comment_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Comment] ([CommentId]),
        CONSTRAINT [FK_Comment_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Project] ([ProjectId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [ContactRequest] (
        [ContactId] int NOT NULL IDENTITY,
        [ProjectId] int NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Email] varchar(100) NOT NULL,
        [Phone] varchar(15) NULL,
        [Organization] nvarchar(150) NULL,
        [RequestType] tinyint NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [Status] tinyint NOT NULL DEFAULT CAST(0 AS tinyint),
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        [HandledBy] int NULL,
        CONSTRAINT [PK_ContactRequest] PRIMARY KEY ([ContactId]),
        CONSTRAINT [FK_ContactRequest_Account_HandledBy] FOREIGN KEY ([HandledBy]) REFERENCES [Account] ([AccountId]),
        CONSTRAINT [FK_ContactRequest_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Project] ([ProjectId]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [ProjectFeature] (
        [FeatureId] int NOT NULL IDENTITY,
        [ProjectId] int NOT NULL,
        [Title] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Icon] nvarchar(100) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_ProjectFeature] PRIMARY KEY ([FeatureId]),
        CONSTRAINT [FK_ProjectFeature_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Project] ([ProjectId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [ProjectImage] (
        [ImageId] int NOT NULL IDENTITY,
        [ProjectId] int NOT NULL,
        [ImageUrl] nvarchar(255) NOT NULL,
        [Caption] nvarchar(200) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_ProjectImage] PRIMARY KEY ([ImageId]),
        CONSTRAINT [FK_ProjectImage_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Project] ([ProjectId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [ProjectVersion] (
        [VersionId] int NOT NULL IDENTITY,
        [ProjectId] int NOT NULL,
        [VersionName] nvarchar(100) NOT NULL,
        [Platform] nvarchar(100) NULL,
        [ReleaseDate] date NULL,
        [ChangeLog] nvarchar(max) NULL,
        [DownloadUrl] nvarchar(255) NULL,
        [IsPublic] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_ProjectVersion] PRIMARY KEY ([VersionId]),
        CONSTRAINT [FK_ProjectVersion_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Project] ([ProjectId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [TeamMember] (
        [MemberId] int NOT NULL IDENTITY,
        [ProjectId] int NOT NULL,
        [AccountId] int NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Position] nvarchar(100) NULL,
        [StudentCode] varchar(20) NULL,
        [ClassName] nvarchar(50) NULL,
        [Avatar] nvarchar(255) NULL,
        [Email] varchar(100) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_TeamMember] PRIMARY KEY ([MemberId]),
        CONSTRAINT [FK_TeamMember_Account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Account] ([AccountId]),
        CONSTRAINT [FK_TeamMember_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Project] ([ProjectId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE TABLE [Vote] (
        [ProjectId] int NOT NULL,
        [AccountId] int NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Vote] PRIMARY KEY ([ProjectId], [AccountId]),
        CONSTRAINT [FK_Vote_Account_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Account] ([AccountId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Vote_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Project] ([ProjectId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Account_Email] ON [Account] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Account_FacultyId] ON [Account] ([FacultyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Account_RoleId] ON [Account] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Category_CategoryName] ON [Category] ([CategoryName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Category_Slug] ON [Category] ([Slug]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Comment_AccountId] ON [Comment] ([AccountId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Comment_ParentId] ON [Comment] ([ParentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Comment_ProjectId] ON [Comment] ([ProjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ContactRequest_HandledBy] ON [ContactRequest] ([HandledBy]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ContactRequest_ProjectId] ON [ContactRequest] ([ProjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Faculty_FacultyName] ON [Faculty] ([FacultyName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_News_AuthorId] ON [News] ([AuthorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_News_Slug] ON [News] ([Slug]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Project_ApprovedBy] ON [Project] ([ApprovedBy]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Project_CategoryId] ON [Project] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Project_FacultyId] ON [Project] ([FacultyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Project_OwnerId] ON [Project] ([OwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Project_Slug] ON [Project] ([Slug]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Project_Status] ON [Project] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ProjectFeature_ProjectId] ON [ProjectFeature] ([ProjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ProjectImage_ProjectId] ON [ProjectImage] ([ProjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ProjectVersion_ProjectId] ON [ProjectVersion] ([ProjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Role_RoleName] ON [Role] ([RoleName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TeamMember_AccountId] ON [TeamMember] ([AccountId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TeamMember_ProjectId] ON [TeamMember] ([ProjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Vote_AccountId] ON [Vote] ([AccountId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261028061433_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261028061433_InitialCreate', N'8.0.10');
END;
GO

COMMIT;
GO

