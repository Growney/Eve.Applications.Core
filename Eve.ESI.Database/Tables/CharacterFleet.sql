CREATE TABLE [dbo].[CharacterFleet]
(
	[CallID] UNIQUEIDENTIFIER,
	[FleetId] BIGINT NULL,
	[Role] INT NULL,
	[SquadId] BIGINT NULL,
	[WingId] BIGINT NULL
)
GO
CREATE CLUSTERED INDEX IX_CharacterFleet_CallID ON dbo.[CharacterFleet] (CallID)
