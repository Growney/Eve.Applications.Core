CREATE PROCEDURE [dbo].[pDiscordRoleConfiguration]
	@Result NCHAR(50),
	@Id BIGINT = NULL,
	@Name VARCHAR(100) = NULL,
	@GuildInviteID VARCHAR(100) = NULL,
	@RoleIDs DataList READONLY
AS
BEGIN

	IF @Result = 'Save'
	BEGIN
		DECLARE @UsedID BIGINT
		IF NOT EXISTS (SELECT* FROM DiscordRoleConfiguration WHERE Id = @Id)
		BEGIN

			INSERT INTO DiscordRoleConfiguration([Name],[GuildInviteID])
			VALUES(@Name,@GuildInviteID)

			SET @UsedID = @@IDENTITY		

		END
		ELSE 
		BEGIN

			UPDATE DiscordRoleConfiguration
			SET Name = @Name,
			GuildInviteID = @GuildInviteID
			WHERE Id = @Id

			SET @UsedID = @ID
			
		END
		
		DELETE FROM DiscordRoleConfigurationRole WHERE DiscordRoleConfigurationId = @UsedID

		INSERT INTO DiscordRoleConfigurationRole (DiscordRoleConfigurationId,DiscordRoleId)
		SELECT @UsedID,DataValue
		FROM @RoleIDs

		SELECT @UsedID AS [Value]

	END

	IF @Result = 'All'
	BEGIN

		SELECT*
		FROM DiscordRoleConfiguration

	END

	IF @Result = 'RolesForConfiguration'
	BEGIN

		SELECT DiscordRoleId AS [Value]
		FROM DiscordRoleConfigurationRole
		WHERE DiscordRoleConfigurationId = @Id

	END

	IF @Result = 'Single'
	BEGIN

		SELECT*
		FROM DiscordRoleConfiguration
		WHERE Id = @Id

	END


END