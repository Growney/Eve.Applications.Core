CREATE TABLE [dbo].[AllianceInfo]
(
	[CallID] UNIQUEIDENTIFIER,
	[CreatorCorporationId] INT NULL,
	[CreatorId] INT NULL,
	[DateFounded] DATETIME NULL,
	[ExecutorCorporationId] INT NULL,
	[FactionId] INT NULL,
	[Name] VARCHAR(MAX) NULL,
	[Ticker] VARCHAR(MAX) NULL
)
GO
CREATE CLUSTERED INDEX IX_AllianceInfo_CallID ON dbo.[AllianceInfo] (CallID)
