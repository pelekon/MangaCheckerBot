using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Parser;
using MangaChecker.Utility;

namespace MangaChecker.Core.Sources.Manganato
{
    public class ManganatoSourceParser : BasicParser, ISourceDataParser
    {
        private ManganatoSourceParser() {}
        public static readonly ManganatoSourceParser Instance = new();
        
        public async Task<SourceDataParserResult> ParseSourceContent(string content, bool isUpdate)
        {
            HtmlDocument document = new();
            document.LoadHtml(content);
            
            string? title = isUpdate ? "" : HtmlContentHelper.GetTagContent(document, HtmlContentHelper.ContentSourceTag.Body, "h1");
            string? desc = isUpdate ? "" : HtmlContentHelper.GetTagPropertyContent(document, HtmlContentHelper.ContentSourceTag.Head,
                "meta", "property", "og:description");
            string? imgUrl = isUpdate ? "" : HtmlContentHelper.GetTagPropertyContent(document, HtmlContentHelper.ContentSourceTag.Head,
                "meta", "property", "og:image");
            List<string>? notParsedChapters = HtmlContentHelper.GetTagContentList(document, HtmlContentHelper.ContentSourceTag.Body,
                "ul", "row-content-chapter", "li/a");
            var chapters = ParseChapterList(notParsedChapters, title ?? "Unknown");

            return new SourceDataParserResult(title ?? "Unk", desc ?? "Unk", imgUrl ?? "Unk", chapters);
        }
    }
}