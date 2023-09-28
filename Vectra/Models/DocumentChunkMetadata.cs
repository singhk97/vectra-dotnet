
namespace Vectra.Models
{
    /// <summary>
    /// The metadata of a document chunk.
    /// </summary>
    public class DocumentChunkMetadata : Dictionary<string, object>
    {
        /// <summary>
        /// The id of the document.
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// The start position of the chunk in the document.
        /// </summary>
        public int StartPos { get; set; }

        /// <summary>
        /// The end position of the chunk in the document.
        /// </summary>
        public int EndPos { get; set; }
    }
}
