using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vault.Shared.Search;
using Vault.Shared.Search.Lucene;

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

        public virtual Task ExecuteAsync(IEnumerable<TDocument> documents, IndexStore indexStore)
        {
            using (var uow = indexStore.CreateUnitOfWork())
            {
                var compiledMapFunc = Map.Compile();
                foreach (var result in compiledMapFunc(documents))
                {
                    var document = new SearchDocument(ObjectDictionary.Create(result));
                    uow.Save(document);
                }
                uow.Commit();
            }

            return Task.FromResult(true);
        }
    }
}