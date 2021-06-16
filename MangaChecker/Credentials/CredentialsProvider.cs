using System.Threading.Tasks;
using MangaChecker.Core.Defines.DataProviders.Credentials;
using Newtonsoft.Json;

namespace MangaChecker.Credentials
{
    public class CredentialsProvider : ICredentialDataProvider
    {
        private CredentialsDataJson _dataJson;
        public string BotToken => _dataJson.BotToken;
        public string DbHost => _dataJson.DbHost;
        public string DbPort => _dataJson.DbPort;
        public string DatabaseName => _dataJson.DatabaseName;
        public string DbUser => _dataJson.DbUser;
        public string DbPass => _dataJson.DbPass;

        public async Task Load()
        {
            string fileContent = await new CredentialsFileProvider().GetFileCredentialsFile();
            _dataJson = JsonConvert.DeserializeObject<CredentialsDataJson>(fileContent);
        }
    }
}