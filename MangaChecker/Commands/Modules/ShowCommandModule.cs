using System;
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
    public class ShowCommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMangaCheckerStorageDataProvider _storageDataProvider;
        private readonly CommandReactionHandler _reactionHandler;

        public ShowCommandModule(IMangaCheckerStorageDataProvider storageDataProvider, CommandReactionHandler reactionHandler)
        {
            _storageDataProvider = storageDataProvider;
            _reactionHandler = reactionHandler;
        }

        [Command("show info")]
        [Summary("Searches for manga with given name and shows it's details." +
                 "Params: Name: String")]
        public async Task ShowMangaDetailsByName(string name)
        {
            var manga = _storageDataProvider.GetManga(name);
            if (manga == null)
                throw new Exception($"Failed to find manga with name: **{name}**!");
            
            await Context.Channel.SendMessageAsync(null, false, CreateInfoEmbed(manga));
        }

        [Command("show info")]
        [Summary("Searches for manga with given storageID and shows it's details." +
                 "Params: StorageID: Number")]
        public async Task ShowMangaDetails(int id)
        {
            var manga = _storageDataProvider.GetManga(id);
            if (manga == null)
                throw new Exception($"Failed to find manga with storageID: **{id}**!");
            
            await Context.Channel.SendMessageAsync(null, false, CreateInfoEmbed(manga));
        }

        [Command("show unread")]
        [Summary("Shows list of all unread mangas.")]
        public async Task ShowAllUnreadMangas()
        {
            var unread = await _storageDataProvider.GetAllUnreadMangas();
            var pages = PrepareShowAllUnreadEmbedPages(unread.OrderBy(m => m.Name));

            var message = await Context.Channel.SendMessageAsync(null, false, 
                pages.Count > 0 ? pages[0] : null);
            if (message == null)
                return;

            await message.AddReactionAsync(new Emoji(MessageReactions.EmoteArrowLeft));
            await message.AddReactionAsync(new Emoji(MessageReactions.EmoteArrowRight));
            _reactionHandler.AddMessage(new PagedContentMessage(message.Id, Context.User.Id, 1, pages));
        }
        
        private Embed CreateInfoEmbed(IManga manga)
        {
            IMangaSource? source = MangaSourceFabric.Instance.GetSource(manga.Source);
            EmbedBuilder embedBuilder = new();
            embedBuilder.WithTitle("Manga details:")
                .WithDescription($"{manga.GetMangeTitleInfo()}\n" +
                                 $"**Description**: {manga.Description ?? "Missing!"}\n" +
                                 $"**Link**: {source?.GetUrlWithMangaId(manga.SiteMangaId) ?? "Unknown"}\n" +
                                 $"Current ch: **{manga.CurrentChapter}** Newest ch: **{manga.NewestChapter}**\n" +
                                 $"Last update detected at: **{manga.LastChaptersUpdate?.ToString() ?? "Never"}**")
                .WithColor(Color.Gold);
            if (manga.MangaImage != null)
                embedBuilder.WithImageUrl(manga.MangaImage);

            return embedBuilder.Build();
        }
        
        private List<Embed> PrepareShowAllUnreadEmbedPages(IOrderedEnumerable<IManga> mangas)
        {
            List<EmbedBuilder> pagesBuilders = new();
            int fieldCounter = 0;
            EmbedBuilder currentEmbedBuilder = new();
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
            return pagesBuilders.Select(b => b.WithTitle("List of all unread mangas:")
                .WithColor(Color.LighterGrey)
                .WithFooter($"Page: {pageCounter++}/{pagesBuilders.Count}")
                .Build()).ToList();
        }
    }
}