using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Service.Module;
using Gware.Standard.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Eve.EveAuthTool.Standard.Discord.Service.Providers;
using Gware.Standard.Web.Tenancy.Routing;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.Extensions.Logging;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public class DiscordBotService : IHostedService
    {
        private readonly IDiscordBotConfiguration m_config;
        private readonly IServiceScopeFactory m_scopeFactory;
        private readonly ITenantConfiguration m_tenantConfiguration;
        private readonly ILogger<DiscordBotService> m_logger;

        private DiscordSocketClient m_client;
        private CommandService m_service;

        public DiscordBotService(ILogger<DiscordBotService> logger,IDiscordBotConfiguration config, IServiceScopeFactory scopeFactory, ITenantConfiguration tenantConfiguration)
        {
            m_scopeFactory = scopeFactory;
            m_config = config;
            m_tenantConfiguration = tenantConfiguration;
            m_logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            m_logger.LogInformation("Discord bot service is starting");
            if (!string.IsNullOrWhiteSpace(m_config.BotKey))
            {
                m_client = new DiscordSocketClient(new DiscordSocketConfig()
                {
                    LogLevel = LogSeverity.Info
                });
                m_client.Log += x =>
                {
                    if (x.Severity == LogSeverity.Error)
                    {
                        m_logger.LogError(x.Exception, "Discord Socket client exception");
                    }
                    else
                    {
                        m_logger.LogInformation(x.Message);
                    }

                    return Task.CompletedTask;
                };
                m_service = new CommandService(); //new DILinkedCommandService(m_provider);

                await AddServiceModules(m_service);

                m_client.MessageReceived += HandleCommand;
                m_client.UserJoined += HandleUserJoined;
                await m_client.LoginAsync(TokenType.Bot, m_config.BotKey);
                await m_client.StartAsync();

            }

        }

        private static async Task AddServiceModules(CommandService service)
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
            if (arg is SocketUserMessage msg)
            {
                int argPos = 0;

                if (msg.HasMentionPrefix(m_client.CurrentUser, ref argPos) ||
                    (!string.IsNullOrWhiteSpace(m_config.CommandPrefix) && msg.HasStringPrefix(m_config.CommandPrefix, ref argPos)))
                {
                    using (IServiceScope scope = m_scopeFactory.CreateScope())
                    {
                        SocketCommandContext context = new SocketCommandContext(m_client, msg);
                        ConfigureServiceScope(m_tenantConfiguration,scope, context);
                        ContextWithDI<SocketCommandContext> contextWithDI = new ContextWithDI<SocketCommandContext>(scope.ServiceProvider, context);
                        string trimmed = msg.Content.Substring(argPos).Trim();
                        try
                        {
                            var result = await m_service.ExecuteAsync(contextWithDI, trimmed);
                            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                            {
                                m_logger.LogWarning($"Error running discord command {result.Error}:{result.ErrorReason}");
                            }
                        }
                        catch(Exception ex)
                        {
                            m_logger.LogError(ex, "Error in discord command");
                        }
                        
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await m_client?.LogoutAsync();
            m_client?.Dispose();

            m_logger.LogInformation("Discord bot service is stopping");
        }

        public static void ConfigureServiceScope(ITenantConfiguration tenantConfiguration,IServiceScope scope,ICommandContext context)
        {
            IDiscordCommandContextAccessor contextAccessor = scope.ServiceProvider.GetService<IDiscordCommandContextAccessor>();
            contextAccessor.CommandContext = context;
            
            if(context != null)
            {
                ITenantStorage tenantStorage = scope.ServiceProvider.GetService<ITenantStorage>();
                tenantStorage.Tenant = tenantConfiguration.GetTenantFromLink(context.Guild.Id.ToString());
                
            }
        }
    }
}
