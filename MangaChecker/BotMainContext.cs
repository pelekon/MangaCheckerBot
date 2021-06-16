using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MangaChecker.Commands;
using MangaChecker.Core.DataFetching;
using MangaChecker.Core.Defines.DataProviders.Credentials;
using MangaChecker.Core.Defines.DataProviders.Storage;
using MangaChecker.Core.Messages;
using MangaChecker.Credentials;
using MangaChecker.Database;
using MangaChecker.Database.MySQL;
using Microsoft.Extensions.DependencyInjection;

namespace MangaChecker
{
    public class BotMainContext
    {
        private ICredentialDataProvider _credentialDataProvider;
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private CommandHandler _commandHandler;
        private IServiceProvider _serviceProvider;
        private MangaInfoUpdater _infoUpdater;
        private CommandReactionHandler _commandReactionHandler;
        private bool _didSetupUpdater = false;
        
        public async Task StartContextAsync()
        {
            await InitializeCredentials();
            // create necessary objects to handle bot events and connection
            CreateNecessaryServices();
            // setup logger handlers
            InitializeLogManager();
            // setup client event handlers
            _client.Ready += ClientOnReady;
            // connect to bot
            await _client.LoginAsync(TokenType.Bot, _credentialDataProvider.BotToken);
            await _client.StartAsync();
            // initialize command handling
            await _commandHandler.SetupCommandHandlingAsync();
            
            // block task until program is closed manually
            await Task.Delay(-1);
        }

        private async Task InitializeCredentials()
        {
            _credentialDataProvider = new CredentialsProvider();
            // Load credentials
            await _credentialDataProvider.Load();
        }

        private void CreateNecessaryServices()
        {
            
            _client = new DiscordSocketClient();
            _commandService = new CommandService();
            _commandReactionHandler = new CommandReactionHandler(_client);
            _serviceProvider = BuildServiceProvider();
            _commandHandler = _serviceProvider.GetService<CommandHandler>()!;
            _infoUpdater = _serviceProvider.GetService<MangaInfoUpdater>()!;
        }

        private void InitializeLogManager()
        {
            _client.Log += LogBotMessageAsync;
            _commandService.Log += LogBotMessageAsync;
        }

        private Task ClientOnReady()
        {
            if (_didSetupUpdater) return Task.CompletedTask;
            _didSetupUpdater = true;
            _client.ReactionAdded += OnReactionAdded;
            _client.ReactionRemoved += OnReactionRemoved;
            // start manga storage's content update
            _infoUpdater.StartUpdating();
            _commandReactionHandler.InitializeCleanupProcess();
            return Task.CompletedTask;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel channel, 
            SocketReaction reaction)
        {
            await _commandReactionHandler.OnReactionAddDetected(channel.Id, reaction.UserId, cachedMessage.Id, reaction.Emote);
        }
        
        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            await _commandReactionHandler.OnReactionRemoveDetected(channel.Id, reaction.UserId, cachedMessage.Id, reaction.Emote);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var dbProvider = new DatabaseDataProvider(_credentialDataProvider);
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton<CommandHandler>()
                .AddSingleton(_credentialDataProvider)
                .AddSingleton(_commandReactionHandler)
                .AddSingleton<IMangaCheckerStorageDataProvider>(dbProvider)
                .AddSingleton<IMangaCheckerSettingsProvider>(dbProvider)
                .AddSingleton<MessageBroadcaster>()
                .AddSingleton<MangaInfoUpdater>()
                .BuildServiceProvider();
        }

        private Task LogBotMessageAsync(LogMessage msg)
        {
            Console.WriteLine($"[{DateTime.Now}] {msg.Message}");
            return Task.CompletedTask;
        }
    }
}