﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Vault.Shared.Search
{
    /// <summary>
    /// The document metadata for bi-directional mapping between object and lucene indexes and vice versa.
    /// </summary>
    public class IndexDocumentMetadata
    {
        private readonly IReadOnlyList<DocumentFieldDescriptor> _fields;
        private readonly IReadOnlyList<DocumentFieldDescriptor> _keys;
        private readonly IDictionary<string, string> _fieldNamesMap;
        private readonly IDictionary<string, DocumentFieldDescriptor> _fieldDescriptorMap;

        public const string KeywordsFieldName = "keywords";

        public IndexDocumentMetadata(List<DocumentFieldDescriptor> fields)
        {
            _fields = fields.AsReadOnly();
            _keys = fields.Where(t => t.IsKey).ToArray();
            _fieldNamesMap = fields.ToDictionary(t => t.Name, t => t.FieldName, StringComparer.OrdinalIgnoreCase);
            _fieldDescriptorMap = fields.ToDictionary(t => t.Name, t => t, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The list of descriptors, which describe process of storing each field in indexes.
        /// </summary>
        public IReadOnlyList<DocumentFieldDescriptor> Fields { get { return _fields; } }

        public IReadOnlyList<DocumentFieldDescriptor> Keys { get { return _keys; } }

        public string RewriteFieldName(string name)
        {
            if (string.Equals(name, KeywordsFieldName, StringComparison.OrdinalIgnoreCase))
                return KeywordsFieldName;

            return _fieldNamesMap[name];
        }

        public bool TryGetDescriptor(string name, out DocumentFieldDescriptor descriptor)
        {
            if (_fieldDescriptorMap.ContainsKey(name))
            {
                descriptor = _fieldDescriptorMap[name];
                return true;
            }

            descriptor = null;
            return false;
        }
    }
}