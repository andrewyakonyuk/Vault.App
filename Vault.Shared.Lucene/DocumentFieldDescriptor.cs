using System.ComponentModel;

namespace Vault.Shared.Lucene
{
    /// <summary>
    /// Describes the specific field in the document
    /// </summary>
    public class DocumentFieldDescriptor
    {
        public static TypeConverter DefaultConverter = new StringConverter();

        public DocumentFieldDescriptor(string name, string fieldName)
        {
            Name = name;
            FieldName = fieldName;
            Converter = DefaultConverter;
        }

        public DocumentFieldDescriptor(string name)
            : this(name, name)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether field must be stored in the indexes and can be retrieved from them.
        /// </summary>
        public bool IsStored { get; set; }

        /// <summary>
        /// Gets or sets name of field
        /// </summary>
        public string Name { get; set; }

        public bool IsIndexed { get; set; }

        public bool IsAnalysed { get; set; }

        public bool OmitNorms { get; set; }

        /// <summary>
        /// Gets or sets converter to transform a field value to string and vice versa
        /// </summary>
        public TypeConverter Converter { get; set; }

        /// <summary>
        /// Gets or sets a name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether field can be use to provide search suggestions
        /// </summary>
        public bool IsKeyword { get; set; }

        public bool IsKey { get; set; }
    }
}