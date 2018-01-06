using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System.IO;
using Version = Lucene.Net.Util.Version;

namespace Vault.Shared.Search.Lucene.Analyzers
{
    public class KeywordsAnalyzer : Analyzer
    {
        readonly Version _version;
        readonly string[] _stopWords;

        public KeywordsAnalyzer(Version version, string[] stopWords)
        {
            _version = version;
            _stopWords = stopWords;
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            TokenStream result = new StandardTokenizer(_version, reader);

            result = new StandardFilter(result);
            result = new LowerCaseFilter(result);
            result = new ASCIIFoldingFilter(result);
            result = new StopFilter(false, result, StopFilter.MakeStopSet(_stopWords));
            result = new EdgeNGramTokenFilter(result, EdgeNGramTokenFilter.Side.FRONT, 1, 20);
            result = new EdgeNGramTokenFilter(result, EdgeNGramTokenFilter.Side.BACK, 1, 20);

            return result;
        }
    }
}