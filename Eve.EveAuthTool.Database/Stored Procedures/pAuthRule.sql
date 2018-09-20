CREATE PROCEDURE [dbo].[pAuthRule]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@Name VARCHAR(200) = NULL,
	@RoleID BIGINT = NULL,
	@Ordinal INT = NULL,
	@MatchAll BIT = NULL
AS

IF @Result = 'Save'
BEGIN

	IF NOT EXISTS (SELECT* FROM [AuthRule] WHERE Id = @Id)
	BEGIN

		INSERT INTO [AuthRule] ([Name],[RoleID],Ordinal,MatchAll)
		VALUES(@Name,@RoleID,@Ordinal,@MatchAll)

		SELECT @@IDENTITY AS [Value]

	END
	ELSE
	BEGIN
		
		UPDATE [AuthRule]
		SET [Name] = @Name,
		RoleID = @RoleID,
		Ordinal = @Ordinal,
		MatchAll = @MatchAll
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