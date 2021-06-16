using System.Collections.Generic;
using System.Threading.Tasks;
using MangaChecker.Core.Defines.Common;

namespace MangaChecker.Core.Defines.DataProviders.Storage
{
    public interface IMangaCheckerStorageDataProvider
    {
        int AmountOfStoredMangas { get; }
        
        IManga? GetManga(string name);
        IManga? GetManga(int id);

        bool AddManga(IManga manga);

        Task UpdateCurrentChapterAsync(IManga manga, float chapter);
        Task UpdateChaptersInfoAsync(IManga manga, float newestChapter, int amount);
        void UpdateChaptersInfo(IEnumerable<(IManga manga, float newestChapter, int amount)> list);

        Task<IEnumerable<IManga>> GetAllUnreadMangas();

        IEnumerable<IManga> GetAll();
    }
}