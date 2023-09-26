using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectra.Models
{
    /// <summary>
    /// Statistics of a document catalog.
    /// </summary>
    public class DocumentCatalogStats
    {
        /// <summary>
        /// The version of the catalog.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The number of documents in the catalog.
        /// </summary>
        public int Documents { get; set; }

        /// <summary>
        /// The number of chunks in the catalog.
        /// </summary>
        public int Chunks { get; set; }

        /// <summary>
        /// The metadata configuration of the catalog.
        /// </summary>
        public MetadataConfig MetadataConfig { get; set; }
    }
}
