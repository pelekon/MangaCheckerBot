using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MangaChecker.Core.Defines.Common;

namespace MangaChecker.Core.Entities
{
    public class MangaUpdateBatch
    {
        public List<IManga> ToUpdate { get; }
        public ConcurrentQueue<IManga> Processed { get; }
        public bool IsDone { get; private set; }
        
        public event EventHandler<BatchDoneEventArgs> OnBatchUpdateDone = delegate {  };

        public MangaUpdateBatch(List<IManga> toUpdate)
        {
            ToUpdate = toUpdate;
            Processed = new ConcurrentQueue<IManga>();
            IsDone = false;
        }

        public void MarkAsDone()
        {
            IsDone = true;
            OnBatchUpdateDone.Invoke(this, new BatchDoneEventArgs(GetUpdatedMangas()));
        }

        private IReadOnlyCollection<UpdateResult> GetUpdatedMangas()
        {
            List<UpdateResult> updatedMangas = new();
            var processedArray = Processed.ToArray();

            foreach (var pm in processedArray)
            {
                var um = ToUpdate.First(m => pm.Equals(m));

                if (um.AmountOfChapters != pm.AmountOfChapters || um.NewestChapter < pm.NewestChapter)
                    updatedMangas.Add(new UpdateResult(um, pm.NewestChapter, pm.AmountOfChapters));
            }

            return updatedMangas;
        }
    }

    public class BatchDoneEventArgs : EventArgs
    {
        public IEnumerable<UpdateResult> ProcessedMangas { get; }
        public int AmountOfProcessedMangas { get; }

        public BatchDoneEventArgs(IReadOnlyCollection<UpdateResult> processedMangas)
        {
            ProcessedMangas = processedMangas;
            AmountOfProcessedMangas = processedMangas.Count;
        }
    }

    public class UpdateResult
    {
        public IManga Source { get; }
        public float NewestChapter { get; }
        public int AmountOfChapters { get; }

        public UpdateResult(IManga source, float newestChapter, int amountOfChapters)
        {
            Source = source;
            NewestChapter = newestChapter;
            AmountOfChapters = amountOfChapters;
        }
    }
}