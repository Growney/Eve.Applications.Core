﻿CREATE TABLE [dbo].[Role]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Permissions] BIGINT NOT NULL, 
    [Ordinal] INT NOT NULL, 
    [Name] VARCHAR(200) NOT NULL,
	[DiscordRoleConfigurationID] BIGINT
)
