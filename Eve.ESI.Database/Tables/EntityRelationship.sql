CREATE TABLE [dbo].[EntityRelationship]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [FromEntityID] BIGINT NOT NULL, 
    [FromEntityType] INT NOT NULL, 
    [ToEntityID] BIGINT NOT NULL, 
    [ToEntityType] INT NOT NULL, 
    [Relationship] INT NOT NULL, 
    [CheckedOn] DATETIME NOT NULL
)
