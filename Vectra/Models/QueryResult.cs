
namespace Vectra.Models
{
    /// <summary>
    /// A result of a query with an item and a score.
    /// </summary>
    /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
    public class QueryResult<TMetadata> where TMetadata : Metadata
    {
        /// <summary>
        /// The item of the result.
        /// </summary>
        public IndexItem<TMetadata> Item { get; set; }

        /// <summary>
        /// The score of the result.
        /// </summary>
        public double Score { get; set; }
    }
}
