CREATE TABLE [dbo].[SkillQueueItem]
(
	[CallID] UNIQUEIDENTIFIER,
	[FinishDate] DATETIME NULL,
	[FinishedLevel] INT NULL,
	[LevelEndSp] INT NULL,
	[LevelStartSp] INT NULL,
	[QueuePosition] INT NULL,
	[SkillId] INT NULL,
	[StartDate] DATETIME NULL,
	[TrainingStartSp] INT NULL
)
GO
CREATE CLUSTERED INDEX IX_SkillQueueItem_CallID ON dbo.[SkillQueueItem] (CallID)
