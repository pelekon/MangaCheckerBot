using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Common;
using MangaChecker.Core.Defines.DataProviders.Storage;
using MangaChecker.Core.Messages;
using MangaChecker.Core.Messages.MessageWrappers;
using MangaChecker.Core.Sources;

namespace MangaChecker.Commands.Modules
{
    public class MangaCommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMangaCheckerStorageDataProvider _dataStorage;
        private readonly CommandReactionHandler _reactionHandler;

        public MangaCommandModule(IMangaCheckerStorageDataProvider dataStorage, CommandReactionHandler reactionHandler)
        {
            _dataStorage = dataStorage;
            _reactionHandler = reactionHandler;
        }

        [Command("manga list")]
        [Summary("List all mangas.")]
        public async Task ShowAllMangas()
        {
            var mangas = _dataStorage.GetAll().OrderBy(m => m.Name);
            var pages = PrepareShowAllEmbedPages(mangas);

            var message = await Context.Channel.SendMessageAsync(null, false, 
                pages.Count > 0 ? pages[0] : null);
            if (message == null)
                return;

            await message.AddReactionAsync(new Emoji(MessageReactions.EmoteArrowLeft));
            await message.AddReactionAsync(new Emoji(MessageReactions.EmoteArrowRight));
            _reactionHandler.AddMessage(new PagedContentMessage(message.Id, Context.User.Id, 1, pages));
        }

        private List<Embed> PrepareShowAllEmbedPages(IOrderedEnumerable<IManga> mangas)
        {
            List<EmbedBuilder> pagesBuilders = new();
            int fieldCounter = 0;
            EmbedBuilder currentEmbedBuilder = new();
            //.WithTitle("List of all observed mangas:")
            foreach (var manga in mangas)
            {
                if (fieldCounter != 0 && fieldCounter % 6 == 0)
                {
                    fieldCounter = 0;
                    pagesBuilders.Add(currentEmbedBuilder);
                    currentEmbedBuilder = new();
                }
                
                IMangaSource? source = MangaSourceFabric.Instance.GetSource(manga.Source);
                EmbedFieldBuilder fieldBuilder = new();
                string desc = $"Link: {source?.GetUrlWithMangaId(manga.SiteMangaId) ?? "Unknown"}\n" +
                              $"Current ch: **{manga.CurrentChapter}** Newest ch: **{manga.NewestChapter}**\n" +
                              $"Last update detected at: **{manga.LastChaptersUpdate?.ToString() ?? "Never"}**";
                fieldBuilder.WithName(manga.GetMangeTitleInfo()).WithValue(desc).WithIsInline(false);
                currentEmbedBuilder.AddField(fieldBuilder);
                ++fieldCounter;
            }

            if (fieldCounter != 0)
                pagesBuilders.Add(currentEmbedBuilder);

            var pageCounter = 1;
            return pagesBuilders.Select(b => b.WithTitle("List of all observed mangas:")
                .WithColor(Color.LighterGrey)
                .WithFooter($"Page: {pageCounter++}/{pagesBuilders.Count}")
                .Build()).ToList();
        }
    }
}