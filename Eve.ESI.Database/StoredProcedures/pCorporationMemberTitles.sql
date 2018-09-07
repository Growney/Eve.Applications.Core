CREATE PROCEDURE [dbo].[pCorporationMemberTitles]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@CharacterId INT = NULL,
	@Titles VARCHAR(MAX) = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CorporationMemberTitles (CallID,CharacterId,Titles)
	VALUES (@CallID,@CharacterId,@Titles)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CorporationMemberTitles
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CorporationMemberTitles WHERE CallID = @CallID
END

END
