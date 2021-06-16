using System;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Parser;

namespace MangaChecker.Core.Sources.Manganato
{
    public class ManganatoSource : IMangaSource
    {
        public string Name { get; } = "Manganato";
        public string Host { get; } = "https://readmanganato.com";
        public SourceType SourceType { get; } = SourceType.Manganato;

        public string GetUrlWithMangaId(string id) => $"{Host}/{id}";

        public string GetMangaIdFromUrl(string url)
        {
            var cleanLink = url;
            var possibleLastSlashIndex = url.LastIndexOf("/", StringComparison.Ordinal);
            if (possibleLastSlashIndex == (url.Length - 1))
                cleanLink = url.Remove(possibleLastSlashIndex);
            
            var beforeIdSlashIndex = url.LastIndexOf("/", StringComparison.Ordinal);
            return cleanLink.Substring(beforeIdSlashIndex + 1);
        }

        public ISourceDataParser GetDefaultParser() => ManganatoSourceParser.Instance;
    }
}