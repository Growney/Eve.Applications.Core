CREATE PROCEDURE [dbo].[pAuthRuleRelationship]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@RuleID BIGINT = NULL,
	@EntityID BIGINT = NULL,
	@EntityType INT = NULL,
	@Relationship INT = NULL
AS

IF @Result = 'Save'
BEGIN

	IF NOT EXISTS (SELECT* FROM Role WHERE Id = @Id)
	BEGIN

		INSERT INTO [AuthRuleRelationship] (RuleID,EntityID,EntityType,Relationship)
		VALUES(@RuleID,@EntityID,@EntityType,@Relationship)

		SELECT @@IDENTITY AS [Value]

	END
	ELSE
	BEGIN
		
		UPDATE [AuthRuleRelationship]
		SET RuleID = @RuleID,
		EntityID = @EntityID,
		EntityType = @EntityType,
		Relationship = @Relationship
		WHERE Id = @Id

		SELECT @Id AS [Value]

	END
END


IF @Result = 'Delete'
BEGIN

	DELETE FROM [AuthRuleRelationship] WHERE Id = @Id

END

IF @Result = 'Single'
BEGIN

	SELECT * FROM [AuthRuleRelationship] WHERE Id = @Id

END

IF @Result = 'ForRule'
BEGIN

	SELECT* FROM [AuthRuleRelationship] WHERE RuleID = @RuleID

END