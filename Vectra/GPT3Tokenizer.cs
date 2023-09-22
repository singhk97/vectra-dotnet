using Microsoft.DeepDev;
using ITokenizer = Vectra.Models.ITokenizer;

namespace Vectra
{
    public class GPT3Tokenizer : ITokenizer
    {
        private Microsoft.DeepDev.ITokenizer? _tokenizer;

        public string Decode(int[] tokens)
        {
            _createTokenizerIfNotExists();

            return _tokenizer!.Decode(tokens);
        }

        public List<int> Encode(string text, IReadOnlyCollection<string>? allowedSpecial = null)
        {
            _createTokenizerIfNotExists();

            allowedSpecial ??= new List<string>();

            return _tokenizer!.Encode(text, allowedSpecial);
        }

        private async void _createTokenizerIfNotExists()
        {
            if (_tokenizer != null) return;

            _tokenizer = await TokenizerBuilder.CreateByModelNameAsync("gpt3");
        }
    }
}
