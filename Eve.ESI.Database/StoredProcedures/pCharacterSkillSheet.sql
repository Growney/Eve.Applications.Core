CREATE PROCEDURE [dbo].[pCharacterSkillSheet]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@TotalSp BIGINT = NULL,
	@UnallocatedSp INT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CharacterSkillSheet (CallID,TotalSp,UnallocatedSp)
	VALUES (@CallID,@TotalSp,@UnallocatedSp)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CharacterSkillSheet
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CharacterSkillSheet WHERE CallID = @CallID
END

END
