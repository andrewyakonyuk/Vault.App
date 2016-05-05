using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Version = Lucene.Net.Util.Version;

namespace Vault.Shared.Lucene.Analyzers
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
    }
}