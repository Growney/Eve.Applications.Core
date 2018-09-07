CREATE PROCEDURE [dbo].[pCharacterAttributes]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@AccruedRemapCooldownDate DATETIME = NULL,
	@BonusRemaps INT = NULL,
	@Charisma INT = NULL,
	@Intelligence INT = NULL,
	@LastRemapDate DATETIME = NULL,
	@Memory INT = NULL,
	@Perception INT = NULL,
	@Willpower INT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CharacterAttributes (CallID,AccruedRemapCooldownDate,BonusRemaps,Charisma,Intelligence,LastRemapDate,Memory,Perception,Willpower)
	VALUES (@CallID,@AccruedRemapCooldownDate,@BonusRemaps,@Charisma,@Intelligence,@LastRemapDate,@Memory,@Perception,@Willpower)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CharacterAttributes
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CharacterAttributes WHERE CallID = @CallID
END

END
