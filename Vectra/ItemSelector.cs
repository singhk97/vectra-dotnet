using Vectra.Models;

namespace Vectra
{
    public class ItemSelector
    {
        /// <summary>
        /// Returns the similarity between two vectors using the cosine similarity.
        /// </summary>
        /// <param name="vector1">Vector 1</param>
        /// <param name="vector2">Vector 2</param>
        /// <returns>Similarity between the two vectors</returns>
        public static float CosineSimilarity(float[] vector1, float[] vector2)
        {
            // Return the quotient of the dot product and the product of the norms
            return DotProduct(vector1, vector2) / (Normalize(vector1) * Normalize(vector2));
        }

        /// <summary>
        /// Normalizes a vector.
        /// </summary>
        /// <remarks>
        /// The norm of a vector is the square root of the sum of the squares of the elements.
        /// The LocalIndex pre-normalizes all vectors to improve performance.
        /// </remarks>
        /// <param name="vector">Vector to normalize</param>
        /// <returns>Normalized vector</returns>
        public static float Normalize(float[] vector)
        {
            // Initialize a variable to store the sum of the squares
            float sum = 0;
            // Loop through the elements of the array
            for (int i = 0; i < vector.Length; i++)
            {
                // Square the element and add it to the sum
                sum += vector[i] * vector[i];
            }
            // Return the square root of the sum
            return (float)Math.Sqrt(sum);
        }

        /// <summary>
        /// Returns the similarity between two vectors using cosine similarity.
        /// </summary>
        /// <remarks>
        /// The LocalIndex pre-normalizes all vectors to improve performance.
        /// This method uses the pre-calculated norms to improve performance.
        /// </remarks>
        /// <param name="vector1">Vector 1</param>
        /// <param name="norm1">Norm of vector 1</param>
        /// <param name="vector2">Vector 2</param>
        /// <param name="norm2">Norm of vector 2</param>
        /// <returns>Similarity between the two vectors</returns>
        public static float NormalizedCosineSimilarity(float[] vector1, float norm1, float[] vector2, float norm2)
        {
            // Return the quotient of the dot product and the product of the norms
            return DotProduct(vector1, vector2) / (norm1 * norm2);
        }

        /// <summary>
        /// Applies a filter to the metadata of an item.
        /// </summary>
        /// <param name="metadata">Metadata of the item</param>
        /// <param name="filter">Filter to apply</param>
        /// <returns>True if the item matches the filter, false otherwise</returns>
        public static bool Select(Dictionary<string, object> metadata, MetadataFilter filter)
        {
            if (filter == null)
            {
                return true;
            }

/*            foreach (var key in filter.Custom?.Keys ?? Enumerable.Empty<string>())
            {
                switch (key)
                {
                    case "$and":
                        if (!filter.And!.All(f => Select(metadata, f)))
                        {
                            return false;
                        }
                        break;
                    case "$or":
                        if (!filter.Or!.Any(f => Select(metadata, f)))
                        {
                            return false;
                        }
                        break;
                    default:
                        var value = filter.Custom![key];
                        if (value == null)
                        {
                            return false;
                        }
                        else if (value is MetadataFilter)
                        {
                            if (!MetadataFilter(metadata[key], value as MetadataFilter))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (metadata[key] != (MetadataTypes)value)
                            {
                                return false;
                            }
                        }
                        break;
                }
            }*/

            return true;
        }

        private static float DotProduct(float[] arr1, float[] arr2)
        {
            // Initialize a variable to store the sum of the products
            float sum = 0;
            // Loop through the elements of the arrays
            for (int i = 0; i < arr1.Length; i++)
            {
                // Multiply the corresponding elements and add them to the sum
                sum += arr1[i] * arr2[i];
            }
            // Return the sum
            return sum;
        }

        private static bool MetadataFilter(MetadataTypes value, MetadataFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(value);

            foreach (var key in filter.Custom?.Keys ?? Enumerable.Empty<string>())
            {
                switch (key)
                {
                    case "$eq":
                        if (value != filter.Eq)
                        {
                            return false;
                        }
                        break;
                    case "$ne":
                        if (value == filter.Ne)
                        {
                            return false;
                        }
                        break;
                    case "$gt":
                        if (value != MetadataTypes.Number || (float)value <= filter.Gt!)
                        {
                            return false;
                        }
                        break;
                    case "$gte":
                        if (value != MetadataTypes.Number || (float)value < filter.Gte!)
                        {
                            return false;
                        }
                        break;
                    case "$lt":
                        if (value != MetadataTypes.Number || (float)value >= filter.Lt!)
                        {
                            return false;
                        }
                        break;
                    case "$lte":
                        if (value != MetadataTypes.Number || (float)value > filter.Lte!)
                        {
                            return false;
                        }
                        break;
                    case "$in":
                        if (value == MetadataTypes.Boolean || !filter.In!.Contains(value))
                        {
                            return false;
                        }
                        break;
                    case "$nin":
                        if (value == MetadataTypes.Boolean || filter.Nin!.Contains(value))
                        {
                            return false;
                        }
                        break;
                    default:
                        return value == (MetadataTypes)filter.Custom![key];
                }
            }
            return true;
        }
    }
}
