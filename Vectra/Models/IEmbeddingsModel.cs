namespace Vectra.Models
{
    /// <summary>
    /// An AI model that can be used to create embeddings.
    /// </summary>
    public interface IEmbeddingsModel
    {
        /// <summary>
        /// Max number of tokens.
        /// </summary>
        int MaxTokens { get; }

        /// <summary>
        /// Creates embeddings for the given inputs.
        /// </summary>
        /// <param name="inputs">Text inputs to create embeddings for.</param>
        /// <returns>A <see cref="EmbeddingsResponse"/> with a status and the generated embeddings or a message when an error occurs.</returns>
        Task<EmbeddingsResponse> CreateEmbeddings(List<string> inputs);
    }

    // <summary>
    /// Status of the embeddings response.
    /// </summary>
    /// <remarks>
    /// <para>Success - The embeddings were successfully created.</para>
    /// <para>Error - An error occurred while creating the embeddings.</para>
    /// <para>RateLimited - The request was rate limited.</para>
    /// </remarks>
    public enum EmbeddingsResponseStatus {
        Success,
        Error,
        RateLimited
    }
}