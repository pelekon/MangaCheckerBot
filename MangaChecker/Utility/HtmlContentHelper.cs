using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MangaChecker.Utility
{
    public class HtmlContentHelper
    {
        public enum ContentSourceTag
        {
            Head,
            Body
        }
        
        public static string? GetTagContent(HtmlDocument document, ContentSourceTag sourceTag, string tag, 
            bool findFirst = true, string? className = null, string? childTag = null)
        {
            var sourceNode = GetSourceNode(document, sourceTag);
            if (sourceNode == null) return null;

            HtmlNode? targetNode = null;

            if (findFirst && className == null && childTag == null)
                targetNode = sourceNode.SelectSingleNode($"//{tag}");
            else
                targetNode = FindInCollection(sourceNode.SelectNodes($"//{tag}"), findFirst, className, childTag);

            return targetNode?.InnerText;
        }

        private static HtmlNode? FindInCollection(HtmlNodeCollection collection, bool first, string? className,
            string? childTag)
        {
            if (collection.Count == 0)
                return null;

            List<HtmlNode> possibleTargets = new(2);
            
            var index = first ? 0 : collection.Count - 1;

            while (index > -1 && index < collection.Count)
            {
                var obj = collection[index];
                if (className != null && !obj.HasClass(className)) continue;
                if (childTag != null)
                {
                    var nodes = obj.SelectNodes($"//{childTag}");
                    if (nodes != null)
                        possibleTargets.AddRange(nodes!);
                }
                else
                {
                    possibleTargets.Add(obj);
                    break;
                }

                index = first ? ++index : --index;
            }

            return possibleTargets.Count > 0 ? possibleTargets[0] : null;
        }

        public static string? GetTagPropertyContent(HtmlDocument document, ContentSourceTag sourceTag, string tag, 
            string attributeName, string attributeValue, bool findFirst = true, string contentPropertyName = "content")
        {
            var sourceNode = GetSourceNode(document, sourceTag);
            if (sourceNode == null) return null;

            var attrs = sourceNode.SelectNodes($"//{tag}")
                .Select(n => n.Attributes)
                .SelectMany(a =>
                {
                    List<HtmlAttribute> attributes = new();
                    for (int i = 0; i < a.Count; ++i)
                    {
                        if (a[i].Name == attributeName && a[i].Value == attributeValue)
                            attributes.Add(a[i]);
                    }

                    return attributes;
                });

            HtmlAttribute target = findFirst ? attrs.First() : attrs.Last();
            
            if (target == null)
                return null;

            var targetContent = target.OwnerNode.Attributes.AttributesWithName(contentPropertyName)
                .First();
            
            return targetContent?.Value;
        }

        public static List<string>? GetTagContentList(HtmlDocument document, ContentSourceTag sourceTag, string tag,
            string? tagClass = null, string? childTag = null)
        {
            var sourceNode = GetSourceNode(document, sourceTag);
            if (sourceNode == null) return null;

            var nodes = sourceNode.SelectNodes($"//{tag}").AsEnumerable();
            if (tagClass != null)
                nodes = nodes.Where(n => n.HasClass(tagClass));

            return nodes?.SelectMany(n => n.SelectNodes($"//{childTag}")).Select(n => n.InnerText).ToList();
        }

        public static string? GetTagContentWithId(HtmlDocument document, ContentSourceTag sourceTag, string tag,
            string id, string? subTags = null, bool mergeContentOfAllChildren = false)
        {
            var sourceNode = GetSourceNode(document, sourceTag);
            if (sourceNode == null) return null;

            var nodes = sourceNode.SelectNodes($"//{tag}");
            var target = nodes?.First(n => n.Id == id);
            if (subTags != null && target != null)
            {
                var children = target.SelectNodes($"//{subTags}");
                if ((children?.Count ?? 0) == 0) return null;
                
                var content = !mergeContentOfAllChildren ? 
                    children![0].InnerText :
                    (string.Join("\n", children!.Select(n => n.InnerText)));
                return content;
            }

            return target?.InnerText;
        }
        
        private static HtmlNode? GetSourceNode(HtmlDocument document, ContentSourceTag sourceTag)
        {
            var sourceNodeName = GetNodeName(sourceTag);
            return document.DocumentNode.SelectSingleNode(sourceNodeName);
        }

        private static string GetNodeName(ContentSourceTag sourceTag)
        {
            switch (sourceTag)
            {
                case ContentSourceTag.Head:
                    return "//head";
                case ContentSourceTag.Body:
                    return "//body";
            }

            return "";
        }
    }
}