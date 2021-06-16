using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using MangaChecker.Core.Defines.DataProviders.Storage;

namespace MangaChecker.Commands.Modules
{
    public class ConfigCommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMangaCheckerSettingsProvider _settingsProvider;

        public ConfigCommandModule(IMangaCheckerSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        [Command("config set")]
        [Summary("Configures input and output channels for current server." +
                 "Params: Input: ChannelMention, Output: ChannelMention")]
        public async Task SetConfiguration(SocketTextChannel inputChannel, SocketTextChannel outputChannel)
        {
            await _settingsProvider.AssignSettingsAsync(Context.Guild.Id, inputChannel.Id, outputChannel.Id);
        }
    }
}