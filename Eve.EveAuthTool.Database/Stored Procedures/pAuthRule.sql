CREATE PROCEDURE [dbo].[pAuthRule]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@Name VARCHAR(200) = NULL,
	@RoleID BIGINT = NULL,
	@Ordinal INT = NULL
AS

IF @Result = 'Save'
BEGIN

	IF NOT EXISTS (SELECT* FROM Role WHERE Id = @Id)
	BEGIN

		INSERT INTO [AuthRule] ([Name],[RoleID],Ordinal)
		VALUES(@Name,@RoleID,@Ordinal)

		SELECT @@IDENTITY AS [Value]

	END
	ELSE
	BEGIN
		
		UPDATE [AuthRule]
		SET [Name] = @Name,
		RoleID = @RoleID,
		Ordinal = @Ordinal
		WHERE Id = @Id

		SELECT @Id AS [Value]

	END
END


IF @Result = 'Delete'
BEGIN

	DELETE FROM [AuthRule] WHERE Id = @Id

END

IF @Result = 'Single'
BEGIN

	SELECT * FROM [AuthRule] WHERE Id = @Id

END

IF @Result = 'All'
BEGIN

	SELECT* FROM [AuthRule]

END