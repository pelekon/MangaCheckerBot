namespace MangaChecker.Core.Defines.DataProviders.Storage
{
    public interface IBotSettings
    {
        public ulong ServerId { get; }
        public ulong InputChannelId { get; }
        public ulong OutputChannelId { get; }
    }
}