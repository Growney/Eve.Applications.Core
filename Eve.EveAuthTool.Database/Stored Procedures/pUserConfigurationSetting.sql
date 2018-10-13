CREATE PROCEDURE [dbo].[pUserConfigurationSetting]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@Value VARBINARY(MAX) = NULL
AS
IF @Result = 'Save'
BEGIN

	IF NOT EXISTS (SELECT* FROM UserConfigurationSetting WHERE Id = @Id)
	BEGIN

		INSERT INTO UserConfigurationSetting(Id,[Value])
		VALUES(Id,[Value])

	END
	ELSE
	BEGIN

		UPDATE UserConfigurationSetting
		SET Value = @Value
		WHERE Id = @Id

	END

	SELECT @Id AS [Value]

END

IF @Result = 'Single'
BEGIN

	SELECT *
	FROM UserConfigurationSetting
	WHERE Id = @Id

END