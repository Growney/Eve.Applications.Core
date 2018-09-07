CREATE PROCEDURE [dbo].[pRole]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@Permissions INT = NULL,
	@Ordinal INT = NULL,
	@Name VARCHAR(200) = NULL
AS

IF @Result = 'Save'
BEGIN

	IF NOT EXISTS (SELECT* FROM Role WHERE Id = @Id)
	BEGIN

		INSERT INTO Role ([Permissions],[Ordinal],[Name])
		VALUES(@Permissions,@Ordinal,@Name)

		SELECT @@IDENTITY AS [Value]

	END
	ELSE
	BEGIN
		
		UPDATE [Role]
		SET [Permissions] = @Permissions,
		Ordinal = @Ordinal,
		[Name] = @Name
		WHERE Id = @Id

		SELECT @Id AS [Value]

	END
END


IF @Result = 'Delete'
BEGIN

	DELETE FROM [Role] WHERE Id = @Id

END

IF @Result = 'Single'
BEGIN

	SELECT * FROM [Role] WHERE Id = @Id

END

IF @Result = 'All'
BEGIN

	SELECT * FROM [Role]

END