CREATE TABLE [dbo].[ESIToken]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[EntityType] TINYINT NOT NULL,
	[AccessToken] VARCHAR(MAX) NOT NULL,
	[TokenType] VARCHAR(50) NOT NULL,
	[ExpiresIn] INT NOT NULL,
	[RefreshToken] VARCHAR(MAX) NOT NULL,
	[CharacterID] BIGINT NOT NULL,
	[ExpiresOn] DATETIME NOT NULL,
	[Scopes] VARCHAR(MAX) NOT NULL,
	[CharacterOwnerHash] VARCHAR(200) NOT NULL, 
    [CorporationID] INT NOT NULL, 
    [AllianceID] INT NOT NULL
)
