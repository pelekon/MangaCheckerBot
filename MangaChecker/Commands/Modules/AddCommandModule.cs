using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MangaChecker.Core.DataFetching;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.DataProviders.Storage;
using MangaChecker.Core.Entities;
using MangaChecker.Core.Sources;

namespace MangaChecker.Commands.Modules
{
    public class AddCommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMangaCheckerStorageDataProvider _dataStorage;

        public AddCommandModule(IMangaCheckerStorageDataProvider dataStorage)
        {
            _dataStorage = dataStorage;
        }
        
        [Command("add")]
        [Summary("Adds manga from given link to list of observed mangas." +
                 "Params: MangaUrl: String, Optional: CurrentChapter: Number")]
        public async Task AddMangaToList(string link, float currentChapter = 0)
        {
            var cleanLink = link.Replace("<", "").Replace(">", "");
            IMangaSource? source = MangaSourceFabric.Instance.ResolveSource(cleanLink);
            if (source == null)
                throw new Exception($"Failed to find proper manga source for link: {cleanLink}");
            
            MangaInfoFetcher fetcher = new();
            var info = await fetcher.FetchInfo(cleanLink, source, false);
            var lastCh = info.ChaptersList.Count > 0 ? info.ChaptersList[0].Item1 : 0;
            
            var addResult = _dataStorage.AddManga(new FetchedMangaData(source.GetMangaIdFromUrl(cleanLink), info.Name,
                info.Description, currentChapter, lastCh, info.ChaptersList.Count, source.SourceType, info.ImageUrl));

            if (!addResult)
                throw new Exception("Failed to add manga! Reason: Manga is already on observed list!");
            
            EmbedBuilder embedBuilder = new();
            Embed embed = embedBuilder
                .WithTitle("Successfully added manga!")
                .WithDescription(
                    $"You have successfully added manga **\"{info.Name}\"** to your watch list!\nNewest ch: **{lastCh}** Last read ch: **{currentChapter}** Amount of chapters: **{info.ChaptersList.Count}**")
                .WithColor(Color.Green)
                .WithImageUrl(info.ImageUrl)
                .Build();
            
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}