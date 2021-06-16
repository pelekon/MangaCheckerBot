using System;
using Discord;

namespace MangaChecker.Core.Messages.Types
{
    public interface IContentModifyingMessage
    {
        public Action<MessageProperties> GetMessageModifier();
    }
}