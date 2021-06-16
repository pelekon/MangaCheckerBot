using System.Collections.Generic;

namespace MangaChecker.Core.Defines
{
    public class SourceDataParserResult
    {
        public string Name { get; }
        public string Description { get; }
        public string ImageUrl { get; }
        public List<(float, string)> ChaptersList { get; }

        public bool IsFailed { get; }
        public string? FailReason { get; }

        public SourceDataParserResult(string name, string description, string imageUrl, List<(float, string)> chaptersList,
            bool isFailed = false, string? failReason = null)
        {
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            ChaptersList = chaptersList;
            IsFailed = isFailed;
            FailReason = failReason;
        }
    }
}