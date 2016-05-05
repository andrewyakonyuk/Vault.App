using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Lucene
{
    /// <summary>
    /// The document metadata for bi-directional mapping between object and lucene indexes and vice versa.
    /// </summary>
    public class IndexDocumentMetadata
    {
        /// <summary>
        /// The document metadata type
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// The list of descriptors, which describe process of storing each field in indexes.
        /// </summary>
        public IList<DocumentFieldDescriptor> Fields { get; set; }

        public IList<DocumentFieldDescriptor> Keys
        {
            get { return Fields.Where(t => t.IsKey).ToArray(); }
        }
    }
}