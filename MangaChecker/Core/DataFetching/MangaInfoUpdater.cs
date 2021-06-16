using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using MangaChecker.Core.Defines;
using MangaChecker.Core.Defines.Common;
using MangaChecker.Core.Defines.DataProviders.Storage;
using MangaChecker.Core.Entities;
using MangaChecker.Core.Messages;
using MangaChecker.Core.Messages.MessageWrappers;
using MangaChecker.Core.Sources;

namespace MangaChecker.Core.DataFetching
{
    public class MangaInfoUpdater
    {
        private readonly DiscordSocketClient _client;
        private readonly IMangaCheckerStorageDataProvider _storageDataProvider;
        private readonly MessageBroadcaster _messageBroadcaster;
        private readonly CommandReactionHandler _reactionHandler;

        public MangaInfoUpdater(DiscordSocketClient client, IMangaCheckerStorageDataProvider storageDataProvider,
            MessageBroadcaster messageBroadcaster, CommandReactionHandler reactionHandler)
        {
            _client = client;
            _storageDataProvider = storageDataProvider;
            _messageBroadcaster = messageBroadcaster;
            _reactionHandler = reactionHandler;
        }

        private int updateInterval = 15;
        private Timer? _updatingTimer;

        public void StartUpdating()
        {
            ClearTimerIfNeeded();
            _updatingTimer = new Timer
            {
                Interval = updateInterval * 60 * 1000,
                AutoReset = true,
                Enabled = true
            };
            _updatingTimer.Elapsed += OnUpdateTimerTick;
            // instantly trigger update as StartUpdating is called on creation of bot context, so at start of program
            PerformBulkUpdate();
        }

        private void ClearTimerIfNeeded()
        {
            if (_updatingTimer == null)
                return;
            _updatingTimer.Stop();
            _updatingTimer.Close();
            _updatingTimer = null;
        }
        
        private void OnUpdateTimerTick(object sender, ElapsedEventArgs e)
        {
            PerformBulkUpdate();
        }

        private void PerformBulkUpdate()
        {
            Console.WriteLine("[MangaInfoUpdater::PerformBulkUpdate]Performing bulk update of stored mangas...");
            var allMangas = _storageDataProvider.GetAll();
            var batches = PrepareUpdateBatches(allMangas);
            // start process of updating mangas and silence warning about not awaited task result
            #pragma warning disable 4014
            StartUpdateProcess(batches);
            #pragma warning restore 4014
        }

        private async Task StartUpdateProcess(IReadOnlyCollection<MangaUpdateBatch> batches)
        {
            Console.WriteLine($"[{DateTime.Now}][MangaInfoUpdater::PerformBulkUpdate] Start of processing {batches.Count}");
            foreach (var batch in batches)
            {
                Console.WriteLine($"[{DateTime.Now}][MangaInfoUpdater::PerformBulkUpdate] Processing batch..");
                await ProcessBatch(batch);
                batch.MarkAsDone();
                // wait with processing next batch as we dont want to get banned for flooding
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        private async Task ProcessBatch(MangaUpdateBatch batch)
        {
            List<Task> tasks = new(batch.ToUpdate.Count);
            foreach (var u in batch.ToUpdate)
            {
                var manga = u;
                tasks.Add(Task.Run(async () => await ProcessBatchNode(manga, batch)));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessBatchNode(IManga manga, MangaUpdateBatch batch)
        {
            var source = MangaSourceFabric.Instance.GetSource(manga.Source);
            if (source == null)
            {
                Console.WriteLine($"[{DateTime.Now}][MangaInfoUpdater::ProcessBatch] Failed to get source object for type {manga.Source}");
                return;
            }

            MangaInfoFetcher infoFetcher = new();
            var data = await infoFetcher.FetchInfo(source, manga.SiteMangaId, true);

            if (data.IsFailed)
                await LogProcessError(manga.Name, data.FailReason ?? "Unknown");
            else
                batch.Processed.Enqueue(new FetchedMangaData(manga, data.ChaptersList));
        }
        
        private void OnOnBatchUpdateDone(object? sender, BatchDoneEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now}][MangaInfoUpdater::PerformBulkUpdate] OnBatchDone...");
            List<(IManga Manga, float newestChapter, int amount)> list = new(e.AmountOfProcessedMangas);
            
            foreach (var item in e.ProcessedMangas)
                list.Add((item.Source, item.NewestChapter, item.AmountOfChapters));

            if (list.Count != 0)
            {
                _storageDataProvider.UpdateChaptersInfo(list);
                SendUpdateMessages(list);
            }
        }

        private void SendUpdateMessages(List<(IManga Manga, float newestChapter, int amount)> list)
        {
            foreach (var node in list)
                SendUpdateMessage(node.Manga, node.newestChapter, node.amount);
        }

        private void SendUpdateMessage(IManga source, float chapter, int amount)
        {
            string newChPart = source.NewestChapter != chapter ? $"New chapter: **{chapter}**\n" : "";
            IMangaSource? mangaSource = MangaSourceFabric.Instance.GetSource(source.Source);
            string desc = $"Manga **\"{ source.Name }**\" got update!\n" +
                          $"{mangaSource?.GetUrlWithMangaId(source.SiteMangaId) ?? ""}\n" +
                          $"{newChPart}Amount of chapters:\nPrevious: **{source.AmountOfChapters}** " +
                          $"Current: **{amount}**";
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("Update detected!")
                .WithDescription(desc)
                .WithColor(Color.DarkOrange)
                .WithFooter("Click on reaction to auto update current chapter to newest.");

            if (source.MangaImage != null)
                builder.WithImageUrl(source.MangaImage);
            
            #pragma warning disable 4014
            _messageBroadcaster.SendMessageAsync(builder.Build(), async message =>
            {
                await message.AddReactionAsync(new Emoji(MessageReactions.EmoteWhiteCheckMark));
                _reactionHandler.AddMessage(new ChapterUpdatingMessage(source, chapter, _client, _storageDataProvider,
                    message.Id, message.Author.Id, DateTime.Now, message.Channel.Id));
            });
            #pragma warning restore 4014
        }

        private IReadOnlyCollection<MangaUpdateBatch> PrepareUpdateBatches(IEnumerable<IManga> mangas)
        {
            List<MangaUpdateBatch> batches = new();
            var currentBatch = CreateBatch();
            int counter = 0;
            foreach (var m in mangas)
            {
                if (counter != 0 && counter % 15 == 0)
                {
                    batches.Add(currentBatch);
                    currentBatch = CreateBatch();
                }
                
                currentBatch.ToUpdate.Add(m);
                ++counter;
            }

            batches.Add(currentBatch);
            return batches;
        }

        private MangaUpdateBatch CreateBatch()
        {
            var batch = new MangaUpdateBatch(new List<IManga>());
            batch.OnBatchUpdateDone += OnOnBatchUpdateDone;
            return batch;
        }

        private async Task LogProcessError(string mangaName, string reason)
        {
            Console.WriteLine($"[{DateTime.Now}] {reason}");
            EmbedBuilder builder = new();
            builder.WithTitle("Error!")
                .WithDescription($"**Error during manga update process!**\nAffected manga: **{mangaName}**\nReason: {reason}")
                .WithColor(Color.Red);
            await _messageBroadcaster.SendMessageAsync(builder.Build());
        }
    }
}