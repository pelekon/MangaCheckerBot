using System;
using Discord;

namespace MangaChecker.Core.Messages.Types
{
    public interface IReactionObservingMessage
    {
        public ulong MessageId { get; }
        public ulong UserId { get; }
        public DateTime CreationTime { get; }
        
        public void OnReactionAdd(ulong userId, IEmote emote);
        public void OnReactionRemove(ulong userId, IEmote emote);
    }
}