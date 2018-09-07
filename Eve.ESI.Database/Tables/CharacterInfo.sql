CREATE TABLE [dbo].[CharacterInfo]
(
	[CallID] UNIQUEIDENTIFIER,
	[AllianceId] INT NULL,
	[AncestryId] INT NULL,
	[Birthday] DATETIME NULL,
	[BloodlineId] INT NULL,
	[CorporationId] INT NULL,
	[Description] VARCHAR(MAX) NULL,
	[FactionId] INT NULL,
	[Gender] VARCHAR(MAX) NULL,
	[Name] VARCHAR(MAX) NULL,
	[RaceId] INT NULL,
	[SecurityStatus] FLOAT NULL
)
GO
CREATE CLUSTERED INDEX IX_CharacterInfo_CallID ON dbo.[CharacterInfo] (CallID)
