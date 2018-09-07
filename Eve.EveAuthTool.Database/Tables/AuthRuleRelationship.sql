CREATE TABLE [dbo].[AuthRuleRelationship]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [RuleID] BIGINT NOT NULL, 
    [EntityID] BIGINT NOT NULL, 
    [EntityType] INT NOT NULL, 
    [Relationship] INT NOT NULL
)
