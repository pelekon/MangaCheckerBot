using System;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Parser;

namespace MangaChecker.Core.Sources.Mangakakalot
{
    public class MangakakalotSource : IMangaSource
    {
        public string Name { get; } = "Mangakalot";
        public string Host { get; } = "https://mangakakalot.com";
        public SourceType SourceType { get; } = SourceType.Mangakakalot;

        public string GetUrlWithMangaId(string id) => id.StartsWith("read") ? $"{Host}/{id}" : $"{Host}/manga/{id}";

        public string GetMangaIdFromUrl(string url)
        {
            var cleanLink = url;
            var possibleLastSlashIndex = url.LastIndexOf("/", StringComparison.Ordinal);
            if (possibleLastSlashIndex == (url.Length - 1))
                cleanLink = url.Remove(possibleLastSlashIndex);
            
            var beforeIdSlashIndex = url.LastIndexOf("/", StringComparison.Ordinal);
            return cleanLink.Substring(beforeIdSlashIndex + 1);
        }
        
        public ISourceDataParser GetDefaultParser() => MangakakalotSourceParser.Instance;
    }
}