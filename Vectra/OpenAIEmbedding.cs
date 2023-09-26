using Azure;
using Azure.AI.OpenAI;
using System.Net;
using Vectra.Models;

namespace Vectra
{
    public class OpenAIEmbedding : IEmbeddingsModel
    {
        private bool _useAzureOpenAI;
        private OpenAIClient _client;
        private string _modelName;

        public int MaxTokens { get; } = 8000;

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
