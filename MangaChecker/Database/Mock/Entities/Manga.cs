using System;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Common;

namespace MangaChecker.Database.Mock.Entities
{
    public class Manga : IManga
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

        public Manga(string siteMangaId, string name, string description, float currentChapter, float newestChapter, 
            int amountOfChapters, SourceType source, string? mangaImage)
        {
            SiteMangaId = siteMangaId;
            Name = name;
            Description = description;
            CurrentChapter = currentChapter;
            NewestChapter = newestChapter;
            AmountOfChapters = amountOfChapters;
            Source = source;
            LastChaptersUpdate = null;
            MangaImage = mangaImage;
        }
        
        public string GetMangeTitleInfo() => $"Title: {Name}";

        public bool Equals(IManga other)
        {
            throw new NotImplementedException();
        }
    }
}