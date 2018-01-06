using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Search.Lucene.Analyzers
{
    public class PerFieldAnalyzer : Analyzer
    {
        readonly Analyzer _defaultAnalyzer;
        readonly IDictionary<string, Analyzer> _fieldAnalyzersMap;

        public PerFieldAnalyzer(Analyzer defaultAnalyzer)
        {
            _defaultAnalyzer = defaultAnalyzer;
            _fieldAnalyzersMap = new Dictionary<string, Analyzer>();
        }

        public void AddAnalyzer(string fieldName, Analyzer analyzer)
        {
            _fieldAnalyzersMap[fieldName] = analyzer;
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            Analyzer fieldAnalyzer;
            if (_fieldAnalyzersMap.TryGetValue(fieldName, out fieldAnalyzer))
            {
                return fieldAnalyzer.TokenStream(fieldName, reader);
            }
            return _defaultAnalyzer.TokenStream(fieldName, reader);
        }
    }
}