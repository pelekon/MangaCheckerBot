using System.Threading.Tasks;

namespace MangaChecker.Core.Defines.DataProviders.Storage
{
    public interface IMangaCheckerSettingsProvider
    {
        Task<IBotSettings?> GetCurrentSettingsAsync(ulong serverId);

        Task AssignSettingsAsync(ulong serverId, ulong inputId, ulong outputId);
    }
}