using MangaChecker.Core.Defines;
using MangaChecker.Core.Sources.Mangakakalot;
using MangaChecker.Core.Sources.Manganato;
using MangaChecker.Core.Sources.Readm;

namespace MangaChecker.Core.Sources
{
    public class MangaSourceFabric
    {
        public static readonly MangaSourceFabric Instance = new MangaSourceFabric();

        public IMangaSource? ResolveSource(string url)
        {
            if (url.Contains("mangakakalot.com"))
                return new MangakakalotSource();
            if (url.Contains("readm.org"))
                return new ReadmSource();
            if (url.Contains("manganato.com"))
                return new ManganatoSource();
            return null;
        }

        public IMangaSource? GetSource(SourceType type)
        {
            switch (type)
            {
                case SourceType.Mangakakalot:
                    return new MangakakalotSource();
                case SourceType.Readm:
                    return new ReadmSource();
                case SourceType.Manganato:
                    return new ManganatoSource();
            }

            return null;
        }
    }
}