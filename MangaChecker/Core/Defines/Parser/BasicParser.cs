using System;
using System.Collections.Generic;
using System.Globalization;

namespace MangaChecker.Core.Defines.Parser
{
    public abstract class BasicParser
    {
        protected List<(float, string)> ParseChapterList(IReadOnlyCollection<string>? list, string mangaTitle)
        {
            if (list == null)
                return new List<(float, string)>();
                    
            List<(float, string)> chapters = new(list.Count);
        
            foreach (var ch in list)
            {
                var components = ch.Split(" ");
                var chapter = GetChapterFromComponents(components);
                        
                if (!chapter.HasValue)
                    throw new Exception($"Failed to get chapter number for manga {mangaTitle}!");
                        
                chapters.Add((chapter.Value, ch));
            }
                    
            return chapters;
        }
        
        protected float? GetChapterFromComponents(IEnumerable<string> components)
        {
            float? ch = null;
        
            foreach (var c in components)
            {
                // remove any not necessary characters from chapter number component
                var clean = c.Replace(":", "").Replace("(", "")
                    .Replace(")", "");
                if (!float.TryParse(clean, NumberStyles.Float, CultureInfo.GetCultureInfo("en-EN"), out var res)) continue;
                ch = res;
                break;
            }
        
            return ch;
        }
    }
}