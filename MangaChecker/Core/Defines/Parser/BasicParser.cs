using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
                var components = ch.Split(new char[] { ' ', '\u00A0'});
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
                var clean = ClearString(c);
                Regex regex = new Regex("([0-9][vV][0-9]{0,2})|([0-9]{1,4}\\.[0-9][vV][0-9]{0,2})\\w+",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(clean);
                if (matches.Count > 0)
                    clean = CleanChapterStringFromVersion(clean) ?? clean;
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

        private string? CleanChapterStringFromVersion(string str)
        {
            var index = str.ToLower().IndexOf("v", StringComparison.Ordinal);
            if (index > -1)
                return str.Substring(0, index);
            return null;
        }

        protected string ClearString(string source)
        {
            return source.Replace(":", "").Replace("(", "")
                .Replace(")", "").Replace("\u00A0", "");
        }
    }
}