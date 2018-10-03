CREATE TABLE [dbo].[DiscordRoleConfigurationRole]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[DiscordRoleConfigurationId] BIGINT NOT NULL,
	[DiscordRoleId] BINARY(8) NOT NULL
)
