using System;
using System.Collections.Generic;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Common;

namespace MangaChecker.Core.Entities
{
    public class FetchedMangaData : IManga
    {
        public string SiteMangaId { get; }
        public string Name { get; }
        public string? Description { get; }
        public float CurrentChapter { get; }
        public float NewestChapter { get; }
        public int AmountOfChapters { get; }
        public SourceType Source { get; }
        public DateTime? LastChaptersUpdate { get; }
        public string? MangaImage { get; set; }

        public FetchedMangaData(string siteMangaId, string name, string? description, float currentChapter, float newestChapter, 
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

        public FetchedMangaData(IManga source, IReadOnlyList<(float, string)> chaptersList)
        {
            SiteMangaId = source.SiteMangaId;
            Name = source.Name;
            Description = source.Description;
            CurrentChapter = source.CurrentChapter;
            NewestChapter = chaptersList.Count > 0 ? chaptersList[0].Item1 : 0;
            AmountOfChapters = chaptersList.Count;
            Source = source.Source;
            LastChaptersUpdate = null;
            MangaImage = source.MangaImage;
        }

        public string GetMangeTitleInfo()
        {
            throw new NotImplementedException();
        }

        public bool Equals(IManga other) => SiteMangaId == other.SiteMangaId && Name == other.Name;
    }
}