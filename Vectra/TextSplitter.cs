using System.Runtime.CompilerServices;
using Vectra.Models;

[assembly:InternalsVisibleTo("Vectra.Tests")]
namespace Vectra
{
    /// <summary>
    /// A class that can split text into smaller chunks based on separators and a chunk size.
    /// </summary>
    public class TextSplitter
    {
        private readonly TextSplitterConfig _config;
        private readonly string ALPHANUMERIC_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Creates a new instance of <see cref="TextSplitter"/> with the given configuration.
        /// </summary>
        /// <param name="config">The configuration to use for splitting text.</param>
        public TextSplitter(TextSplitterConfig? config = null)
        {
            _config = config ?? new TextSplitterConfig();

            if (_config.ChunkSize < 1)
            {
                throw new ArgumentException("chunkSize must be >= 1");
            }
            else if (_config.ChunkOverlap < 0)
            {
                throw new ArgumentException("chunkOverlap must be >= 0");
            }
            else if (_config.ChunkOverlap > _config.ChunkSize)
            {
                throw new ArgumentException("chunkOverlap must be <= chunkSize");
            }
        }

        /// <summary>
        /// Splits the given text into chunks.
        /// </summary>
        /// <param name="text">The text to split.</param>
        /// <returns>A list of text chunks.</returns>
        public async Task<List<TextChunk>> Split(string text)
        {
            // Get basic chunks
            var chunks = await RecursiveSplit(text, _config.Separators, 0);

            List<int> GetOverlapTokens(List<int>? tokens, bool fromStart = true)
            {
                if (tokens != null)
                {
                    var len = tokens.Count > _config.ChunkOverlap ? _config.ChunkOverlap : tokens.Count;
                    if (fromStart)
                    {
                        return tokens.GetRange(0, len);
                    }
                    else
                    {
                        return tokens.GetRange(tokens.Count - len, len);
                    }
                }
                else
                {
                    return new List<int>();
                }
            }

            // Add overlap tokens and text to the start and end of each chunk
            if (_config.ChunkOverlap > 0)
            {
                for (int i = 1; i < chunks.Count; i++)
                {
                    var previousChunk = chunks[i - 1];
                    var chunk = chunks[i];
                    var nextChunk = i < chunks.Count - 1 ? chunks[i + 1] : null;

                    chunk.StartOverlap = GetOverlapTokens(previousChunk.Tokens, fromStart: false);
                    chunk.EndOverlap = GetOverlapTokens(nextChunk?.Tokens);
                }
            }

            return chunks;
        }

        internal async Task<List<TextChunk>> RecursiveSplit(string text, List<string> separators, int startPos)
        {
            var chunks = new List<TextChunk>();
            if (text.Length > 0)
            {
                // Split text into parts
                List<string> parts;
                string separator = string.Empty;
                var nextSeparators = separators.Count > 1 ? separators.GetRange(1, separator.Length) : new List<string>();
                if (separators.Count > 0)
                {
                    // Split by separator
                    separator = separators[0];
                    parts = text.Split(separator).ToList();
                }
                else
                {
                    // Cut text in half
                    int half = (int)Math.Floor((double)text.Length / 2);
                    parts = new List<string> { text[0..half], text[half..] };
                }

                // Iterate over parts
                for (int i = 0; i < parts.Count; i++)
                {
                    var lastChunk = (i == parts.Count - 1);

                    // Get chunk text and endPos
                    var chunk = parts[i];
                    var endPos = (startPos + (chunk.Length - 1)) + (lastChunk ? 0 : separator.Length);
                    if (_config.KeepSeparators && !lastChunk)
                    {
                        chunk += separator;
                    }

                    // Ensure chunk contains text
                    if (!ContainsAlphanumeric(chunk))
                    {
                        continue;
                    }

                    // Optimization to avoid encoding really large chunks
                    if (chunk.Length / 6 > _config.ChunkSize)
                    {
                        // Break the text into smaller chunks
                        var subChunks = await RecursiveSplit(chunk, nextSeparators, startPos);
                        chunks.AddRange(subChunks);
                    }
                    else
                    {
                        // Encode chunk text
                        var tokens = _config.Tokenizer.Encode(chunk);
                        if (tokens.Count > _config.ChunkSize)
                        {
                            // Break the text into smaller chunks
                            var subChunks = await RecursiveSplit(chunk, nextSeparators, startPos);
                            chunks.AddRange(subChunks);
                        }
                        else
                        {
                            // Append chunk to output
                            chunks.Add(new TextChunk(
                                    text: chunk,
                                    tokens: tokens,
                                    startPos: startPos,
                                    endPos: endPos,
                                    startOverlap: new List<int>(),
                                    endOverlap: new List<int>()
                                )
                            );
                        }

                    }


                    // Update startPos
                    startPos = endPos + 1;
                }
            }

            return CombineChunks(chunks);
        }

