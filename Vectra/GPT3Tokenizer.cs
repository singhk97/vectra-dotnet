using SharpToken;

namespace Vectra
{
    public class GPT3Tokenizer : Models.ITokenizer
    {
        private readonly GptEncoding _gptEncoding;

        internal GPT3Tokenizer()
        {
            _gptEncoding = GptEncoding.GetEncodingForModel("gpt-3.5-turbo");
        }

        /// <summary>
        /// Decodes a list of tokens into a string.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>The decoded string.</returns>
        public string Decode(List<int> tokens)
        {
            return _gptEncoding.Decode(tokens);
        }

        /// <summary>
        /// Encodes a string into a list of tokens.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <param name="allowedSpecial">The allowed special tokens.</param>
        /// <returns></returns>
        public List<int> Encode(string text, ISet<string>? allowedSpecial = null)
        {
            return _gptEncoding.Encode(text, allowedSpecial);
        }
    }
}
