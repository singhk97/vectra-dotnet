namespace Vectra.Models
{
    /// <summary>
    /// A filter for metadata.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    /// <returns></returns>
    public delegate Task<bool> MetadataFilter(Metadata metadata);

    /// <summary>
    /// A dictionary of the metadata associated with a embedding in the index.
    /// </summary>
    public class Metadata : Dictionary<string, object>
    {
        /// <summary>
        /// Constructs the <see cref="Metadata"/> class.
        /// </summary>
        public Metadata() : base()
        {
        }

        /// <summary>
        /// Constructs the <see cref="Metadata"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public Metadata(Dictionary<string, object> dictionary) : base(dictionary)
        {
        }


        public Metadata Clone()
        {
            return new Metadata(this);
        }
    }
}
