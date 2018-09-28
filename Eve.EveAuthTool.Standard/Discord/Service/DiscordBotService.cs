using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Service.Module;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public class DiscordBotService : IHostedService
    {
        private readonly IDiscordBotConfiguration m_config;
        private readonly IServiceProvider m_provider;

        private DiscordSocketClient m_client;
        private DILinkedCommandService m_service;
        
        public DiscordBotService(IDiscordBotConfiguration config, IServiceProvider provider)
        {
            m_provider = provider;
            m_config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(m_config.BotKey))
            {
                m_client = new DiscordSocketClient(new DiscordSocketConfig()
                {
                    LogLevel = LogSeverity.Critical
                });
                m_client.Log += (msg) =>
                {
                    Console.WriteLine(msg);
                    return Task.CompletedTask;
                };
                m_service = new DILinkedCommandService(m_provider);

                await AddServiceModules(m_service);

                m_client.MessageReceived += HandleCommand;
                m_client.UserJoined += HandleUserJoined;
                await m_client.LoginAsync(TokenType.Bot, m_config.BotKey);
                await m_client.StartAsync();
            }
        }

        private static async Task AddServiceModules(DILinkedCommandService service)
        {
            await service.AddModuleAsync(typeof(RegistrationCommands));
        }

        private Task LoggedIn()
        {
            return Task.CompletedTask;
        }

        private Task HandleUserJoined(SocketGuildUser arg)
        {
            return Task.CompletedTask; 
        }

        private async Task HandleCommand(SocketMessage arg)
        {
            if(arg is SocketUserMessage msg)
            {
                SocketCommandContext context = new SocketCommandContext(m_client, msg);
                ContextWithDI<SocketCommandContext> contextWithDI = new ContextWithDI<SocketCommandContext>(m_service.Provider, context);
                int argPos = 0;

                if(msg.HasMentionPrefix(m_client.CurrentUser,ref argPos) || 
                    (!string.IsNullOrWhiteSpace(m_config.CommandPrefix) && msg.HasStringPrefix(m_config.CommandPrefix,ref argPos)))

                {
                    string trimmed = msg.Content.Substring(argPos).Trim();
                    var result = await m_service.ExecuteAsync(contextWithDI, trimmed);
                    if(!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        Console.WriteLine(result.ErrorReason);
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await m_client?.LogoutAsync();
            m_client?.Dispose();
        }

        public IUser GetUser(ulong id)
        {
            return m_client.GetUser(id);
        }

        public SocketGuild GetGuild(ulong id)
        {
            return m_client.GetGuild(id);
        }

        public Task<IGuildUser> GetBotGuildUser(IGuild guild)
        {
            return guild.GetUserAsync(m_client.CurrentUser.Id);
        }
    }
}
