CREATE PROCEDURE [dbo].[pAuthRuleRelationship]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@RuleID BIGINT = NULL,
	@EntityID BIGINT = NULL,
	@EntityType INT = NULL,
	@Relationship INT = NULL,
	@ParentEntityID BIGINT = NULL,
	@ParentEntityType INT = NULL
AS

IF @Result = 'Save'
BEGIN

	IF NOT EXISTS (SELECT* FROM Role WHERE Id = @Id)
	BEGIN

		INSERT INTO [AuthRuleRelationship] (RuleID,EntityID,EntityType,Relationship,ParentEntityID,ParentEntityType)
		VALUES(@RuleID,@EntityID,@EntityType,@Relationship,@ParentEntityID,@ParentEntityType)

		SELECT @@IDENTITY AS [Value]

	END
	ELSE
	BEGIN
		
		UPDATE [AuthRuleRelationship]
		SET RuleID = @RuleID,
		EntityID = @EntityID,
		EntityType = @EntityType,
		Relationship = @Relationship,
		ParentEntityID = @ParentEntityID,
		ParentEntityType = @ParentEntityType
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

IF @Result = 'DeleteForRule'
BEGIN

	DELETE FROM [AuthRuleRelationship] WHERE RuleID = @RuleID

END