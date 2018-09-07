CREATE TABLE [dbo].[CorporationMemberTitles]
(
	[CallID] UNIQUEIDENTIFIER,
	[CharacterId] INT NULL,
	[Titles] VARCHAR(MAX) NULL
)
GO
CREATE CLUSTERED INDEX IX_CorporationMemberTitles_CallID ON dbo.[CorporationMemberTitles] (CallID)
