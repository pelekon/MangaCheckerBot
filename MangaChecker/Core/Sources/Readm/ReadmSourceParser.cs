using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Parser;
using MangaChecker.Utility;

namespace MangaChecker.Core.Sources.Readm
{
    public class ReadmSourceParser : BasicParser, ISourceDataParser
    {
        private ReadmSourceParser() {}
        public static readonly ReadmSourceParser Instance = new();
        
        public async Task<SourceDataParserResult> ParseSourceContent(string content, bool isUpdate)
        {
            HtmlDocument document = new();
            document.LoadHtml(content);

            string? title = isUpdate ? "" : HtmlContentHelper.GetTagContent(document, HtmlContentHelper.ContentSourceTag.Body,
                "h1", true, "page-title");
            string? desc = isUpdate ? "" : HtmlContentHelper.GetTagContent(document, HtmlContentHelper.ContentSourceTag.Body,
                "p", true, null, "p/span");
            string imgUrl = isUpdate ? "" : ("https://readm.org" + HtmlContentHelper.GetTagPropertyContent(document, 
                HtmlContentHelper.ContentSourceTag.Body, "img", "class", 
                "series-profile-thumb", true, "src"));
            var chapters = HtmlContentHelper.GetTagContentList(document, HtmlContentHelper.ContentSourceTag.Body,
                "div", "season-list-column", "div/div/div/div/div/table/tbody/tr/td/h6");
            var chapterList = ParseChapterList(chapters, title ?? "Unk");
            
            return new SourceDataParserResult(title ?? "Unk", desc ?? "Unk", imgUrl, chapterList);
        }
    }
}