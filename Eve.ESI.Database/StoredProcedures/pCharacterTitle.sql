CREATE PROCEDURE [dbo].[pCharacterTitle]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@Name VARCHAR(MAX) = NULL,
	@TitleId INT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CharacterTitle (CallID,Name,TitleId)
	VALUES (@CallID,@Name,@TitleId)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CharacterTitle
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CharacterTitle WHERE CallID = @CallID
END

END
