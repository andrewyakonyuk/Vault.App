using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Vault.Shared.Search.Lucene;
using Vault.Shared.Search.Lucene.Analyzers;
using System.IO;
using Vault.Shared.Search;

namespace Vault.Activity.Host
{
    public class IndexWriterInitializer : IIndexWriterInitializer
    {
        public const string InMemory = ":memory:";

        private readonly IConfiguration _configuration;

        public IndexWriterInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LuceneIndexWriter Create(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                indexName = IndexNames.Default;

            Lucene.Net.Store.Directory directory;
            var shouldCreate = true;

            if (_configuration["connectionStrings:index"] == InMemory)
            {
                directory = new RAMDirectory();
            }
            else
            {
                var safeIndexName = string.Join("_", indexName.Split(Path.GetInvalidFileNameChars()));
                var pathToIndex = Path.Combine(_configuration["connectionStrings:index"], safeIndexName);
                directory = FSDirectory.Open(pathToIndex);
                shouldCreate = !((FSDirectory)directory).Directory.Exists || !directory.ListAll().Any();
            }

            var analyzer = new PerFieldAnalyzer(new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            analyzer.AddAnalyzer("keywords", new KeywordsAnalyzer(Lucene.Net.Util.Version.LUCENE_30, new string[0]));

            var writer = new LuceneIndexWriter(directory, analyzer, shouldCreate, IndexWriter.MaxFieldLength.UNLIMITED);
            writer.Commit();

            return writer;
        }
    }
}