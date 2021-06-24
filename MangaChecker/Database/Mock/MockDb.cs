using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MangaChecker.Core.Defines.Common;
using MangaChecker.Core.Defines.DataProviders.Storage;

namespace MangaChecker.Database.Mock
{
    [ExcludeFromCodeCoverage]
    public class MockDb : IMangaCheckerStorageDataProvider
    {
        private readonly Dictionary<string, IManga> _storage = new();

        public int AmountOfStoredMangas => _storage.Count;

        public IManga? GetManga(string name) => _storage.ContainsKey(name) ? _storage[name] : null;

        public IManga? GetManga(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool AddManga(IManga manga)
        {
            if (_storage.ContainsKey(manga.Name))
                return false;
            
            _storage.Add(manga.Name, manga);
            return true;
        }

        public Task RemoveMangaAsync(IManga manga)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateCurrentChapterAsync(IManga manga, float chapter)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateChaptersInfoAsync(IManga manga, float newestChapter, int amount)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateChaptersInfo(IEnumerable<(IManga manga, float newestChapter, int amount)> list)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IManga>> GetAllUnreadMangas()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IManga> GetAll() => _storage.Select(p => p.Value);
    }
}