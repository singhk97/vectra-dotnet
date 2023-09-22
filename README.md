# vectra-dotnet

> Implementation is in progress.

This is a port of Steve Ickman's local in-memory vector database project Vectra. It maintains similar functionality, with adjustments made to with the .NET conventions. 

vectra-dotnet is a local vector database for .NET with features similar to Pinecone or Qdrant but built using local files. Each Vectra index is a folder on disk. There's an `index.json` file in the folder that contains all the vectors for the index along with any indexed metadata. When you create an index you can specify which metadata properties to index and only those fields will be stored in the `index.json` file. All of the other metadata for an item will be stored on disk in a separate file keyed by a GUID.

When queryng Vectra you'll be able to use the same subset of Mongo DB query operators that Pinecone supports and the results will be returned sorted by similarity. Every item in the index will first be filtered by metadata and then ranked for similarity. Even though every item is evaluated its all in memory so it should by nearly instantanious. Likely 1ms - 2ms for even a rather large index. Smaller indexes should be < 1ms.

Keep in mind that your entire Vectra index is loaded into memory so it's not well suited for scenarios like long term chat bot memory. Use a real vector DB for that. Vectra is intended to be used in scenarios where you have a small corpus of mostly static data that you'd like to include in your prompt. Infinite few shot examples would be a great use case for Vectra or even just a single document you want to ask questions over.

Pinecone style namespaces aren't directly supported but you could easily mimic them by creating a separate Vectra index (and folder) for each namespace.

