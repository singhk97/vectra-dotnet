
namespace Vectra.Models
{
    /// <summary>
    /// An item in an index with metadata and vector information.
    /// </summary>
    /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
    public class IndexItem<TMetadata> where TMetadata : Metadata
    {
        /// <summary>
        /// The id of the item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The metadata of the item.
        /// </summary>
        public TMetadata Metadata { get; set; }

        /// <summary>
        /// The vector of the item.
        /// </summary>
        public float[] Vector { get; set; }

        /// <summary>
        /// The norm of the vector.
        /// </summary>
        public float Norm { get; set; }

        /// <summary>
        /// Optional. The metadata file of the item.
        /// </summary>
        public string? MetadataFile { get; set; }

        public IndexItem<TMetadata> Clone()
        {
            return new IndexItem<TMetadata>()
            {
                Id = Id,
                Metadata = (TMetadata)Metadata.Clone(),
                Vector = Vector.ToArray(),
                Norm = Norm,
                MetadataFile = MetadataFile,
            };
        }
    }
}
