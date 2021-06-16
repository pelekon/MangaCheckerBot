using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MangaChecker.Core.Defines;

namespace MangaChecker.Core.DataFetching
{
    public class MangaInfoFetcher
    {
        private static readonly HttpClient _httpClient = new();
        
        public async Task<SourceDataParserResult> FetchInfo(IMangaSource source, string mangaId, bool isUpdateFetch)
        {
            var url = source.GetUrlWithMangaId(mangaId);
            return await FetchInfo(url, source, isUpdateFetch);
        }

        public async Task<SourceDataParserResult> FetchInfo(string url, IMangaSource source, bool isUpdateFetch)
        {
            try
            {
                var data = await _httpClient.GetAsync(url);
                var parser = source.GetDefaultParser();
                var stringContent = await data.Content.ReadAsStringAsync();
                return await parser.ParseSourceContent(stringContent, isUpdateFetch);
            }
            catch (Exception e)
            {
                return new SourceDataParserResult("", "", "", new List<(float, string)>(),
                    true, $"[MangaInfoFetcher] {e.Message}");
            }
        }
    }
}