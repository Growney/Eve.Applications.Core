CREATE TABLE [dbo].[CorporationInfo]
(
	[CallID] UNIQUEIDENTIFIER,
	[AllianceId] INT NULL,
	[CeoId] INT NULL,
	[CreatorId] INT NULL,
	[DateFounded] DATETIME NULL,
	[Description] VARCHAR(MAX) NULL,
	[FactionId] INT NULL,
	[HomeStationId] INT NULL,
	[MemberCount] INT NULL,
	[Name] VARCHAR(MAX) NULL,
	[Shares] BIGINT NULL,
	[TaxRate] FLOAT NULL,
	[Ticker] VARCHAR(MAX) NULL,
	[Url] VARCHAR(MAX) NULL
)
GO
CREATE CLUSTERED INDEX IX_CorporationInfo_CallID ON dbo.[CorporationInfo] (CallID)
