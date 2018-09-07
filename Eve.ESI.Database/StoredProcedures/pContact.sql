CREATE PROCEDURE [dbo].[pContact]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@ContactId INT = NULL,
	@ContactType INT = NULL,
	@IsBlocked BIT = NULL,
	@IsWatched BIT = NULL,
	@Standing FLOAT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO Contact (CallID,ContactId,ContactType,IsBlocked,IsWatched,Standing)
	VALUES (@CallID,@ContactId,@ContactType,@IsBlocked,@IsWatched,@Standing)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM Contact
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM Contact WHERE CallID = @CallID
END

END
