CREATE PROCEDURE [dbo].[pCharacterSkill]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@ActiveSkillLevel INT = NULL,
	@SkillId INT = NULL,
	@SkillpointsInSkill BIGINT = NULL,
	@TrainedSkillLevel INT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CharacterSkill (CallID,ActiveSkillLevel,SkillId,SkillpointsInSkill,TrainedSkillLevel)
	VALUES (@CallID,@ActiveSkillLevel,@SkillId,@SkillpointsInSkill,@TrainedSkillLevel)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CharacterSkill
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CharacterSkill WHERE CallID = @CallID
END

END
