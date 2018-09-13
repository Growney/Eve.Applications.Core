CREATE PROCEDURE [dbo].[pToken]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@EntityType TINYINT = NULL,
	@AccessToken VARCHAR(MAX) = NULL,
	@TokenType VARCHAR(50) = NULL,
	@ExpiresIn INT = NULL,
	@RefreshToken VARCHAR(MAX) = NULL,
	@EntityID BIGINT = NULL,
	@EntityName VARCHAR(150) = NULL,
	@ExpiresOn DATETIME = NULL,
	@Scopes VARCHAR(MAX) = NULL,
	@CharacterOwnerHash VARCHAR(200) = NULL,
	@Ids LongIDList READONLY,
	@Scope VARCHAR(50) = NULL
AS
IF @Result = 'Save'
BEGIN

	IF NOT EXISTS (SELECT* FROM Token WHERE Id = @Id)
	BEGIN
		
		INSERT INTO Token(AccessToken,TokenType,ExpiresIn,RefreshToken,EntityType,EntityID,EntityName,ExpiresOn,Scopes,CharacterOwnerHash)
		VALUES(@AccessToken,@TokenType,@ExpiresIn,@RefreshToken,@EntityType,@EntityID,@EntityName,@ExpiresOn,@Scopes,@CharacterOwnerHash)

		SELECT @@IDENTITY AS [Value]

	END
	ELSE
	BEGIN
		
		UPDATE Token
		SET AccessToken = @AccessToken,
		ExpiresOn = @ExpiresOn,
		ExpiresIn = @ExpiresIn
		WHERE Id = @Id

		SELECT @Id AS [Value]
	END

END

IF @Result = 'ForId'
BEGIN

	SELECT*
	FROM Token
	WHERE Id IN (SELECT Id FROM @Ids)

END

IF @Result = 'ForScope'
BEGIN

	SELECT*
	FROM Token
	WHERE Id IN (SELECT Id FROM @Ids)
	AND Scopes LIKE '%' + @Scope + '%'
	AND EntityID = @EntityID
	AND EntityType = @EntityType

END

IF @Result = 'Single'
BEGIN

	SELECT*
	FROM Token
	WHERE Id = @Id

END

