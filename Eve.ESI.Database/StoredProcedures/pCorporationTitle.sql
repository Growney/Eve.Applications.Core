CREATE PROCEDURE [dbo].[pCorporationTitle]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@GrantableRoles BIGINT = NULL,
	@GrantableRolesAtBase BIGINT = NULL,
	@GrantableRolesAtHq BIGINT = NULL,
	@GrantableRolesAtOther BIGINT = NULL,
	@Name VARCHAR(MAX) = NULL,
	@Roles BIGINT = NULL,
	@RolesAtBase BIGINT = NULL,
	@RolesAtHq BIGINT = NULL,
	@RolesAtOther BIGINT = NULL,
	@TitleId INT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CorporationTitle (CallID,GrantableRoles,GrantableRolesAtBase,GrantableRolesAtHq,GrantableRolesAtOther,Name,Roles,RolesAtBase,RolesAtHq,RolesAtOther,TitleId)
	VALUES (@CallID,@GrantableRoles,@GrantableRolesAtBase,@GrantableRolesAtHq,@GrantableRolesAtOther,@Name,@Roles,@RolesAtBase,@RolesAtHq,@RolesAtOther,@TitleId)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CorporationTitle
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CorporationTitle WHERE CallID = @CallID
END
IF @Result = 'ForTitleID'
BEGIN
	SELECT* FROM CorporationTitle WHERE TitleId = @TitleId
END
END
