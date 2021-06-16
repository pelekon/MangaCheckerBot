using System.Threading.Tasks;

namespace MangaChecker.Core.Defines.DataProviders.Credentials
{
    public interface ICredentialDataProvider
    {
        public string BotToken { get; }
        
        public string DbHost { get; }
        public string DbPort { get; }
        public string DatabaseName { get; }
        public string DbUser { get; }
        public string DbPass { get; }

        public Task Load();
    }
}