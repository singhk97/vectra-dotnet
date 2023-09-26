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

        public string Decode(List<int> tokens)
        {
            return _gptEncoding.Decode(tokens);
        }

        public List<int> Encode(string text, ISet<string>? allowedSpecial = null)
        {
            return _gptEncoding.Encode(text, allowedSpecial);
        }
    }
}
