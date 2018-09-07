CREATE TABLE [dbo].[CharacterTitles]
(
	[CallID] UNIQUEIDENTIFIER,
	[Name] VARCHAR(MAX) NULL,
	[TitleId] INT NULL
)
GO
CREATE CLUSTERED INDEX IX_CharacterTitles_CallID ON dbo.[CharacterTitles] (CallID)
