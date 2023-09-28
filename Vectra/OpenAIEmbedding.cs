using Azure;
using Azure.AI.OpenAI;
using System.Net;
using Vectra.Models;

namespace Vectra
{
    public class OpenAIEmbedding : IEmbeddingsModel
    {
        private readonly bool _useAzureOpenAI;
        private readonly OpenAIClient _client;
        private readonly string _modelName;

        /// <summary>
        /// The maximum number of tokens that can be passed to the model.
        /// </summary>
        public int MaxTokens { get; } = 8000;

        /// <summary>
        /// The maximum number of tokens that can be passed to the model.
        /// </summary>
        /// <param name="apiKey">The API key to use.</param>
        /// <param name="modelName">The name of the model to use.</param>
        /// <param name="endpoint">The endpoint to use.</param>
        /// <param name="useAzureOpenAI">Whether to use the Azure OpenAI API or not.</param>
        public OpenAIEmbedding(string apiKey, string modelName, string? endpoint = null, bool useAzureOpenAI = false)
        {
            _useAzureOpenAI = useAzureOpenAI;
            _modelName = modelName;

            if (_useAzureOpenAI)
            {
                ArgumentNullException.ThrowIfNull(endpoint);

                _client = new OpenAIClient(
                    new Uri(endpoint),
                    new AzureKeyCredential(apiKey)
                );
            }
            else
            {
                _client = new OpenAIClient(apiKey);
            }
        }

        /// <summary>
        /// Creates embeddings for a list of inputs.
        /// </summary>
        /// <param name="inputs">The inputs to create embeddings for.</param>
        /// <returns>The embeddings.</returns>
        public async Task<EmbeddingsResponse> CreateEmbeddings(List<string> inputs)
        {
            Response<Embeddings> response = await _client.GetEmbeddingsAsync(_modelName, new EmbeddingsOptions(inputs));

            HttpStatusCode status = (HttpStatusCode)response.GetRawResponse().Status;

            switch (status)
            {
                case HttpStatusCode.OK:
                    List<float[]> output = response.Value.Data.Select(e => e.Embedding.ToArray()).ToList();

                    return new EmbeddingsResponse
                    {
                        Status = EmbeddingsResponseStatus.Success,
                        Output = output
                    };
                case HttpStatusCode.TooManyRequests:
                    return new EmbeddingsResponse
                    {
                        Status = EmbeddingsResponseStatus.RateLimited,
                        Message = response.GetRawResponse().ReasonPhrase
                    };
                default:
                    return new EmbeddingsResponse
                    {
                        Status = EmbeddingsResponseStatus.Error,
                        Message = response.GetRawResponse().ReasonPhrase
                    };
            }
        }
    }
}