        internal List<TextChunk> CombineChunks(List<TextChunk> chunks)
        {
            var combinedChunks = new List<TextChunk>();
            TextChunk? currentChunk = null;
            int currentLength = 0;
            var separator = _config.KeepSeparators ? string.Empty : " ";
            for (int i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                if (currentChunk != null)
                {
                    var length = currentChunk.Tokens.Count + chunk.Tokens.Count;
                    if (length > _config.ChunkSize)
                    {
                        combinedChunks.Add(currentChunk);
                        currentChunk = chunk;
                        currentLength = chunk.Tokens.Count;
                    }
                    else
                    {
                        currentChunk.Text += separator + chunk.Text;
                        currentChunk.Tokens = currentChunk.Tokens.Concat(chunk.Tokens).ToList();
                        currentLength += chunk.Tokens.Count;
                    }
                }
                else
                {
                    currentChunk = chunk;
                    currentLength = chunk.Tokens.Count;
                }
            }
            if (currentChunk != null)
            {
                combinedChunks.Add(currentChunk);
            }
            return combinedChunks;
        }

        private bool ContainsAlphanumeric(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (ALPHANUMERIC_CHARS.Contains(text[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the separators for different document types.
        /// </summary>
        /// <param name="docType">The document type, optional.</param>
        /// <returns>An array of strings that are used as separators.</returns>
        internal static List<string> GetSeparators(string? docType = null)
        {
            switch (docType?.ToLower() ?? "")
            {
                case "cpp":
                    return new List<string> { "\nclass ", "\nvoid ", "\nint ", "\nfloat ", "\ndouble ", "\nif ", "\nfor ", "\nwhile ", "\nswitch ", "\ncase ", "\n\n", "\n", " " };
                case "go":
                    return new List<string> { "\nfunc ", "\nvar ", "\nconst ", "\ntype ", "\nif ", "\nfor ", "\nswitch ", "\ncase ", "\n\n", "\n", " " };
                case "java":
                case "c#":
                case "csharp":
                case "cs":
                case "ts":
                case "tsx":
                case "typescript":
                    return new List<string> { "\nclass ", "\npublic ", "\nprotected ", "\nprivate ", "\nstatic ", "\nif ", "\nfor ", "\nwhile ", "\nswitch ", "\ncase ", "\n\n", "\n", " " };
                case "js":
                case "jsx":
                case "javascript":
                    return new List<string> { "\nclass ", "\nfunction ", "\nconst ", "\nlet ", "\nvar ", "\nclass ", "\nif ", "\nfor ", "\nwhile ", "\nswitch ", "\ncase ", "\ndefault ", "\n\n", "\n", " " };
                case "php":
                    return new List<string> { "\nfunction ", "\nclass ", "\nif ", "\nforeach ", "\nwhile ", "\ndo ", "\nswitch ", "\ncase ", "\n\n", "\n", " " };
                case "proto":
                    return new List<string> { "\nmessage ", "\nservice ", "\nenum ", "\noption ", "\nimport ", "\nsyntax ", "\n\n", "\n", " " };
                case "python":
                case "py":
                    return new List<string> { "\nclass ", "\ndef ", "\n\tdef ", "\n\n", "\n", " " };
                case "rst":
                    return new List<string> { "\n===\n", "\n---\n", "\n***\n", "\n.. ", "\n\n", "\n", " " };
                case "ruby":
                    return new List<string> { "\ndef ", "\nclass ", "\nif ", "\nunless ", "\nwhile ", "\nfor ", "\ndo ", "\nbegin ", "\nrescue ", "\n\n", "\n", " " };
                case "rust":
                    return new List<string> { "\nfn ", "\nconst ", "\nlet ", "\nif ", "\nwhile ", "\nfor ", "\nloop ", "\nmatch ", "\nconst ", "\n\n", "\n", " " };
                case "scala":
                    return new List<string> { "\nclass ", "\nobject ", "\ndef ", "\nval ", "\nvar ", "\nif ", "\nfor ", "\nwhile ", "\nmatch ", "\ncase ", "\n\n", "\n", " " };
                case "swift":
                    return new List<string> { "\nfunc ", "\nclass ", "\nstruct ", "\nenum ", "\nif ", "\nfor ", "\nwhile ", "\ndo ", "\nswitch ", "\ncase ", "\n\n", "\n", " " };
                case "md":
                case "markdown":
                    return new List<string> { "\n## ", "\n### ", "\n#### ", "\n##### ", "\n###### ", "```\n\n", "\n\n***\n\n", "\n\n---\n\n", "\n\n___\n\n", "<table>", "\n\n", "\n", " " };
                case "latex":
                    return new List<string> { "\n\\chapter{", "\n\\section{", "\n\\subsection{", "\n\\subsubsection{", "\n\\begin{enumerate}", "\n\\begin{itemize}", "\n\\begin{description}", "\n\\begin{list}", "\n\\begin{quote}", "\n\\begin{quotation}", "\n\\begin{verse}", "\n\\begin{verbatim}", "\n\\begin{align}", "$$", "$", "\n\n", "\n", " " };
                case "html":
                    return new List<string> { "<body>", "<div>", "<p>", "<br>", "<li>", "<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>", "<span>", "<table>", "<tr>", "<td>", "<th>", "<ul>", "<ol>", "<header>", "<footer>", "<nav>", "<head>", "<style>", "<script>", "<meta>", "<title>", " " };
                case "sol":
                    return new List<string> { "\npragma ", "\nusing ", "\ncontract ", "\ninterface ", "\nlibrary ", "\nconstructor ", "\ntype ", "\nfunction ", "\nevent ", "\nmodifier ", "\nerror ", "\nstruct ", "\nenum ", "\nif ", "\nfor ", "\nwhile ", "\ndo while ", "\nassembly ", "\n\n", "\n", " " };
                default:
                    return new List<string> { "\n\n", "\n", " ", "" };
            }
        }

    }
}
