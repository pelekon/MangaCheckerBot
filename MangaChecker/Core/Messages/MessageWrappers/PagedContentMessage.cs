using System;
using System.Collections.Generic;
using Discord;
using MangaChecker.Core.Messages.Types;

namespace MangaChecker.Core.Messages.MessageWrappers
{
    public class PagedContentMessage : IReactionObservingMessage, IContentModifyingMessage
    {
        public ulong MessageId { get; }
        public ulong UserId { get; }
        public DateTime CreationTime { get; }
        public int CurrentPage { get; private set; }
        public int AllPages { get; }
        private readonly IReadOnlyList<Embed> _pages;

        public PagedContentMessage(ulong messageId, ulong userId, int currentPage, IReadOnlyList<Embed> pages)
        {
            MessageId = messageId;
            UserId = userId;
            CreationTime = DateTime.Now;
            CurrentPage = currentPage;
            AllPages = pages.Count;
            _pages = pages;
        }

        public void OnReactionAdd(ulong userId, IEmote emote)
        {
            if (userId != UserId)
                return;
            
            if (emote is not Emoji emoji)
                return;

            if (emoji.Name == MessageReactions.EmoteArrowLeft)
                CurrentPage = CurrentPage == 1 ? CurrentPage : (CurrentPage - 1);
            
            if (emoji.Name == MessageReactions.EmoteArrowRight)
                CurrentPage = CurrentPage == AllPages ? CurrentPage : (CurrentPage + 1);
        }

        public void OnReactionRemove(ulong userId, IEmote emote)
        {
            
        }

        public Action<MessageProperties> GetMessageModifier()
        {
            return properties =>
            {
                properties.Embed = _pages.Count > 0 && CurrentPage <= _pages.Count ? _pages[CurrentPage - 1] : null;
            };
        }
    }
}