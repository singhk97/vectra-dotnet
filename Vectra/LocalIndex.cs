
using System.Text.Json;
using System.Text.Json.Serialization;
using Vectra.Models;

namespace Vectra
{
    public class LocalIndex
    {
        private readonly string _folderPath;
        private readonly string _indexName;

        private IndexData<Metadata>? _data;
        private IndexData<Metadata>? _update;

        private static readonly JsonSerializerOptions _jsonSerializationOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public LocalIndex(string folderPath, string indexName = "index.json")
        {
            _folderPath = folderPath;
            _indexName = indexName;
        }

        public string FolderPath => _folderPath;

        public string IndexName => _indexName;

        /// <summary>
        /// Begins an update to the index.
        /// </summary>
        /// <remarks>
        /// This method loads the index into memory and prepares it for updates.
        /// </remarks>
        public async Task BeginUpdateAsync()
        {
            if (_update != null)
            {
                throw new InvalidOperationException("Update already in progress");
            }

            await LoadIndexDataAsync();
            _update = _data?.Clone();
        }

        /// <summary>
        /// Cancels an update to the index.
        /// </summary>
        /// <remarks>
        /// This method discards any changes made to the index since the update began.
        /// </remarks>
        public void CancelUpdate()
        {
            _update = null;
        }

        /// <summary>
        /// Creates a new index.
        /// </summary>
        /// <remarks>
        /// This method creates a new folder on disk containing an index.json file.
        /// </remarks>
        /// <param name="config">Index configuration</param>
        public async Task CreateIndexAsync(CreateIndexConfig? config = null)
        {
            // Delete if exists
            if (IsIndexCreated())
            {
                if (config?.DeleteIfExists ?? false)
                {
                    DeleteIndex();
                }
                else
                {
                    throw new InvalidOperationException("Index already exists");
                }
            }

            try
            {
                // Create folder for index
                Directory.CreateDirectory(_folderPath);

                // Initialize index.json file
                _data = new IndexData<Metadata>
                {
                    Version = config?.Version ?? 1,
                    MetadataConfig = config?.MetadataConfig ?? new MetadataConfig(),
                    Items = new List<IndexItem<Metadata>>()
                };

                await File.WriteAllTextAsync(Path.Combine(_folderPath, _indexName), _getIndexDataJson(_data));
            }
            catch (Exception)
            {
                DeleteIndex();
                throw new Exception("Error creating index");
            }
        }

        /// <summary>
        /// Deletes the index.
        /// </summary>
        /// <remarks>
        /// This method deletes the index folder from disk.
        /// </remarks>
        public void DeleteIndex()
        {
            _data = null;
            Directory.Delete(_folderPath, true);
        }

        /// <summary>
        /// Deletes an item from the index.
        /// </summary>
        /// <param name="id">Item id</param>
        public async Task DeleteItemAsync(string id)
        {
            if (_update != null)
            {
                var index = _update.Items.FindIndex(i => i.Id == id);
                if (index >= 0)
                {
                    _update.Items.RemoveAt(index);
                }
            }
            else
            {
                await BeginUpdateAsync();
                var index = _update!.Items.FindIndex(i => i.Id == id);
                if (index >= 0)
                {
                    _update!.Items.RemoveAt(index);
                }
                await EndUpdateAsync();
            }
        }

        /// <summary>
        /// Ends an update to the index.
        /// </summary>
        /// <remarks>
        /// This method saves the index to disk.
        /// </remarks>
        public async Task EndUpdateAsync()
        {
            if (_update == null)
            {
                throw new InvalidOperationException("No update in progress");
            }

            try
            {
                // Save index
                await File.WriteAllTextAsync(Path.Combine(_folderPath, _indexName), _getIndexDataJson(_update));
                _data = _update;
                _update = null;
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving index: {e.Message}");
            }
        }

        /// <summary>
        /// Loads an index from disk and returns its stats.
        /// </summary>
        /// <returns>Index stats</returns>
        public async Task<IndexStats> GetIndexStatsAsync()
        {
            await LoadIndexDataAsync();
            return new IndexStats
            {
                Version = _data!.Version,
                MetadataConfig = _data!.MetadataConfig,
                Items = _data!.Items.Count
            };
        }

