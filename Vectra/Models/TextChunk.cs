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
        public List<int> Tokens { get; set; }

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
        public List<int> StartOverlap { get; set; }

        /// <summary>
        /// The tokens that overlap with the next chunk.
        /// </summary>
        public List<int> EndOverlap { get; set; }

        
        /// <summary>
        /// Creates a new text chunk.
        /// </summary>
        /// <param name="text">The text of the chunk.</param>
        /// <param name="tokens">The tokens of the chunk.</param>
        /// <param name="startPos">The start position of the chunk in the original text.</param>
        /// <param name="endPos">The end position of the chunk in the original text.</param>
        /// <param name="startOverlap">The start overlap of the chunk.</param>
        /// <param name="endOverlap">The end overlap of the chunk.</param>
        public TextChunk(string text, List<int> tokens, int startPos, int endPos, List<int> startOverlap, List<int> endOverlap)
        {
            Text = text;
            Tokens = tokens;
            StartPos = startPos;
            EndPos = endPos;
            StartOverlap = startOverlap;
            EndOverlap = endOverlap;
        }
    }
}