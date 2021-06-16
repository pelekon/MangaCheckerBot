using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace MangaChecker.Credentials
{
    [ExcludeFromCodeCoverage]
    public struct CredentialsDataJson
    {
        [JsonProperty(PropertyName = "bot_token")]
        public string BotToken { get; set; }
        
        [JsonProperty(PropertyName = "db_host")]
        public string DbHost { get; set; }
        
        [JsonProperty(PropertyName = "db_port")]
        public string DbPort { get; set; }
        
        [JsonProperty(PropertyName = "db_name")]
        public string DatabaseName { get; set; }
        
        [JsonProperty(PropertyName = "db_user")]
        public string DbUser { get; set; }
        
        [JsonProperty(PropertyName = "db_password")]
        public string DbPass { get; set; }
    }
}