using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System.IO;
using Version = Lucene.Net.Util.Version;

namespace Vault.Shared.Search.Lucene.Analyzers
{
    public class LowerCaseAnalyzer : StandardAnalyzer
    {
        public LowerCaseAnalyzer(Version version)
            : base(version)
        {
        }

        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
            return new LowerCaseFilter(base.ReusableTokenStream(fieldName, reader));
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new LowerCaseFilter(base.TokenStream(fieldName, reader));
        }
    }
}