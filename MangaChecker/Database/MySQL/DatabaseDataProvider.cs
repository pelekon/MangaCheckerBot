using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MangaChecker.Core.Defines.Common;
using MangaChecker.Core.Defines.DataProviders.Credentials;
using MangaChecker.Core.Defines.DataProviders.Storage;
using MangaChecker.Database.MySQL.Entities;

namespace MangaChecker.Database.MySQL
{
    public class DatabaseDataProvider : IMangaCheckerStorageDataProvider, IMangaCheckerSettingsProvider
    {
        public DatabaseDataProvider(ICredentialDataProvider credentialDataProvider)
        {
            DataConnection.DefaultSettings = new MySQLDatabaseSettings(credentialDataProvider);
        }

        public int AmountOfStoredMangas => 0;
        
        public IManga? GetManga(string name)
        {
            using var db = new MySQLDatabase();
            return db.GetObservedMangasInfoTable().First(m => m.Name == name);
        }

        public IManga? GetManga(int id)
        {
            using var db = new MySQLDatabase();
            return db.GetObservedMangasInfoTable().First(m => m.MangaId == id);
        }

        public bool AddManga(IManga manga)
        {
            using var db = new MySQLDatabase();
            var isPresent = false;

            try
            {
                var mangaEntity = db.GetObservedMangasInfoTable()
                    .First(e => e.Name == manga.Name);
                isPresent = true;
            }
            catch (InvalidOperationException e)
            {
                isPresent = false;
            }

            if (isPresent)
                return false;
            
            db.BeginTransaction(IsolationLevel.ReadCommitted);
            db.GetObservedMangasInfoTable().InsertWithInt32Identity(() => new MangaEntity()
            {
                SiteMangaId = manga.SiteMangaId,
                Name = manga.Name,
                Description = manga.Description,
                CurrentChapter = manga.CurrentChapter,
                NewestChapter = manga.NewestChapter,
                AmountOfChapters = manga.AmountOfChapters,
                Source = manga.Source,
                LastChaptersUpdate = manga.LastChaptersUpdate,
                MangaImage = manga.MangaImage
            });
            db.CommitTransaction();
            return true;
        }

        public async Task RemoveMangaAsync(IManga manga)
        {
            await using var db = new MySQLDatabase();
            var castedManga = manga as MangaEntity;
            if (castedManga == null)
                throw new Exception("[IMangaCheckerStorageDataProvider::RemoveManga] Failed to update manga! " +
                                    "Reason: Storage entity and param type mismatch!");
            
            await db.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await db.GetObservedMangasInfoTable().Where(m => m.MangaId == castedManga.MangaId).DeleteAsync();
            await db.CommitTransactionAsync();
        }

        public async Task UpdateCurrentChapterAsync(IManga manga, float chapter)
        {
            await using var db = new MySQLDatabase();
            var updated = manga as MangaEntity;
            if (updated == null)
                throw new Exception("[IMangaCheckerStorageDataProvider::UpdateCurrentChapter] Failed to update manga! " +
                                    "Reason: Storage entity and param type mismatch!");
            
            await db.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await db.GetObservedMangasInfoTable().Where(m => m.MangaId == updated.MangaId)
                .Set(m => m.CurrentChapter, chapter)
                .UpdateAsync();
            await db.CommitTransactionAsync();
        }

        public async Task UpdateChaptersInfoAsync(IManga manga, float newestChapter, int amount)
        {
            await using var db = new MySQLDatabase();
            var updated = manga as MangaEntity;
            if (updated == null)
                throw new Exception("[IMangaCheckerStorageDataProvider::UpdateCurrentChapter] Failed to update manga! " +
                                    "Reason: Storage entity and param type mismatch!");
            
            await db.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await db.GetObservedMangasInfoTable().Where(m => m.MangaId == updated.MangaId)
                .Set(m => m.NewestChapter, newestChapter)
                .Set(m => m.AmountOfChapters, amount)
                .UpdateAsync();
            await db.CommitTransactionAsync();
        }

        public void UpdateChaptersInfo(IEnumerable<(IManga manga, float newestChapter, int amount)> list)
        {
            using var db = new MySQLDatabase();
            db.BeginTransaction(IsolationLevel.ReadCommitted);
            var table = db.GetObservedMangasInfoTable();

            foreach (var (manga, newestChapter, amount) in list)
            {
                var updated = manga as MangaEntity;
                if (updated == null) continue;

                table.Where(m => m.MangaId == updated.MangaId)
                    .Set(m => m.NewestChapter, newestChapter)
                    .Set(m => m.AmountOfChapters, amount)
                    .Set(m => m.LastChaptersUpdate, DateTime.Now)
                    .Update();
            }
            
            db.CommitTransaction();
        }

        public async Task UpdateRedirectedManga(IManga oldData, IManga newData)
        {
            await using var db = new MySQLDatabase();
            if (!(oldData is MangaEntity updated))
                throw new Exception("[IMangaCheckerStorageDataProvider::UpdateRedirectedManga] Failed to update manga! " +
                                    "Reason: Storage entity and param type mismatch!");
            
            await db.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            var table = db.GetObservedMangasInfoTable();
            await table.Where(m => m.MangaId == updated.MangaId)
                .Set(m => m.SiteMangaId, newData.SiteMangaId)
                .Set(m => m.Name, newData.Name)
                .Set(m => m.Description, newData.Description)
                .Set(m => m.NewestChapter, newData.NewestChapter)
                .Set(m => m.AmountOfChapters, newData.AmountOfChapters)
                .Set(m => m.Source, newData.Source)
                .Set(m => m.LastChaptersUpdate, DateTime.Now)
                .Set(m => m.MangaImage, newData.MangaImage)
                .UpdateAsync();

            await db.CommitTransactionAsync();
        }

        public async Task<IEnumerable<IManga>> GetAllUnreadMangas()
        {
            await using var db = new MySQLDatabase();
            var table = db.GetObservedMangasInfoTable();
            return await table.Where(m => m.CurrentChapter < m.NewestChapter).ToListAsync();
        }

        public IEnumerable<IManga> GetAll()
        {
            using var db = new MySQLDatabase();
            return db.GetObservedMangasInfoTable().ToList();
        }

        public async Task<IBotSettings?> GetCurrentSettingsAsync(ulong serverId)
        {
            await using var db = new MySQLDatabase();
            try
            {
                return await db.GetServerSettingsTable().Where(r => r.ServerId == serverId).FirstAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task AssignSettingsAsync(ulong serverId, ulong inputId, ulong outputId)
        {
            await using var db = new MySQLDatabase();
            
            await db.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await db.InsertOrReplaceAsync(new ServerSettings()
            {
                ServerId = serverId,
                InputChannelId = inputId,
                OutputChannelId = outputId
            });

            await db.CommitTransactionAsync();
        }
    }
}