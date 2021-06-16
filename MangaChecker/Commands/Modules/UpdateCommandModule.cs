using System;
using System.Threading.Tasks;
using Discord.Commands;
using MangaChecker.Core.Defines.DataProviders.Storage;

namespace MangaChecker.Commands.Modules
{
    public class UpdateCommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMangaCheckerStorageDataProvider _storageDataProvider;

        public UpdateCommandModule(IMangaCheckerStorageDataProvider storageDataProvider)
        {
            _storageDataProvider = storageDataProvider;
        }

        // [Command("update manga")]
        // [Summary("Forces update of chapters list for manga with given name.\n" +
        //          "Params: Name: String")]
        // public async Task UpdateManga(string name)
        // {
        //     var manga = _storageDataProvider.GetManga(name);
        //     if (manga == null)
        //         throw new Exception("Failed to find manga with name: **{name}**!");
        //
        //     await PerformUpdate(manga);
        // }
        //
        // [Command("update manga")]
        // [Summary("Forces update of chapters list for manga with given storageID.\n" +
        //          "Params: StorageID: Number")]
        // public async Task UpdateManga(int storageId)
        // {
        //     var manga = _storageDataProvider.GetManga(storageId);
        //     if (manga == null)
        //         throw new Exception($"Failed to find manga with storageID: **{storageId}**!");
        //     
        //     await PerformUpdate(manga);
        // }
        //
        // private async Task PerformUpdate(IManga manga)
        // {
        //     
        // }
        
        [Command("update chapter")]
        [Summary("Forces update of current chapter for manga with given name.\n" +
                 "Params: Name: String, Chapter: Number")]
        public async Task UpdateMangaChapter(string name, float chapter)
        {
            var manga = _storageDataProvider.GetManga(name);
            if (manga == null)
                throw new Exception($"Failed to find manga with name: **{name}**!");
            
            await _storageDataProvider.UpdateCurrentChapterAsync(manga, chapter);
            await Context.Channel.SendMessageAsync(":white_check_mark: Successfully updated chapter.");
        }
        
        [Command("update chapter")]
        [Summary("Forces update of current chapter for manga with given storageID.\n" +
                 "Params: StorageID: Number, Chapter: Number")]
        public async Task UpdateMangaChapter(int storageId, float chapter)
        {
            var manga = _storageDataProvider.GetManga(storageId);
            if (manga == null)
                throw new Exception($"Failed to find manga with storageID: **{storageId}**!");
            
            await _storageDataProvider.UpdateCurrentChapterAsync(manga, chapter);
            await Context.Channel.SendMessageAsync(":white_check_mark: Successfully updated chapter.");
        }
    }
}