using Microsoft.Extensions.Configuration;
using System.Reflection;
using Vectra;
using Vectra.Models;

var configuration = new ConfigurationBuilder().AddJsonFile(getAbsolutePath("appsettings.json")).Build();

var openAIKey = configuration["OpenAIKey"];

ArgumentNullException.ThrowIfNull(openAIKey);

var embeddingApi = new OpenAIEmbedding(openAIKey, "text-embedding-ada-002");

// Create local index
var index = new LocalIndex(getAbsolutePath("index"));

await chat(string.Join("\n", new string[] {
    "Vectra sample usage:",
    "-add <text> will insert a new item into the index.",
    "-delete will delete the index and start over.",
    "-exit will exit the program.",
    "Otherwise, type a question to query the index." 
}));


async Task chat(string? botMessage = null)
{
    var getVector = async (string text) =>
    {
        var response = await embeddingApi.CreateEmbeddings(new() { text });

        if (response.Status != EmbeddingsResponseStatus.Success)
        {
            throw new InvalidOperationException("Unable to create embeddings.");
        }

        return response.Output[0];
    };

    if (botMessage != null)
    {
        Console.WriteLine($"{botMessage}");
    }

    string input = Console.ReadLine();
    if (input == string.Empty)
    {
        await chat();
        return;
    }

    if (!index.IsIndexCreated())
    {
        await index.CreateIndexAsync();
    }

    if (input.StartsWith("-exit"))
    {
        return;
    } 
    else if (input.StartsWith("-add"))
    {
        // Get the text to add
        var text = input.Split("-add")[1].Trim();
        var vector = await getVector(text);

        // Add the text to the index
        await index.InsertItem(new()
        {
            Vector = vector,
            Metadata = new Metadata(new() {
                { "text" , text }
            })
        });

        await chat("Added to the index");
    }
    else if (input.StartsWith("-delete"))
    {
        // Delete the index
        index.DeleteIndex();
        await chat("Deleted the index");
    } else
    {
        // Query the index
        var vector = await getVector(input);
        var results = await index.QueryItemsAsync(vector, 3, null);
        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                Console.WriteLine($"[{result.Score}] {result.Item.Metadata["text"]}");
            };

            await chat();
        } else
        {
            await chat("No results found");
        };
    }
}

string getAbsolutePath(string relativePath)
{
    var currentAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    if (string.IsNullOrWhiteSpace(currentAssemblyDirectory))
    {
        throw new InvalidOperationException("Unable to determine current assembly directory.");
    }

    return Path.GetFullPath(Path.Combine(currentAssemblyDirectory, $"../../../{relativePath}"));
};