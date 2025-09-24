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
CREATE TABLE [Tasks] (
    [Id] bigint NOT NULL IDENTITY,
    [TaskId] bigint NOT NULL,
    [UserId] bigint NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Deadline] datetime2 NULL,
    [Priority] int NOT NULL,
    [Status] int NOT NULL,
    [Uuid] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Tasks] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] bigint NOT NULL IDENTITY,
    [Username] nvarchar(255) NOT NULL,
    [NormalizedUsername] nvarchar(255) NOT NULL,
    [Password] nvarchar(255) NOT NULL,
    [FirstName] nvarchar(50) NULL,
    [LastName] nvarchar(50) NULL,
    [Gender] int NOT NULL,
    [Uuid] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [UserSessions] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [RefreshToken] nvarchar(250) NULL,
    [JwtId] nvarchar(36) NULL,
    [RefreshTokenExpiration] datetime2 NOT NULL,
    [Redeemed] bit NOT NULL,
    [Uuid] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_UserSessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserSessions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);

CREATE INDEX [IX_UserSessions_UserId] ON [UserSessions] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250922084919_AddedUserFieldsToEdit', N'9.0.9');

COMMIT;
GO

