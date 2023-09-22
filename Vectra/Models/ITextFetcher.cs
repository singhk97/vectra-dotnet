namespace Vectra.Models
{
    /// <summary>
    /// An interface for fetching text from a URI.
    /// </summary>
    public interface ITextFetcher
    {
        /// <summary>
        /// Fetches text from a URI.
        /// </summary>
        /// <param name="uri">The URI to fetch text from.</param>
        /// <returns>A tuple of the text and the optional document type.</returns>
        Task<(string text, string? docType)> Fetch(string uri);
    }
}