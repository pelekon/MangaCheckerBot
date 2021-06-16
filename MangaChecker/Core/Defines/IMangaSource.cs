using MangaChecker.Core.Defines.Parser;

namespace MangaChecker.Core.Defines
{
    public interface IMangaSource
    {
        string Name { get; }
        string Host { get; }
        SourceType SourceType { get; }

        string GetUrlWithMangaId(string id);
        string GetMangaIdFromUrl(string url);

        ISourceDataParser GetDefaultParser();
    }
}