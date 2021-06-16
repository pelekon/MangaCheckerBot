using System;
using LinqToDB.Mapping;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Common;

namespace MangaChecker.Database.MySQL.Entities
{
    [Table("observed_mangas_info")]
    public class MangaEntity : IManga
    {
        [Column("ID")]
        [PrimaryKey]
        public int MangaId { get; set; }

        [Column("siteID")]
        public string SiteMangaId { get; set; }

        [Column("name")]
        public string Name { get; set; }
        
        [Column("description")]
        public string? Description { get; set; }
        
        [Column("current_chapter")]
        public float CurrentChapter { get; set; }
        
        [Column("newest_chapter")]
        public float NewestChapter { get; set; }
        
        [Column("chapters_amount")]
        public int AmountOfChapters { get; set; }
        
        [Column("data_source_type")]
        public SourceType Source { get; set; }
        
        [Column("last_chapter_update")]
        public DateTime? LastChaptersUpdate { get; set; }
        
        [Column("manga_image_url")]
        public string? MangaImage { get; set; }

        public string GetMangeTitleInfo() => $"**StoregeID**: {MangaId}\n**Title**: {Name}";

        public bool Equals(IManga other)
        {
            if (other is MangaEntity otherEntity)
                return MangaId == otherEntity.MangaId;
            
            return false;
        }
    }
}