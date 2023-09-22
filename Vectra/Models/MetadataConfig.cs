using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectra.Models
{
    /// <summary>
    /// The metadata configuration of an index.
    /// </summary>
    public class MetadataConfig
    {
        /// <summary>
        /// Optional. The metadata fields that are indexed.
        /// </summary>
        public string[]? Indexed { get; set; }
    }
}
