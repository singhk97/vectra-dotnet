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
        public static bool Select(Metadata metadata, MetadataFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(metadata);

            return filter(metadata).Result;
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
    }
}
