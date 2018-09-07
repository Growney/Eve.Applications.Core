CREATE TABLE [dbo].[CharacterRoles]
(
	[CallID] UNIQUEIDENTIFIER NOT NULL,
	[Roles] BIGINT NOT NULL,
	[RolesAtBase] BIGINT NOT NULL,
	[RolesAtHq] BIGINT NOT NULL,
	[RolesAtOther] BIGINT NOT NULL
)
GO
CREATE CLUSTERED INDEX IX_CharacterRoles_CallID ON dbo.[CharacterRoles] (CallID)
