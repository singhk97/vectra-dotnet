namespace Vectra.Models
{
    /// <summary>
    /// A section of text in a document.
    /// </summary>
    public class DocumentTextSection
    {
        /// <summary>
        /// The text content of the section.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The number of tokens in the section.
        /// </summary>
        public int TokenCount { get; set; }

        /// <summary>
        /// The score of the section based on some criteria.
        /// </summary>
        public double Score { get; set; }
    }

}
