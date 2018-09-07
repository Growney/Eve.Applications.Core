CREATE PROCEDURE [dbo].[pIDIntegerList]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@Id BIGINT = NULL,
	@Value BIGINT = NULL
AS
BEGIN
	IF @Result = 'Save'
	BEGIN

		INSERT INTO IDIntegerList(CallID,Id,Value)
		SELECT @CallID,@Id,@Value

		SELECT @Id AS [Value]

	END

	IF @Result = 'ForRequest'
	BEGIN

		SELECT*
		FROM IDIntegerList
		WHERE CallID = @CallID

	END

	IF @Result = 'ForIdCallID'
	BEGIN

		SELECT*
		FROM IDIntegerList
		WHERE CallID = @CallID
		AND Id = @ID

	END
END
