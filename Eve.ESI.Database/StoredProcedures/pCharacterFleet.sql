CREATE PROCEDURE [dbo].[pCharacterFleet]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@FleetId BIGINT = NULL,
	@Role INT = NULL,
	@SquadId BIGINT = NULL,
	@WingId BIGINT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CharacterFleet (CallID,FleetId,Role,SquadId,WingId)
	VALUES (@CallID,@FleetId,@Role,@SquadId,@WingId)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CharacterFleet
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CharacterFleet WHERE CallID = @CallID
END

END
