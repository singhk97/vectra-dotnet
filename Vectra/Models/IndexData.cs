using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vectra.Models
{
    /// <summary>
    /// Represents the data of an index.
    /// </summary>
    public class IndexData<TMetadata> where TMetadata : Metadata
    {
        private static readonly JsonSerializerOptions _jsonSerializationOptions = new()
        {
            WriteIndented = true,
        };

        /// <summary>
        /// The version of the index.
        /// </summary>
        [JsonPropertyName("version")]
        [JsonInclude]
        public int Version { get; set; }

        /// <summary>
        /// The configuration of the metadata fields to be indexed.
        /// </summary>
        [JsonPropertyName("metadataConfig")]
        [JsonInclude]
        public MetadataConfig? MetadataConfig { get; set; }

        /// <summary>
        /// The items in the index.
        /// </summary>
        [JsonPropertyName("items")]
        [JsonInclude]
        public List<IndexItem<TMetadata>> Items { get; set; }


        public IndexData<TMetadata> Clone()
        {
            int version = Version;
            MetadataConfig metadataConfig = new()
            {
                Indexed = MetadataConfig?.Indexed?.ToArray(),
            };
            List<IndexItem<TMetadata>> new_items = new();

            for (int i = 0; i < Items.Count; i++)
            {
                IndexItem<TMetadata> item = Items[i];

                new_items.Add(item.Clone());
            }

            return new IndexData<TMetadata>()
            {
                Version = version,
                MetadataConfig = metadataConfig,
                Items = new_items,
            };
        }

        internal string GetJson()
        {
            return JsonSerializer.Serialize(this, _jsonSerializationOptions);
        }
    }
}
