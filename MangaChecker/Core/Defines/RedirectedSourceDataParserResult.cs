using System.Collections.Generic;

namespace MangaChecker.Core.Defines
{
    public class RedirectedSourceDataParserResult : SourceDataParserResult
    {
        public string RedirectUrl { get; }
        public IMangaSource? NewSource { get; }

        public RedirectedSourceDataParserResult(string redirectUrl) : base("", "", "", 
            new List<(float, string)>())
        {
            RedirectUrl = redirectUrl;
        }

        public RedirectedSourceDataParserResult(SourceDataParserResult newResult, string redirectUrl, IMangaSource source) :
            base(newResult.Name, newResult.Description,  newResult.ImageUrl, newResult.ChaptersList)
        {
            RedirectUrl = redirectUrl;
            NewSource = source;
        }
    }
}