using System;
using System.Threading.Tasks;
using Discord.Commands;
using MangaChecker.Core.Defines.DataProviders.Storage;

namespace MangaChecker.Commands.Modules
{
    public class RemoveCommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMangaCheckerStorageDataProvider _storageDataProvider;

        public RemoveCommandModule(IMangaCheckerStorageDataProvider storageDataProvider)
        {
            _storageDataProvider = storageDataProvider;
        }
        
        [Command("remove")]
        [Summary("Removes manga with given storageId from observing list." +
                 "Params: storageId: Number")]
        public async Task RemoveMangaFromList(int storageId)
        {
            var manga = _storageDataProvider.GetManga(storageId);
            if (manga == null)
                throw new Exception($"Failed to delete manga with storageID: **{storageId}**! " +
                                    $"Reason: Failed to find manga in storage!");

            await _storageDataProvider.RemoveMangaAsync(manga);
            await Context.Channel.SendMessageAsync(
                $":white_check_mark: Successfully removed manga **{manga.Name}** from observed list.");
        }
    }
}