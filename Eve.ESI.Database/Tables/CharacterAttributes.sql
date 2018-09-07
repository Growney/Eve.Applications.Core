CREATE TABLE [dbo].[CharacterAttributes]
(
	[CallID] UNIQUEIDENTIFIER,
	[AccruedRemapCooldownDate] DATETIME NULL,
	[BonusRemaps] INT NULL,
	[Charisma] INT NULL,
	[Intelligence] INT NULL,
	[LastRemapDate] DATETIME NULL,
	[Memory] INT NULL,
	[Perception] INT NULL,
	[Willpower] INT NULL
)
GO
CREATE CLUSTERED INDEX IX_CharacterAttributes_CallID ON dbo.[CharacterAttributes] (CallID)
