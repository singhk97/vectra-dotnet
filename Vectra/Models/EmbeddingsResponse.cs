namespace Vectra.Models 
{
    /// <summary>
    /// Response returned by a <see cref="IEmbeddingsClient"/>.
    /// </summary>
    public class EmbeddingsResponse {
        /// <summary>
        /// Status of the embeddings response.
        /// </summary>
        public EmbeddingsResponseStatus Status { get; set; }

        /// <summary>
        /// Optional. Embeddings for the given inputs.
        /// </summary>
        public double[][]? Output { get; set; }

        /// <summary>
        /// Optional. Message when status is not equal to <see cref="EmbeddingsResponseStatus.Success"/>.
        /// </summary>
        public string? Message { get; set; }
    }
}