        /// <summary>
        /// Returns an item from the index given its ID.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns>Item or null if not found</returns>
        public async Task<IndexItem<Metadata>?> GetItem(string id)
        {
            await LoadIndexDataAsync();
            return _data?.Items.Find(i => i.Id == id);
        }

        /// <summary>
        /// Adds an item to the index.
        /// </summary>
        /// <remarks>
        /// A new update is started if one is not already in progress. If an item with the same ID
        /// already exists, an exception will be thrown.
        /// </remarks>
        /// <param name="item">Item to insert</param>
        /// <returns>Inserted item</returns>
        public async Task<IndexItem<Metadata>> InsertItem(IndexItem<Metadata> item)
        {
            if (_update != null)
            {
                return await AddItemToUpdate(item, true);
            }
            else
            {
                await BeginUpdateAsync();
                var newItem = await AddItemToUpdate(item, true);
                await EndUpdateAsync();
                return newItem;
            }
        }

        /// <summary>
        /// Returns true if the index exists.
        /// </summary>
        public bool IsIndexCreated()
        {
            return File.Exists(Path.Combine(_folderPath, _indexName));
        }

        /// <summary>
        /// Returns all items in the index.
        /// </summary>
        /// <remarks>
        /// This method loads the index into memory and returns all its items. A copy of the items
        /// array is returned so no modifications should be made to the array.
        /// </remarks>
        /// <returns>All items in the index</returns>
        public async Task<List<IndexItem<Metadata>>?> ListItems()
        {
            await LoadIndexDataAsync();
            return _data?.Items.ToList();
        }

        /// <summary>
        /// Returns all items in the index matching the filter.
        /// </summary>
        /// <remarks>
        /// This method loads the index into memory and returns all its items matching the filter.
        /// </remarks>
        /// <param name="filter">Filter to apply</param>
        /// <returns>Items matching the filter</returns>
        public async Task<List<IndexItem<Metadata>>> ListItemsByMetadata(MetadataFilter filter)
        {
            await LoadIndexDataAsync();

            /// TODO: Implement the ItemSelector.Select method
            // return _data!.Items.Where(i => ItemSelector.Select(i.Metadata, filter)).ToArray();

            return new List<IndexItem<Metadata>>();
        }

        /// <summary>
        /// Finds the top k items in the index that are most similar to the vector.
        /// </summary>
        /// <remarks>
        /// This method loads the index into memory and returns the top k items that are most similar.
        /// An optional filter can be applied to the metadata of the items.
        /// </remarks>
        /// <param name="vector">Vector to query against.</param>
        /// <param name="topK">Number of items to return.</param>
        /// <param name="filter">Optional. Filter to apply.</param>
        /// <returns>Similar items to the vector that matches the filter.</returns>
        public async Task<List<QueryResult<Metadata>>> QueryItemsAsync(float[] vector, int topK, MetadataFilter? filter)
        {
            await LoadIndexDataAsync();

            // Filter items
            var items = _data!.Items;
            if (filter != null)
            {
                items = _data!.Items.Where(i => ItemSelector.Select(i.Metadata, filter)).ToList();
            }

            // Calculator distances
            var norm = ItemSelector.Normalize(vector);
            // (index, distance) pairs
            List<(int, double)> distances = new();
            for (int i = 0; i < _data!.Items.Count; i++)
            {
                var item = _data!.Items[i];
                var distance = ItemSelector.NormalizedCosineSimilarity(vector, norm, item.Vector, item.Norm);
                distances.Add((i, distance));
            }

            // Sort by distance DESCENDING
            distances.Sort((a, b) => b.Item2.CompareTo(a.Item2));

            // Find top k
            List<QueryResult<Metadata>> top = distances
                .GetRange(0, Math.Min(topK, distances.Count))
                .Select(d => new QueryResult<Metadata>
                    {
                        Item = _data!.Items[d.Item1].Clone(),
                        Score = d.Item2
                    }
                )
                .ToList();
            
            // Load external metadata
            foreach (var result in top)
            {
                if (result.Item.MetadataFile != null)
                {
                    var metadataPath = Path.Join(_folderPath, result.Item.MetadataFile);
                    var metadataJson = await File.ReadAllTextAsync(metadataPath);
                    result.Item.Metadata = JsonSerializer.Deserialize<Metadata>(metadataJson, _jsonSerializationOptions)!;
                }
            }

            return top;
        }

