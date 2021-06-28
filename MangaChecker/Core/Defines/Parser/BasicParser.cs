using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
                {
                    if (!IsBonusChapterWithoutNumber(components))
                        throw new Exception(
                            $"Failed to get chapter number for manga {mangaTitle}! Chapter string: {ch}");
                    
                    chapter = GetChapterNrForBonusChapter(chapters);
                }
                        
                chapters.Add((chapter.Value, ch));
            }
                    
            return chapters;
        }
        
        private float? GetChapterFromComponents(IEnumerable<string> components)
        {
            float? ch = null;
        
            foreach (var c in components)
            {
                // remove any not necessary characters from chapter number component
                var clean = c.Replace(":", "").Replace("(", "")
                    .Replace(")", "");
                if (clean.ToLower().Contains("v2"))
                    clean = clean.ToLower().Replace("v2", "");
                if (!float.TryParse(clean, NumberStyles.Float, CultureInfo.GetCultureInfo("en-EN"), out var res)) continue;
                ch = res;
                break;
            }
        
            return ch;
        }

        private bool IsBonusChapterWithoutNumber(IEnumerable<string> components)
        {
            var enumerable = components as string[] ?? components.ToArray();
            return enumerable.Contains("Bonus") || enumerable.Contains("bonus");
        }

        private float GetChapterNrForBonusChapter(IList<(float, string)> chapters)
        {
            if (chapters.Count == 0)
                return 0.5f;
            
            var last = chapters[^1];
            return Math.Abs(last.Item1) - 1 + 0.55f;
        }
    }
}