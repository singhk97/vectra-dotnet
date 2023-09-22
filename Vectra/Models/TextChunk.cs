namespace Vectra.Models
{
    /// <summary>
    /// A chunk of text with tokenization information.
    /// </summary>
    public class TextChunk
    {

        /// <summary>
        /// The text of the chunk.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The tokens of the chunk.
        /// </summary>
        public int[] Tokens { get; set; }

        /// <summary>
        /// The start position of the chunk in the original text.
        /// </summary>
        public int StartPos { get; set; }

        /// <summary>
        /// The end position of the chunk in the original text.
        /// </summary>
        public int EndPos { get; set; }

        /// <summary>
        /// The tokens that overlap with the previous chunk.
        /// </summary>
        public int[] StartOverlap { get; set; }

        /// <summary>
        /// The tokens that overlap with the next chunk.
        /// </summary>
        public int[] EndOverlap { get; set; }
    }
}