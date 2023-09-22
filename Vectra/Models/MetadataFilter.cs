using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectra.Models
{
    /// <summary>
    /// A filter for metadata.
    /// </summary>
    public class MetadataFilter
    {

        /// <summary>
        /// Equal to (number, string, boolean).
        /// </summary>
        public MetadataTypes? Eq { get; set; }

        /// <summary>
        /// Not equal to (number, string, boolean).
        /// </summary>
        public MetadataTypes? Ne { get; set; }

        /// <summary>
        /// Greater than (number).
        /// </summary>
        public double? Gt { get; set; }

        /// <summary>
        /// Greater than or equal to (number).
        /// </summary>
        public double? Gte { get; set; }

        /// <summary>
        /// Less than (number).
        /// </summary>
        public double? Lt { get; set; }

        /// <summary>
        /// Less than or equal to (number).
        /// </summary>
        public double? Lte { get; set; }

        /// <summary>
        /// In array (string or number).
        /// </summary>
        public MetadataTypes[]? In { get; set; }

        /// <summary>
        /// Not in array (string or number).
        /// </summary>
        public MetadataTypes[]? Nin { get; set; }

        /// <summary>
        /// AND (MetadataFilter[]).
        /// </summary>
        public MetadataFilter[]? And { get; set; }

        /// <summary>
        /// OR (MetadataFilter[]).
        /// </summary>
        public MetadataFilter[]? Or { get; set; }

        /// <summary>
        /// Any other custom filter.
        /// </summary>
        public Dictionary<string, object>? Custom { get; set; }
    }

    /// <summary>
    /// The types of metadata values.
    /// </summary>
    public enum MetadataTypes
    {
        Number,
        String,
        Boolean
    }

}
