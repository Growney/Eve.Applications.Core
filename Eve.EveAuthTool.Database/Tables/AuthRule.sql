﻿CREATE TABLE [dbo].[AuthRule]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] VARCHAR(200) NOT NULL,
	[RoleID] BIGINT NOT NULL, 
    [Ordinal] INT NOT NULL 
)