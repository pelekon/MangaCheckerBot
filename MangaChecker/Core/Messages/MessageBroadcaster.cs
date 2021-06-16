using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MangaChecker.Core.Defines.DataProviders.Storage;

namespace MangaChecker.Core.Messages
{
    public class MessageBroadcaster
    {
        private readonly DiscordSocketClient _client;
        private readonly IMangaCheckerSettingsProvider _settingsProvider;

        public MessageBroadcaster(DiscordSocketClient client, IMangaCheckerSettingsProvider settingsProvider)
        {
            _client = client;
            _settingsProvider = settingsProvider;
        }

        public async Task SendMessageAsync(Embed embed, Func<RestUserMessage, Task>? onMessageSent = null)
        {
            var serverIds = _client.Guilds.Select(s => s.Id);
            List<Task> tasks = new(_client.Guilds.Count);

            foreach (var id in serverIds)
            {
                var serverId = id;
                tasks.Add(Task.Run(async () => await SendMessageToServerAsync(serverId, embed, onMessageSent)));
            }

            await Task.WhenAll(tasks);
        }

        private async Task SendMessageToServerAsync(ulong id, Embed embed, Func<RestUserMessage, Task>? onMessageSent)
        {
            var serverSettings = await _settingsProvider.GetCurrentSettingsAsync(id);
            if (serverSettings == null)
                return;

            if (_client.GetChannel(serverSettings.OutputChannelId) is SocketTextChannel textChannel)
            {
                var message = await textChannel.SendMessageAsync(null, false, embed);
                if (message != null && onMessageSent != null)
                    await onMessageSent.Invoke(message);
            }
        }
    }
}