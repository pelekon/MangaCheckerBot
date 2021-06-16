using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;
using MangaChecker.Core.Defines.DataProviders.Credentials;

namespace MangaChecker.Database.MySQL
{
    public class MySQLDatabaseSettings : ILinqToDBSettings
    {
        private readonly ICredentialDataProvider _credentialDataProvider;

        public MySQLDatabaseSettings(ICredentialDataProvider credentialDataProvider)
        {
            _credentialDataProvider = credentialDataProvider;
        }
        
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();
        public string? DefaultConfiguration { get; } = "MySqlConnector";
        public string? DefaultDataProvider { get; } = "MySqlConnector";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings => new[]
        {
            new ConnectionStringSettings("MySQLDatabase", "MySqlConnector",
                $"Server={_credentialDataProvider.DbHost};Port={_credentialDataProvider.DbPort};Database={_credentialDataProvider.DatabaseName};" +
                $"Uid={_credentialDataProvider.DbUser};Pwd={_credentialDataProvider.DbPass};AllowUserVariables=True")
        };
    }

    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; }
        public string Name { get; }
        public string? ProviderName { get; }
        public bool IsGlobal { get; }

        public ConnectionStringSettings(string name, string providerName, string connectionString)
        {
            Name = name;
            ProviderName = providerName;
            ConnectionString = connectionString;
            IsGlobal = false;
        }
    }
}