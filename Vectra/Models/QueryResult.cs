
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

        /// <summary>
        /// Constructs the <see cref="QueryResult{TMetadata}"/> class.
        /// </summary>
        /// <param name="item">The item of the result.</param>
        /// <param name="score">The score of the result.</param>
        public QueryResult(IndexItem<TMetadata> item, double score)
        {
            Item = item;
            Score = score;
        }
    }
}
