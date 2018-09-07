CREATE PROCEDURE [dbo].[pIntegerList]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER = NULL,
	@Value BIGINT = NULL
AS
BEGIN

	IF @Result = 'Save'
	BEGIN

		INSERT INTO IntegerList(CallID,Value)
		VALUES(@CallID,@Value)

		SELECT 0 AS [Value]

	END

	IF @Result = 'All'
	BEGIN

		SELECT* FROM IntegerList

	END

	IF @Result = 'ForRequest'
	BEGIN

		SELECT * FROM IntegerList WHERE CallID = @CallID

	END
END
