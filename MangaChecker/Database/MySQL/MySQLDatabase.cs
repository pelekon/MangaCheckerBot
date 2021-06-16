using LinqToDB;
using LinqToDB.Data;
using MangaChecker.Database.MySQL.Entities;

namespace MangaChecker.Database.MySQL
{
    public class MySQLDatabase : DataConnection
    {
        public MySQLDatabase() : base("MySQLDatabase") {}
        
        public ITable<MangaEntity> GetObservedMangasInfoTable() => GetTable<MangaEntity>();

        public ITable<ServerSettings> GetServerSettingsTable() => GetTable<ServerSettings>();
    }
}