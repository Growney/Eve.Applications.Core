CREATE TABLE [dbo].[Contact]
(
	[CallID] UNIQUEIDENTIFIER,
	[ContactId] INT NULL,
	[ContactType] INT NULL,
	[IsBlocked] BIT NULL,
	[IsWatched] BIT NULL,
	[Standing] FLOAT NULL
)
GO
CREATE CLUSTERED INDEX IX_Contact_CallID ON dbo.[Contact] (CallID)
