CREATE TABLE [dbo].[CharacterSkill]
(
	[CallID] UNIQUEIDENTIFIER,
	[ActiveSkillLevel] INT NULL,
	[SkillId] INT NULL,
	[SkillpointsInSkill] BIGINT NULL,
	[TrainedSkillLevel] INT NULL
)
GO
CREATE CLUSTERED INDEX IX_CharacterSkill_CallID ON dbo.[CharacterSkill] (CallID)
