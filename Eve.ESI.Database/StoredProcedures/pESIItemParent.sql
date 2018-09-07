CREATE PROCEDURE [dbo].[pESIItemParent]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER = NULL
AS
BEGIN
	IF @Result = 'Save'
	BEGIN

		INSERT INTO [ESIItemParent](CallID)
		VALUES(@CallID)

		SELECT 0 AS [Value]

	END
	IF @Result = 'ForRequest'
	BEGIN

		SELECT * FROM ESIItemParent WHERE CallID = @CallID

	END
END