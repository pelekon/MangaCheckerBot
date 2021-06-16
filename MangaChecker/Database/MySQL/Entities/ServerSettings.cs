using LinqToDB.Mapping;
using MangaChecker.Core.Defines.DataProviders.Storage;

namespace MangaChecker.Database.MySQL.Entities
{
    [Table("server_info")]
    public class ServerSettings : IBotSettings
    {
        [Column("ID")]
        [PrimaryKey]
        public ulong ServerId { get; set; }
        
        [Column("input_channel")]
        public ulong InputChannelId { get; set; }
        
        [Column("output_channel")]
        public ulong OutputChannelId { get; set; }
    }
}