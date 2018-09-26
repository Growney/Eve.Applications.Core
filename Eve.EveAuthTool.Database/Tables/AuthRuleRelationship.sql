CREATE TABLE [dbo].[AuthRuleRelationship]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [RuleID] BIGINT NOT NULL, 
    [EntityID] BIGINT NOT NULL, 
    [EntityType] INT NOT NULL, 
    [Relationship] INT NOT NULL,
	[ParentEntityID] BIGINT NOT NULL CONSTRAINT [DF_AuthRuleRelationship_ParentEntityID] DEFAULT(0), 
    [ParentEntityType] INT NOT NULL CONSTRAINT [DF_AuthRuleRelationship_ParentEntityType] DEFAULT(-1), 
)
