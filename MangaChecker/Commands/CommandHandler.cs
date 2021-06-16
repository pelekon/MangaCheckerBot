using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MangaChecker.Commands
{
    public class CommandHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public CommandHandler(IServiceProvider serviceProvider, DiscordSocketClient client, CommandService commandService)
        {
            _serviceProvider = serviceProvider;
            _client = client;
            _commandService = commandService;
        }

        public async Task SetupCommandHandlingAsync()
        {
            _client.MessageReceived += HandleReceivedCommand;
            _commandService.CommandExecuted += HandleOnCommandExecuted;
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        }

        private async Task HandleOnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (!result.IsSuccess)
            {
                string commandName = commandInfo.IsSpecified ? commandInfo.Value.Name : "Unknown";
                EmbedBuilder embedBuilder = new();
                embedBuilder.WithColor(Color.Red)
                    .WithTitle($"Failed to execute command {commandName}")
                    .WithDescription(result.ErrorReason);
                await commandContext.Channel.SendMessageAsync(null, false, embedBuilder.Build());
            }
        }

        private async Task HandleReceivedCommand(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null)
                return;

            int argPos = 0;
            if (!message.HasCharPrefix('.', ref argPos) || message.Author.IsBot)
                return;

            var commandContext = new SocketCommandContext(_client, message);
            await _commandService.ExecuteAsync(commandContext, argPos, _serviceProvider);
        }
    }
}