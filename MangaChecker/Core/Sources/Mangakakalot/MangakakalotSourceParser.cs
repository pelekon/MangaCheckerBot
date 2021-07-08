using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LinqToDB.Tools;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Parser;
using MangaChecker.Utility;

namespace MangaChecker.Core.Sources.Mangakakalot
{
    public class MangakakalotSourceParser : BasicParser, ISourceDataParser
    {
        private MangakakalotSourceParser() { }
        public static readonly MangakakalotSourceParser Instance = new();

        public async Task<SourceDataParserResult> ParseSourceContent(string content, bool isUpdate)
        {
            HtmlDocument document = new();
            document.LoadHtml(content);

            if (IsRedirected(document))
            {
                var cleanLink = GetRedirectUrl(document);
                if (cleanLink != null)
                    return new RedirectedSourceDataParserResult(cleanLink);
            }
            
            string? title = isUpdate ? "" : HtmlContentHelper.GetTagContent(document, HtmlContentHelper.ContentSourceTag.Body, "h1");
            string? desc = isUpdate ? "" : HtmlContentHelper.GetTagPropertyContent(document, HtmlContentHelper.ContentSourceTag.Head,
                "meta", "property", "og:description");
            string? imgUrl = isUpdate ? "" : HtmlContentHelper.GetTagPropertyContent(document, HtmlContentHelper.ContentSourceTag.Head,
                "meta", "property", "og:image");
            List<string>? notParsedChapters = HtmlContentHelper.GetTagContentList(document, HtmlContentHelper.ContentSourceTag.Body,
                "div", "chapter-list", "div/span/a");
            var chapters = ParseChapterList(notParsedChapters, title ?? "Unknown");

            return new SourceDataParserResult(title ?? "Unk", desc ?? "Unk", imgUrl ?? "Unk", chapters);
        }

        private bool IsRedirected(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode("//body");
            return node.InnerText.StartsWith("REDIRECT") || node.InnerText.StartsWith("\nREDIRECT") ;
        }

        private string? GetRedirectUrl(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode("//body");
            int index = node.InnerText.IndexOf(":", StringComparison.Ordinal);
            
            if (index == -1)
                return null;

            var url = node.InnerText[(index + 1)..];
            if (url.EndsWith(" ") || url.EndsWith("\n"))
                url = url.Replace(" ", "").Replace("\n", "");
            return url;
        }
    }
}