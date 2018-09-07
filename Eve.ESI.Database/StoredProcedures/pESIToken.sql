CREATE PROCEDURE [dbo].[pESIToken]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@EntityType TINYINT = NULL,
	@AccessToken VARCHAR(MAX) = NULL,
	@TokenType VARCHAR(50) = NULL,
	@ExpiresIn INT = NULL,
	@RefreshToken VARCHAR(MAX) = NULL,
	@CharacterID BIGINT = NULL,
	@CorporationID INT = NULL,
	@AllianceID INT = NULL,
	@ExpiresOn DATETIME = NULL,
	@Scopes VARCHAR(MAX) = NULL,
	@CharacterOwnerHash VARCHAR(200) = NULL,
	@Ids LongIDList READONLY,
	@Scope VARCHAR(50) = NULL,
	@EntityID BIGINT = NULL
AS
IF @Result = 'Save'
BEGIN

	IF NOT EXISTS (SELECT* FROM [ESIToken] WHERE Id = @Id)
	BEGIN
		
		INSERT INTO [ESIToken](AccessToken,TokenType,ExpiresIn,RefreshToken,EntityType,CharacterID,CorporationID,AllianceID,ExpiresOn,Scopes,CharacterOwnerHash)
		VALUES(@AccessToken,@TokenType,@ExpiresIn,@RefreshToken,@EntityType,@CharacterID,@CorporationID,@AllianceID,@ExpiresOn,@Scopes,@CharacterOwnerHash)

		SELECT @@IDENTITY AS [Value]

	END
	ELSE
	BEGIN
		
		UPDATE [ESIToken]
		SET AccessToken = @AccessToken,
		ExpiresOn = @ExpiresOn,
		ExpiresIn = @ExpiresIn,
		CharacterID = @CharacterID,
		CorporationID = @CorporationID,
		AllianceID = @AllianceID
		WHERE Id = @Id

		SELECT @Id AS [Value]
	END

END

IF @Result = 'ForId'
BEGIN

	SELECT*
	FROM [ESIToken]
	WHERE Id IN (SELECT Id FROM @Ids)

END

IF @Result = 'ForScope'
BEGIN

	SELECT*
	FROM [ESIToken]
	WHERE (Id IN (SELECT Id FROM @Ids) OR (SELECT COUNT(Id) FROM @Ids) = 0)
	AND Scopes LIKE '%' + @Scope + '%'
	AND 
	(
		CASE 
		WHEN @EntityType = 0 THEN CharacterID 
		WHEN @EntityType = 1 THEN CorporationID 
		WHEN @EntityType = 2 THEN AllianceID END
	) = @EntityID
	AND EntityType = @EntityType

END

IF @Result = 'Single'
BEGIN

	SELECT*
	FROM [ESIToken]
	WHERE Id = @Id

END

IF @Result = 'ForEntityTypeAndID'
BEGIN

	SELECT*
	FROM ESIToken
	WHERE EntityType = @EntityType
	AND 
	(
		CASE 
		WHEN @EntityType = 0 THEN CharacterID 
		WHEN @EntityType = 1 THEN CorporationID 
		WHEN @EntityType = 2 THEN AllianceID END
	) = @EntityID

END
