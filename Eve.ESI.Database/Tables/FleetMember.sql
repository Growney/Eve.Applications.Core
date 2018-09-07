CREATE TABLE [dbo].[FleetMember]
(
	[CallID] UNIQUEIDENTIFIER,
	[CharacterId] INT NULL,
	[JoinTime] DATETIME NULL,
	[Role] INT NULL,
	[RoleName] VARCHAR(MAX) NULL,
	[ShipTypeId] INT NULL,
	[SolarSystemId] INT NULL,
	[SquadId] BIGINT NULL,
	[StationId] BIGINT NULL,
	[TakesFleetWarp] BIT NULL,
	[WingId] BIGINT NULL
)
GO
CREATE CLUSTERED INDEX IX_FleetMember_CallID ON dbo.[FleetMember] (CallID)
