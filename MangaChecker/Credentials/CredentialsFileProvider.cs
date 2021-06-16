using System.IO;
using System.Threading.Tasks;

namespace MangaChecker.Credentials
{
    public class CredentialsFileProvider
    {
        public async Task<string> GetFileCredentialsFile() => await File.ReadAllTextAsync("Credentials/credentials.json");
    }
}