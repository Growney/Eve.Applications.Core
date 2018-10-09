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
        public DiscordLinkProvider(ILogger<DiscordLinkProvider> logger,ISingleParameters singles,IScopeParameters scoped)
        {
            m_logger = logger;
            m_singles = singles;
            m_scoped = scoped;
        }

        public async Task<IGuildUser> GetAccountGuildUser(IGuild guild,LinkedUserAccount account)
        {
            if(guild == null)
            {
                throw new ArgumentException("Guild");
            }
            if(account == null)
            {
                throw new ArgumentNullException("Account");
            }

            if(ulong.TryParse(account.Link,out ulong userID))
            {
                IGuildUser retVal = await guild.GetUserAsync(userID);
                if(retVal != null)
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
        public bool CanActionUpdateOnUser(IGuild guild,IGuildUser botUser,IGuildUser updateOn)
        {
            if(botUser == null)
            {
                throw new ArgumentNullException("BotUser");
            }
            if(updateOn == null)
            {
                throw new ArgumentNullException("UpdateOn");
            }
            
            if (guild.OwnerId != updateOn.Id)
            {
                try
                {
                    IReadOnlyCollection<IRole> roles = guild.Roles;
                    int[] highest = GetHighestRole(roles, botUser.RoleIds.Index(), updateOn.RoleIds.Index());
                    if(highest.Length == 2)
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
        public async Task UpdateNickname(IGuildUser updateOn,long characterID)
        {
            if(updateOn == null)
            {
                throw new ArgumentNullException("updateOn");
            }

            string setTo = await m_singles.PublicDataProvider.GetTaggedCharacterName(characterID);
            if (!string.IsNullOrWhiteSpace(setTo))
            {
                if(setTo != updateOn.Nickname)
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
                    }
                    catch(Exception ex)
                    {
                        m_logger.LogError(ex,$"Failed to set user {updateOn.Nickname} ({updateOn.Id}) nickname to {setTo} on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
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

        }
        public Task RemoveRoles(IEnumerable<IRole> guildRoles,IGuildUser updateOn)
        {
            return UpdateRoles(guildRoles,updateOn, null);
        }
        public async Task UpdateRoles(IEnumerable<IRole> guildRoles,IGuildUser updateOn, DiscordRoleConfiguration configuration)
        {
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
            HashSet<ulong> updateTo = configuration.AssignedRoles;
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
            if (toAdd.Count > 0)
            {
                await updateOn.AddRolesAsync(CreateRoleList(toAdd, guildRoles));
            }
            if (toRemove.Count > 0)
            {
                await updateOn.RemoveRolesAsync(CreateRoleList(toRemove, guildRoles));
            }

            m_logger.LogInformation($"Guild user {updateOn.Nickname} ({updateOn.Id}) had {toAdd.Count} roles added and {toAdd.Count} removed to assign them to configuration {configuration?.Name ?? "NULL"} ({configuration?.Id.ToString() ?? "NULL"}) on guild {updateOn.Guild.Name} ({updateOn.Guild.Id})");
        }
        public async Task JoinUserToGuild(IDiscordClient client,string inviteID)
        {
            IInvite invite = await client.GetInviteAsync(inviteID);
            if (invite != null)
            {
                await invite.AcceptAsync();
            }
        }

        private static int[] GetHighestRole(IEnumerable<IRole> roles, params HashSet<ulong>[] usersRoleIds)
        {
            if(roles == null)
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
                    max[i] = Math.Max(role.Position, max[i]);
                }
            }
            return max;
        }
        private static IEnumerable<IRole> CreateRoleList(IEnumerable<ulong> selected, IEnumerable<IRole> allRoles)
        {
            HashSet<ulong> set = new HashSet<ulong>(selected);
            List<IRole> roles = new List<IRole>();
            foreach (IRole role in allRoles)
            {
                if (set.Contains(role.Id))
                {
                    roles.Add(role);
                }
            }
            return roles;
        }
    }
}
