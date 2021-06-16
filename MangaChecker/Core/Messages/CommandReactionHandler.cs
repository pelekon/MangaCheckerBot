using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using MangaChecker.Core.Messages.Types;

namespace MangaChecker.Core.Messages
{
    public class CommandReactionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly int updateInterval = 3;
        private readonly int _defaultLifeTime = 5;
        private readonly ConcurrentDictionary<ulong, IReactionObservingMessage> _observedMessages = new();
        private Timer? currentTimer;

        public CommandReactionHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public void InitializeCleanupProcess()
        {
            var timer = new Timer()
            {
                Enabled = true,
                Interval = updateInterval * 60 * 1000,
                AutoReset = true
            };
            timer.Elapsed += OnContainerCheck;
            currentTimer = timer;
            timer.Start();
        }

        private void OnContainerCheck(object sender, ElapsedEventArgs e)
        {
            var keys = _observedMessages.Keys;
            foreach (var key in keys)
            {
                IReactionObservingMessage? msg;
                _observedMessages.TryGetValue(key, out msg);
                if (msg == null)
                    continue;
            
                var lifeTimePoint = msg.CreationTime.Add(TimeSpan.FromMinutes(_defaultLifeTime));
                if (lifeTimePoint < DateTime.Now)
                    _observedMessages.TryRemove(new KeyValuePair<ulong, IReactionObservingMessage>(key, msg));
            }
        }

        public async Task OnReactionAddDetected(ulong channelId, ulong userId, ulong messageId, IEmote emote)
        {
            var channel = _client.GetChannel(channelId) as SocketTextChannel;
            if (channel == null)
                return;
            
            IReactionObservingMessage? message = null;
            if (!_observedMessages.TryGetValue(messageId, out message))
                return;
            
            message.OnReactionAdd(userId, emote);
            if (message is IContentModifyingMessage modifyingMessage)
                await channel.ModifyMessageAsync(messageId, modifyingMessage.GetMessageModifier());
        }
        
        public async Task OnReactionRemoveDetected(ulong channelId, ulong userId, ulong messageId, IEmote emote)
        {
            var channel = _client.GetChannel(channelId) as SocketTextChannel;
            if (channel == null)
                return;
            
            IReactionObservingMessage? message = null;
            if (!_observedMessages.TryGetValue(messageId, out message))
                return;
            
            message.OnReactionRemove(userId, emote);
            if (message is IContentModifyingMessage modifyingMessage)
                await channel.ModifyMessageAsync(messageId, modifyingMessage.GetMessageModifier());
        }

        public void AddMessage(IReactionObservingMessage message)
        {
            _ = _observedMessages.TryAdd(message.MessageId, message);
        }
        
    }
}