CREATE PROCEDURE [dbo].[pCharacterTitles]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@Name VARCHAR(MAX) = NULL,
	@TitleId INT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CharacterTitles (CallID,Name,TitleId)
	VALUES (@CallID,@Name,@TitleId)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CharacterTitles
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CharacterTitles WHERE CallID = @CallID
END

END
