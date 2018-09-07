CREATE PROCEDURE [dbo].[pCharacterRoles]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@Roles BIGINT = NULL,
	@RolesAtBase BIGINT = NULL,
	@RolesAtHq BIGINT = NULL,
	@RolesAtOther BIGINT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CharacterRoles (CallID,Roles,RolesAtBase,RolesAtHq,RolesAtOther)
	VALUES (@CallID,@Roles,@RolesAtBase,@RolesAtHq,@RolesAtOther)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CharacterRoles
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CharacterRoles WHERE CallID = @CallID
END

END
