CREATE PROCEDURE [dbo].[pESICallResponse]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@CallID UNIQUEIDENTIFIER = NULL,
	@ParameterGuid UNIQUEIDENTIFIER = NULL,
	@Uri VARCHAR(MAX) = NULL,
	@Executed DATETIME = NULL,
	@ResponseCode SMALLINT = NULL,
	@ETag VARCHAR(MAX) = NULL,
	@Expires DATETIME = NULL,
	@LastModified DATETIME = NULL,
	@Page INT = NULL,
	@Pages INT = NULL,
	@TableName VARCHAR(MAX) = NULL
AS
BEGIN
	IF @Result = 'Save'
	BEGIN
		
		IF NOT EXISTS (SELECT* FROM ESICallResponse WHERE CallID = @CallID)
		BEGIN
			INSERT INTO ESICallResponse(CallID,ParameterGuid,Uri,Executed,ResponseCode,ETag,Expires,LastModified,Page,Pages)
			VALUES(@CallID,@ParameterGuid,@Uri,@Executed,@ResponseCode,@ETag,@Expires,@LastModified,@Page,@Pages)

			SELECT @@IDENTITY AS [Value]
		END
		ELSE
		BEGIN
			
			UPDATE ESICallResponse
			SET Executed = @Executed,
			ResponseCode = @ResponseCode,
			Etag = @Etag,
			Expires = @Expires,
			LastModified = @LastModified,
			Page = @Page,
			Pages = @Pages
			WHERE CallID = @CallID


			SELECT Id AS [Value] FROM ESICallResponse WHERE CallID = @CallID
		END
		
		
	

	END

	IF @Result = 'Single'
	BEGIN

		SELECT*
		FROM ESICallResponse
		WHERE Id = @Id

	END

	IF @Result = 'ParameterHash'
	BEGIN

		SELECT*
		FROM ESICallResponse
		WHERE ParameterGuid = @ParameterGuid

	END

	IF @Result = 'SelectData'
	BEGIN
		DECLARE @Select VARCHAR(500) = 'SELECT* FROM '+ @TableName+ ' WHERE CallID = ''' + CONVERT(NVARCHAR(50),@CallID) + ''''
		EXEC (@Select)
	END

	IF @Result = 'ClearData'
	BEGIN

		DECLARE @Clear VARCHAR(500) = 'DELETE FROM '+ @TableName+ ' WHERE CallID = ''' + CONVERT(NVARCHAR(50),@CallID) + ''''
		EXEC (@Clear)

	END
END