CREATE PROCEDURE [dbo].[pCharacterInfo]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@AllianceId INT = NULL,
	@AncestryId INT = NULL,
	@Birthday DATETIME = NULL,
	@BloodlineId INT = NULL,
	@CorporationId INT = NULL,
	@Description VARCHAR(MAX) = NULL,
	@FactionId INT = NULL,
	@Gender VARCHAR(MAX) = NULL,
	@Name VARCHAR(MAX) = NULL,
	@RaceId INT = NULL,
	@SecurityStatus FLOAT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CharacterInfo (CallID,AllianceId,AncestryId,Birthday,BloodlineId,CorporationId,Description,FactionId,Gender,Name,RaceId,SecurityStatus)
	VALUES (@CallID,@AllianceId,@AncestryId,@Birthday,@BloodlineId,@CorporationId,@Description,@FactionId,@Gender,@Name,@RaceId,@SecurityStatus)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CharacterInfo
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CharacterInfo WHERE CallID = @CallID
END

END
