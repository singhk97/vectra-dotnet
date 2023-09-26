using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectra.Models
{
    /// <summary>
    /// An interface for tokenizing text.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Decodes tokens to text.
        /// </summary>
        /// <param name="tokens">The tokens to decode.</param>
        /// <returns>The decoded text.</returns>
        string Decode(List<int> tokens);

        /// <summary>
        /// Encodes text to tokens.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <returns>The encoded tokens.</returns>
        List<int> Encode(string text, ISet<string>? allowedSpecial = null);
    }
}
