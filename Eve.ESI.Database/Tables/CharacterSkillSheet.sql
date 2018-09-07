CREATE TABLE [dbo].[CharacterSkillSheet]
(
	[CallID] UNIQUEIDENTIFIER,
	[TotalSp] BIGINT NULL,
	[UnallocatedSp] INT NULL
)
GO
CREATE CLUSTERED INDEX IX_CharacterSkillSheet_CallID ON dbo.[CharacterSkillSheet] (CallID)
