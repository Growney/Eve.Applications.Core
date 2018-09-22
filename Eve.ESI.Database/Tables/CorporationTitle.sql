CREATE TABLE [dbo].[CorporationTitle]
(
	[CallID] UNIQUEIDENTIFIER,
	[GrantableRoles] BIGINT NULL,
	[GrantableRolesAtBase] BIGINT NULL,
	[GrantableRolesAtHq] BIGINT NULL,
	[GrantableRolesAtOther] BIGINT NULL,
	[Name] VARCHAR(MAX) NULL,
	[Roles] BIGINT NULL,
	[RolesAtBase] BIGINT NULL,
	[RolesAtHq] BIGINT NULL,
	[RolesAtOther] BIGINT NULL,
	[TitleId] INT NULL
)
GO
CREATE CLUSTERED INDEX IX_CorporationTitle_CallID ON dbo.[CorporationTitle] (CallID)
