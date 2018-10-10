using Discord;
using Eve.ESI.Standard.Account;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Eve.EveAuthTool.Standard.Helpers;
using Gware.Standard.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public class DiscordLinkProvider : IDiscordLinkProvider
    {
        private readonly ILogger<DiscordLinkProvider> m_logger;
        private readonly ISingleParameters m_singles;
        private readonly IScopeParameters m_scoped;
        private readonly IDiscordBot m_bot;
        public DiscordLinkProvider(ILogger<DiscordLinkProvider> logger, ISingleParameters singles, IScopeParameters scoped,IDiscordBot discordBot)
        {
            m_logger = logger;
            m_singles = singles;
            m_scoped = scoped;
            m_bot = discordBot;
        }

        public async Task<IGuildUser> GetAccountGuildUser(IGuild guild, LinkedUserAccount account,eCacheOptions cacheOptions = eCacheOptions.Default)
        {
            if (guild == null)
            {
                throw new ArgumentException("Guild");
            }
            if (account == null)
            {
                throw new ArgumentNullException("Account");
            }

            if (ulong.TryParse(account.Link, out ulong userID))
            {
                IGuildUser retVal = await m_bot.GetGuildUser(guild.Id,userID, cacheOptions);
                if (retVal != null)
                {
                    m_logger.LogTrace($"Account {account.AccountGuid} is returned guild user {retVal.Username} ({userID}) on guild {guild.Name}");
                }
                else
                {
                    m_logger.LogInformation($"Account {account.AccountGuid} with discord user id {userID} is not a member of guild {guild.Name}");
                }
                return retVal;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(account.Link))
                {
                    throw new ArgumentException("Linked account cannot have a empty link");
                }
                else
                {
                    throw new ArgumentException("Account link must be a valid ulong");
                }

            }
        }
        public bool CanActionUpdateOnUser(IGuild guild, IGuildUser botUser, IGuildUser updateOn)
        {
            if (botUser == null)
            {
                throw new ArgumentNullException("BotUser");
            }
            if (updateOn == null)
            {
                throw new ArgumentNullException("UpdateOn");
            }

            if (guild.OwnerId != updateOn.Id)
            {
                try
                {
                    IReadOnlyCollection<IRole> roles = guild.Roles;
                    int[] highest = GetHighestRole(roles, botUser.RoleIds.Index(), updateOn.RoleIds.Index());
                    if (highest.Length == 2)
                    {
                        if (highest[0] >= highest[1])
                        {
                            return true;
                        }
                        else
                        {
                            m_logger.LogInformation($"Bot user {botUser.Nickname} ({botUser.Id}) cannot update user {updateOn.Nickname} ({updateOn.Id}) on guild {guild.Name} ({guild.Id}) as bot highest role is {highest[0]} and user highest is {highest[1]}");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid number of highest roles");
                    }
                }
                catch
                {
                    m_logger.LogWarning($"Error getting guild {guild.Id} roles user {updateOn.Nickname} cannot perform update action");
                }
            }
            else
            {
                m_logger.LogInformation($"Bot user {botUser.Nickname} ({botUser.Id}) cannot update user {updateOn.Nickname} ({updateOn.Id}) on guild {guild.Name} ({guild.Id}) as the user is the owner of the guild");
            }
            return false;

        }
        public async Task<(bool updated, string to)> UpdateNickname(IGuildUser updateOn, long characterID)
        {
            (bool updated, string to) retVal = (false, string.Empty);
            if (updateOn == null)
            {
                throw new ArgumentNullException("updateOn");
            }

            string setTo = await m_singles.PublicDataProvider.GetTaggedCharacterName(characterID);
            if (!string.IsNullOrWhiteSpace(setTo))
            {
                if (setTo != updateOn.Nickname)
                {
                    try
                    {
                        await updateOn.ModifyAsync(x =>
                        {
                            x.Nickname = setTo.ToString();
                        }, new RequestOptions()
                        {
                            AuditLogReason = $"ESA updating nickname to match characterID {characterID}",
                            RetryMode = RetryMode.AlwaysRetry
                        });
                        retVal.updated = true;
                        retVal.to = setTo;
                        m_logger.LogInformation($"User {updateOn.Nickname} ({updateOn.Id}) has had nickname updated to {setTo} on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
                    }
                    catch (Exception ex)
                    {
                        m_logger.LogError(ex, $"Failed to set user {updateOn.Nickname} ({updateOn.Id}) nickname to {setTo} on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
                    }
                }
                else
                {
                    m_logger.LogTrace($"User {updateOn.Nickname} ({updateOn.Id}) already has nickname {setTo} on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
                }
            }
            else
            {
                m_logger.LogWarning($"Cannot set tagged character name to null or empty string for user {updateOn.Nickname} ({updateOn.Id}) character id {characterID} on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
            }
            return retVal;
        }
        public Task<(int addedCount, int removedCount, int shouldAdd, int shouldRemove)> RemoveRoles(IEnumerable<IRole> guildRoles, IGuildUser botUser, IGuildUser updateOn)
        {
            return UpdateRoles(guildRoles, botUser, updateOn, null);
        }
        public async Task<(int addedCount, int removedCount, int shouldAdd, int shouldRemove)> UpdateRoles(IEnumerable<IRole> guildRoles, IGuildUser botUser, IGuildUser updateOn, DiscordRoleConfiguration configuration)
        {
            (int addedCount, int removedCount, int shouldAdd, int shouldRemove) retVal = (0, 0, 0, 0);
            if (updateOn == null)
            {
                throw new ArgumentNullException("updateOn");
            }
            if (updateOn.Guild == null)
            {
                throw new ArgumentException("Guild user guild cannot be null");
            }
            if (updateOn.Guild.EveryoneRole == null)
            {
                throw new ArgumentException("Guild everyone role cannot be null");
            }
            ulong everyoneRoleID = updateOn.Guild.EveryoneRole.Id;
            HashSet<ulong> updateTo = configuration?.AssignedRoles;
            if (updateTo == null)
            {
                updateTo = new HashSet<ulong> { everyoneRoleID };
            }

            IEnumerable<ulong> unindexedCurrentRoles = updateOn.RoleIds;
            if (unindexedCurrentRoles == null)
            {
                unindexedCurrentRoles = new ulong[] { everyoneRoleID };
            }

            List<ulong> toRemove = new List<ulong>();
            List<ulong> toAdd = new List<ulong>();
            HashSet<ulong> currentRoles = unindexedCurrentRoles.Index();
            foreach (ulong currentRole in currentRoles)
            {
                if (currentRole != everyoneRoleID)
                {
                    if (!updateTo.Contains(currentRole))
                    {
                        toRemove.Add(currentRole);
                    }
                }
            }

            foreach (ulong configRole in updateTo)
            {
                if (!currentRoles.Contains(configRole))
                {
                    toAdd.Add(configRole);
                }
            }
            List<IRole> removeRoleList = null;
            List<IRole> addRoleList = null;
            int botHighest = int.MinValue;

            if (toAdd.Count > 0 || toRemove.Count > 0)
            {
                botHighest = GetHighestRole(guildRoles, botUser.RoleIds.Index())[0];
            }

            if (toAdd.Count > 0)
            {
                retVal.shouldAdd = toAdd.Count;
                addRoleList = CreateRoleList(botHighest, toAdd, guildRoles);
                if (addRoleList.Count > 0)
                {
                    retVal.addedCount = addRoleList.Count;
                    try
                    {
                        await updateOn.AddRolesAsync(addRoleList);
                    }
                    catch (Exception ex)
                    {
                        m_logger.LogError(ex, $"Error adding roles to guild user  {updateOn.Nickname} ({updateOn.Id}) on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
                        retVal.addedCount = 0;
                    }
                }

            }
            if (toRemove.Count > 0)
            {
                retVal.shouldRemove = toRemove.Count;
                removeRoleList = CreateRoleList(botHighest, toRemove, guildRoles);
                if (removeRoleList.Count > 0)
                {
                    retVal.removedCount = removeRoleList.Count;
                    try
                    {
                        await updateOn.RemoveRolesAsync(removeRoleList);
                    }
                    catch (Exception ex)
                    {
                        m_logger.LogError(ex, $"Error removing roles from guild user  {updateOn.Nickname} ({updateOn.Id}) on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
                        retVal.removedCount = 0;
                    }
                }
            }

            m_logger.LogInformation($"Guild user {updateOn.Nickname} ({updateOn.Id}) had {addRoleList?.Count ?? 0} roles added and {removeRoleList?.Count ?? 0} removed to assign them to configuration {configuration?.Name ?? "NULL"} ({configuration?.Id.ToString() ?? "NULL"}) on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
            return retVal;
        }
        public async Task JoinUserToGuild(IDiscordClient client, string inviteID)
        {
            IInvite invite = await client.GetInviteAsync(inviteID);
            if (invite != null)
            {
                await invite.AcceptAsync();
            }
        }

        private static int[] GetHighestRole(IEnumerable<IRole> roles, params HashSet<ulong>[] usersRoleIds)
        {
            if (roles == null)
            {
                throw new ArgumentNullException("Roles");
            }
            if (usersRoleIds.Length == 0)
            {
                throw new ArgumentException("userRoleIds must contain at least one collection of roles");
            }

            int[] max = Enumerable.Repeat(int.MinValue, usersRoleIds.Length).ToArray();
            foreach (var role in roles)
            {
                for (int i = 0; i < max.Length; i++)
                {
                    if (usersRoleIds[i].Contains(role.Id))
                    {
                        max[i] = Math.Max(role.Position, max[i]);
                    }
                }
            }
            return max;
        }
        private List<IRole> CreateRoleList(int highestAllowed, IEnumerable<ulong> selected, IEnumerable<IRole> allRoles)
        {
            HashSet<ulong> set = new HashSet<ulong>(selected);
            List<IRole> roles = new List<IRole>();
            foreach (IRole role in allRoles)
            {
                if (set.Contains(role.Id))
                {
                    if (role.Position <= highestAllowed)
                    {
                        roles.Add(role);
                    }
                    else
                    {
                        m_logger.LogWarning($"Cannot add role {role.Name}[{role.Position}]({role.Id}) to list on guild {role.Guild.Name} ({role.Guild.Id}) as bots highest role is {highestAllowed}");
                    }
                }
            }
            return roles;
        }
    }
}
