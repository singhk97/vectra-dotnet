namespace Vectra.Models
{
    /// <summary>
    /// Statistics of an index.
    /// </summary>
    public class IndexStats
    {
        /// <summary>
        /// The version of the index.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The metadata configuration of the index.
        /// </summary>
        public MetadataConfig? MetadataConfig { get; set; }

        /// <summary>
        /// The number of items in the index.
        /// </summary>
        public int Items { get; set; }
    }
}
