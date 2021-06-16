using System;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Parser;

namespace MangaChecker.Core.Sources.Readm
{
    public class ReadmSource : IMangaSource
    {
        public string Name { get; } = "Readm";
        public string Host { get; } = "https://readm.org";
        public SourceType SourceType { get; } = SourceType.Readm;

        public string GetUrlWithMangaId(string id) => $"{Host}/manga/{id}";

        public string GetMangaIdFromUrl(string url)
        {
            var cleanLink = url;
            var possibleLastSlashIndex = url.LastIndexOf("/", StringComparison.Ordinal);
            if (possibleLastSlashIndex == (url.Length - 1))
                cleanLink = url.Remove(possibleLastSlashIndex);
            
            var beforeIdSlashIndex = url.LastIndexOf("/", StringComparison.Ordinal);
            return cleanLink.Substring(beforeIdSlashIndex + 1);
        }

        public ISourceDataParser GetDefaultParser() => ReadmSourceParser.Instance;
    }
}