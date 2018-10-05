CREATE PROCEDURE [dbo].[pDiscordLinkParameter]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@LinkGuid UNIQUEIDENTIFIER = NULL,
	@TenantID BIGINT = NULL
AS
BEGIN

	DELETE 
	FROM DiscordLinkParameter 
	WHERE Expires < GETUTCDATE()

	IF @Result = 'Save'
	BEGIN

		IF NOT EXISTS (SELECT* FROM DiscordLinkParameter WHERE LinkGuid = @LinkGuid)
		BEGIN

			INSERT INTO DiscordLinkParameter (LinkGuid,TenantID,Expires)
			VALUES(@LinkGuid,@TenantID,DATEADD(MINUTE,15,GETUTCDATE()))

		END

		SELECT 0 AS [Value]

	END

	IF @Result = 'Recall'
	BEGIN

		SELECT*
		FROM DiscordLinkParameter
		WHERE LinkGuid = @LinkGuid

	END

	IF @Result = 'Discard'
	BEGIN

		DELETE 
		FROM DiscordLinkParameter 
		WHERE LinkGuid = @LinkGuid

	END

END
