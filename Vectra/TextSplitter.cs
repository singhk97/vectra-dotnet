using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectra.Models;

namespace Vectra
{
    /// <summary>
    /// A class that can split text into smaller chunks based on separators and a chunk size.
    /// </summary>
    public class TextSplitter
    {
        private readonly TextSplitterConfig _config;

        /// <summary>
        /// Creates a new instance of <see cref="TextSplitter"/> with the given configuration.
        /// </summary>
        /// <param name="config">The configuration to use for splitting text.</param>
        public TextSplitter(TextSplitterConfig? config = null)
        {
            _config = config ?? new TextSplitterConfig();


            _config.Tokenizer ??= new GPT3Tokenizer();


            if (_config.Separators == null || _config.Separators.Count == 0)
            {
                _config.Separators = _getSeparators(_config.DocType);
            }


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
        /// Gets the separators for different document types.
        /// </summary>
        /// <param name="docType">The document type, optional.</param>
        /// <returns>An array of strings that are used as separators.</returns>
        private List<string> _getSeparators(string? docType = null)
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
