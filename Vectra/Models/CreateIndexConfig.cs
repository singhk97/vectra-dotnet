
namespace Vectra.Models
{
    /// <summary>
    /// Configuration for creating an index.
    /// </summary>
    public class CreateIndexConfig
    {
        /// <summary>
        /// Version number of the index.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Whether to delete the existing index if it exists.
        /// </summary>
        public bool DeleteIfExists { get; set; }

        /// <summary>
        /// Configuration for the metadata fields to be indexed.
        /// </summary>
        public MetadataConfig? MetadataConfig { get; set; }
    }
}
