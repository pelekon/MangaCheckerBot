using System;

namespace MangaChecker.Core.Defines.Common
{
    public interface IManga
    {
        public string SiteMangaId { get; }
        public string Name { get; }
        public string? Description { get; }
        public float CurrentChapter { get; }
        public float NewestChapter { get; }
        public int AmountOfChapters { get; }
        public SourceType Source { get; }
        public DateTime? LastChaptersUpdate { get; }
        public string? MangaImage { get; }

        public string GetMangeTitleInfo();

        public bool Equals(IManga other);
    }
}