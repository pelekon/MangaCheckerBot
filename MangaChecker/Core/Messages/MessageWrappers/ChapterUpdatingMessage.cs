using System;
using Discord;
using Discord.WebSocket;
using MangaChecker.Core.Defines.Common;
using MangaChecker.Core.Defines.DataProviders.Storage;
using MangaChecker.Core.Messages.Types;

namespace MangaChecker.Core.Messages.MessageWrappers
{
    public class ChapterUpdatingMessage : IReactionObservingMessage
    {
        public ulong MessageId { get; }
        public ulong UserId { get; }
        public DateTime CreationTime { get; }

        private readonly IManga _manga;
        private readonly float _chapter;
        private readonly DiscordSocketClient _client;
        private readonly IMangaCheckerStorageDataProvider _storageDataProvider;
        private readonly ulong _channelId;
        private bool didUpdate = false;

        public ChapterUpdatingMessage(IManga manga, float chapter, DiscordSocketClient client, 
            IMangaCheckerStorageDataProvider storageDataProvider, ulong messageId, 
            ulong userId, DateTime creationTime, ulong channelId)
        {
            _manga = manga;
            _chapter = chapter;
            _client = client;
            _storageDataProvider = storageDataProvider;
            MessageId = messageId;
            UserId = userId;
            CreationTime = creationTime;
            _channelId = channelId;
        }

        public void OnReactionAdd(ulong userId, IEmote emote)
        {
            if (userId == UserId)
                return;
            
            if (emote is not Emoji emoji)
                return;

            if (emoji.Name == MessageReactions.EmoteWhiteCheckMark && !didUpdate)
                SendUpdateAndMessageNotification();
        }

        public void OnReactionRemove(ulong userId, IEmote emote) { }

        private void SendUpdateAndMessageNotification()
        {
            didUpdate = true;
            _storageDataProvider.UpdateCurrentChapterAsync(_manga, _chapter);
            var channel = _client.GetChannel(_channelId);
            if (channel is not SocketTextChannel textChannel)
                return;

            textChannel.SendMessageAsync(":white_check_mark: Successfully updated chapter.");
        }
    }
}