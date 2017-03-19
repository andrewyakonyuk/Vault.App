using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vault.Shared.Search;

namespace Vault.Activity.Indexes
{
    public abstract class AbstractIndexCreationTask<TDocument, TReduceResult> : AbstractIndexCreationTask<TDocument>
    {
        protected Expression<Func<IEnumerable<TReduceResult>, IEnumerable>> Reduce { get; set; }

        public override bool IsMapReduce
        {
            get { return Reduce != null; }
        }
    }

    public abstract class AbstractIndexCreationTask<TDocument>
    {
        public virtual string IndexName => GetType().Name.Replace("_", "/");

        protected Expression<Func<IEnumerable<TDocument>, IEnumerable>> Map { get; set; }

        public virtual IndexDocumentMetadata GetIndexMetadata()
        {
            return new IndexDocumentMetadata(new List<DocumentFieldDescriptor>());
        }

        public virtual bool IsMapReduce => false;
    }

    public interface IIndexCreationTask
    {
        string IndexName { get; }

        IndexDocumentMetadata GetIndexMetadata();

        bool IsMapReduce { get; }
    }
}