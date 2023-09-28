
namespace Vectra.Models
{
    /// <summary>
    /// A configuration for splitting text into chunks.
    /// </summary>
    public class TextSplitterConfig
    {
        /// <summary>
        /// The separators to use for splitting text.
        /// </summary>
        public List<string> Separators { get; set; }

        /// <summary>
        /// Whether to keep the separators in the chunks or not.
        /// </summary>
        public bool KeepSeparators { get; set; } = false;

        /// <summary>
        /// The maximum size of each chunk in tokens.
        /// </summary>
        public int ChunkSize { get; set; } = 400;

        /// <summary>
        /// The number of tokens to overlap between adjacent chunks.
        /// </summary>
        public int ChunkOverlap { get; set; } = 40;

        /// <summary>
        /// The tokenizer to use for converting text to tokens.
        /// </summary>
        public ITokenizer Tokenizer { get; set; }

        /// <summary>
        /// The optional document type to assign to the chunks.
        /// </summary>
        public string? DocType { get; set; }

        public TextSplitterConfig()
        {
            Separators = TextSplitter.GetSeparators(DocType);
            Tokenizer = new GPT3Tokenizer();
        }
    }

}
