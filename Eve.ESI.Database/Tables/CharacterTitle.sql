CREATE TABLE [dbo].[CharacterTitle]
(
	[CallID] UNIQUEIDENTIFIER,
	[Name] VARCHAR(MAX) NULL,
	[TitleId] INT NULL
)
GO
CREATE CLUSTERED INDEX IX_CharacterTitles_CallID ON dbo.[CharacterTitle] (CallID)