        /// <summary>
        /// Adds or replaces an item in the index.
        /// </summary>
        /// <remarks>
        /// A new update is started if one is not already in progress. If an item with the same ID
        /// already exists, it will be replaced.
        /// </remarks>
        /// <param name="item">Item to insert or replace</param>
        /// <returns>Upserted item</returns>
        public async Task<IndexItem<Metadata>> UpsertItem(IndexItem<Metadata> item)
        {
            if (_update != null)
            {
                return await AddItemToUpdate(item, false);
            }
            else
            {
                await BeginUpdateAsync();
                var newItem = await AddItemToUpdate(item, false);
                await EndUpdateAsync();
                return newItem;
            }
        }

        /// <summary>
        /// Ensure that the index has been loaded into memory.
        /// </summary>
        protected async Task LoadIndexDataAsync()
        {
            if (_data != null)
            {
                return;
            }

            if (IsIndexCreated() != true)
            {
                throw new Exception("Index does not exist");
            }

            try
            {
                var indexDataJson = await File.ReadAllTextAsync(Path.Combine(_folderPath, _indexName));
                _data = JsonSerializer.Deserialize<IndexData<Metadata>>(indexDataJson, _jsonSerializationOptions);
            }
            catch (Exception e)
            {
                throw new Exception($"Error loading index: {e.Message}");
            }
        }


        internal async Task<IndexItem<Metadata>> AddItemToUpdate(IndexItem<Metadata> item, bool unique)
        {
            if (item.Vector == null)
            {
                throw new Exception("Vector is required");
            }

            // Ensure unique
            string id = item.Id ?? Guid.NewGuid().ToString();
            if (unique)
            {
                var existing = _update!.Items.Find(i => i.Id == id);
                if (existing != null)
                {
                    throw new Exception($"Item with id {id} already exists");
                }
            }

            // Check for indexed metadata
            Metadata metadata = new();
            string? metadataFile = null;

            if (_update!.MetadataConfig.Indexed != null && _update!.MetadataConfig.Indexed.Length > 0 && item.Metadata.Values.Count > 0)
            {
                // Copy only indexed metadata
                foreach (var key in _update!.MetadataConfig.Indexed)
                {
                    if (item.Metadata.TryGetValue(key, out var value))
                    {
                        metadata[key] = value;
                    }
                }

                // Save metadata to file
                metadataFile = Path.Combine(_folderPath, $"{id}.json");
                await File.WriteAllTextAsync(metadataFile, JsonSerializer.Serialize(metadata, _jsonSerializationOptions));
            } 
            else if (item.Metadata.Count > 0)
            {
                metadata = item.Metadata;
            }

            // Create new item
            var newItem = new IndexItem<Metadata>
            {
                Id = id,
                Vector = item.Vector,
                Metadata = metadata,
                Norm = ItemSelector.Normalize(item.Vector)
            };

            if (metadataFile != null)
            {
                newItem.MetadataFile = metadataFile;
            } 

            // Add item to index
            if (!unique)
            {
                var existing = _update!.Items.Find(i => i.Id == id);
                if (existing != null)
                {
                    existing.Metadata = newItem.Metadata;
                    existing.Vector = newItem.Vector;
                    existing.MetadataFile = newItem.MetadataFile;
                    return existing;
                }
                else
                {
                    _update!.Items.Add(newItem);
                    return newItem;
                }
            } else
            {
                _update!.Items.Add(newItem);
                return newItem;
            }
        }

        private string _getIndexDataJson(IndexData<Metadata> indexData)
        {
            return JsonSerializer.Serialize(indexData, _jsonSerializationOptions);
        }
    }
}
