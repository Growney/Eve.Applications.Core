CREATE PROCEDURE [dbo].[pEntityRelationship]
	@Result NVARCHAR(50),
	@Id BIGINT = NULL,
	@FromEntityID BIGINT  = NULL,
	@FromEntityType INT = NULL,
	@ToEntityID BIGINT = NULL,
	@ToEntityType INT = NULL,
	@Relationship INT = NULL,
	@CheckedOn DATETIME = NULL
AS
BEGIN

	IF @Result = 'Save'
	BEGIN

		IF NOT EXISTS (
			SELECT * FROM EntityRelationship 
			WHERE FromEntityID = @FromEntityID AND FromEntityType = @FromEntityType 
			AND ToEntityID = @ToEntityID AND ToEntityType = @ToEntityType
		)
		BEGIN
			
			INSERT INTO EntityRelationship(FromEntityID,FromEntityType,ToEntityID,ToEntityType,Relationship,CheckedOn)
			VALUES(@FromEntityID,@FromEntityType,@ToEntityID,@ToEntityType,@Relationship,@CheckedOn)

			SELECT @@IDENTITY AS [Value]
			
		END
		ELSE
		BEGIN
			
			UPDATE EntityRelationship
			SET Relationship = Relationship | @Relationship,
			CheckedOn = @CheckedOn
			FROM EntityRelationship
			WHERE FromEntityID = @FromEntityID AND FromEntityType = @FromEntityType 
			AND ToEntityID = @ToEntityID AND ToEntityType = @ToEntityType

			SELECT Id AS [Value]
			FROM EntityRelationship
			WHERE FromEntityID = @FromEntityID AND FromEntityType = @FromEntityType 
			AND ToEntityID = @ToEntityID AND ToEntityType = @ToEntityType

		END

	END

	IF @Result = 'RemoveForFrom'
	BEGIN

		DELETE FROM EntityRelationship
		WHERE FromEntityID = @FromEntityID AND FromEntityType = @FromEntityType 

	END

	IF @Result = 'RemoveForFromTo'
	BEGIN

		DELETE FROM EntityRelationship
		WHERE FromEntityID = @FromEntityID AND FromEntityType = @FromEntityType 
			AND ToEntityID = @ToEntityID AND ToEntityType = @ToEntityType

	END

	IF @Result = 'SelectForFrom'
	BEGIN

		SELECT*
		FROM EntityRelationship
		WHERE FromEntityID = @FromEntityID AND FromEntityType = @FromEntityType 

	END

	IF @Result = 'SelectForTo'
	BEGIN

		SELECT*
		FROM EntityRelationship
		WHERE ToEntityID = @ToEntityID AND ToEntityType = @ToEntityType

	END

	IF @Result = 'SelectNotCheckedSince'
	BEGIN

		SELECT*
		FROM EntityRelationship
		WHERE CheckedOn < @CheckedOn

	END

	IF @Result = 'SelectNotCheckedSinceType'
	BEGIN

		SELECT*
		FROM EntityRelationship
		WHERE CheckedOn < @CheckedOn
		AND Relationship & @Relationship <> 0

	END
END
