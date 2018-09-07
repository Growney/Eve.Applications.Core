CREATE PROCEDURE [dbo].[pSkillQueueItem]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@FinishDate DATETIME = NULL,
	@FinishedLevel INT = NULL,
	@LevelEndSp INT = NULL,
	@LevelStartSp INT = NULL,
	@QueuePosition INT = NULL,
	@SkillId INT = NULL,
	@StartDate DATETIME = NULL,
	@TrainingStartSp INT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO SkillQueueItem (CallID,FinishDate,FinishedLevel,LevelEndSp,LevelStartSp,QueuePosition,SkillId,StartDate,TrainingStartSp)
	VALUES (@CallID,@FinishDate,@FinishedLevel,@LevelEndSp,@LevelStartSp,@QueuePosition,@SkillId,@StartDate,@TrainingStartSp)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM SkillQueueItem
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM SkillQueueItem WHERE CallID = @CallID
END

END
