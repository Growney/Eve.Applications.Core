CREATE PROCEDURE [dbo].[pAccountLink]
	@Result NCHAR(50),
	@ID BIGINT = NULL,
	@TypeID TINYINT = NULL,
	@AccountGuid UNIQUEIDENTIFIER = NULL,
	@Link VARCHAR(MAX) = NULL
AS
IF @Result = 'Link'
BEGIN

	IF NOT EXISTS (SELECT * FROM AccountLink WHERE AccountGuid = @AccountGuid AND TypeID = @TypeID)
	BEGIN

		INSERT INTO AccountLink(TypeID,AccountGuid,Link)
		VALUES (@TypeID,@AccountGuid,@Link)

	END
	ELSE
	BEGIN

		UPDATE AccountLink
		SET Link = @Link
		WHERE AccountGuid = @AccountGuid AND TypeID = @TypeID

	END

END

IF @Result = 'Remove'
BEGIN

	DELETE FROM AccountLink
	WHERE AccountGuid = @AccountGuid AND TypeID = @TypeID

END

IF @Result = 'ForLink'
BEGIN

	SELECT*,Link,TypeID
	FROM UserAccount UA
	INNER JOIN AccountLink AL ON UA.AccountGuid = AL.AccountGuid
	WHERE Link = @Link

END

IF @Result = 'GetLink'
BEGIN

	SELECT Link AS [Value]
	FROM AccountLink
	WHERE AccountGuid = @AccountGuid

END

IF @Result = 'AllLinked'
BEGIN

	SELECT*,Link,TypeID
	FROM UserAccount UA
	INNER JOIN AccountLink AL ON UA.AccountGuid = AL.AccountGuid
	WHERE TypeID = @TypeID
	
END