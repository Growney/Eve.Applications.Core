CREATE PROCEDURE [dbo].[pUserAccount]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@AccountGuid UNIQUEIDENTIFIER = NULL,
	@CreatedDate DATETIME = NULL,
	@TokenIds LongIDList READONLY,
	@EntityID BIGINT = NULL,
	@EntityType TINYINT = NULL,
	@Main BIT = NULL
AS
IF @Result = 'Single'
BEGIN

	SELECT*
	FROM UserAccount
	WHERE Id = @Id

END

IF @Result = 'TokenIDs'
BEGIN

	SELECT TokenID AS [Value]
	FROM UserAccountToken
	WHERE UserAccountID = @Id

END

IF @Result = 'Save'
BEGIN

	INSERT INTO UserAccount (AccountGuid,CreatedDate)
	VALUES(@AccountGuid,@CreatedDate)

	SELECT @@IDENTITY AS [Value]

	INSERT INTO UserAccountToken (UserAccountID,TokenID)
	SELECT @@IDENTITY,ID
	FROM @TokenIds

END

IF @Result = 'AddTokens'
BEGIN

	INSERT INTO UserAccountToken (UserAccountID,TokenID)
	SELECT @ID,ID
	FROM @TokenIds

END

IF @Result = 'ForGUID'
BEGIN

	SELECT*
	FROM UserAccount
	WHERE AccountGuid = @AccountGuid

END

IF @Result = 'ForEntity'
BEGIN

	SELECT UA.*
	FROM UserAccount UA
	INNER JOIN UserAccountToken UACT ON UA.Id = UACT.UserAccountID
	INNER JOIN [ESIToken] CT ON CT.Id = UACT.TokenID
	WHERE  
	(
		CASE 
		WHEN @EntityType = 0 THEN CharacterID 
		WHEN @EntityType = 1 THEN CorporationID 
		WHEN @EntityType = 2 THEN AllianceID END
	)  = @EntityID 
	AND CT.EntityType = @EntityType

END

IF @Result = 'MainCharacterID'
BEGIN

	
	SELECT TOP 1 CharacterID AS [Value]
	FROM ESIToken ET
	INNER JOIN UserAccountToken UAT ON ET.Id = UAT.TokenID
	WHERE (UAT.Main = 1 OR (SELECT COUNT(*) FROM UserAccountToken MT WHERE MT.Main = 1 AND MT.UserAccountID = @Id) = 0)
	AND UAT.UserAccountID = @Id

END



