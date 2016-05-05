using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Vault.Shared.Lucene
{
    public class LuceneIndexWriter : IndexWriter
    {
        bool _isClosed;

        public LuceneIndexWriter(Directory d, Analyzer a, MaxFieldLength mfl)
            : base(d, a, mfl)
        {
        }

        public LuceneIndexWriter(Directory d, Analyzer a, IndexDeletionPolicy deletionPolicy, MaxFieldLength mfl)
            : base(d, a, deletionPolicy, mfl)
        {
        }

        public LuceneIndexWriter(Directory d, Analyzer a, bool create, MaxFieldLength mfl)
            : base(d, a, create, mfl)
        {
        }

        public LuceneIndexWriter(Directory d, Analyzer a, IndexDeletionPolicy deletionPolicy, MaxFieldLength mfl, IndexCommit commit)
            : base(d, a, deletionPolicy, mfl, commit)
        {
        }

        public LuceneIndexWriter(Directory d, Analyzer a, bool create, IndexDeletionPolicy deletionPolicy, MaxFieldLength mfl)
            : base(d, a, create, deletionPolicy, mfl)
        {
        }

        public bool IsOpen()
        {
            return !this._isClosed;
        }

        public override void Rollback()
        {
            base.Rollback();
            this._isClosed = true;
        }

        public override void Dispose(bool waitForMerges)
        {
            base.Dispose(waitForMerges);
            this._isClosed = true;
        }
    }
}