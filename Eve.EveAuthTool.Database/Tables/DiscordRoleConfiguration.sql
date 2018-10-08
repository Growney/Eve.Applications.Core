CREATE TABLE [dbo].[DiscordRoleConfiguration]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Name] VARCHAR(100) NOT NULL, 
    [GuildInviteID] VARCHAR(100) NULL
)
