using Qdrant.Client;
using Qdrant.Client.Grpc; // All models now live here

public class VectorDbService
{
    private readonly QdrantClient _client;

    public VectorDbService()
    {
        // For local Qdrant, the gRPC port is 6334 (HTTP is 6333)
        // The .NET SDK uses gRPC for better performance.
        _client = new QdrantClient("localhost", 6334);
    }

    public async Task UpsertAsync(string collectionName, ulong id, float[] vector, Dictionary<string, object> payload)
    {
        var collections = await _client.ListCollectionsAsync();

        if (!collections.Contains(collectionName))
        {
            await _client.CreateCollectionAsync(collectionName, new VectorParams
            {
                Size = (ulong)vector.Length,
                Distance = Distance.Cosine
            });
        }

        var point = new PointStruct
        {
            Id = id,
            Vectors = vector
        };

        foreach (var kvp in payload)
        {
            point.Payload[kvp.Key] = kvp.Value?.ToString() ?? string.Empty;
        }

        await _client.UpsertAsync(collectionName, new List<PointStruct> { point });
    }

    public async Task<List<Dictionary<string, object>>> SearchAsync(string collectionName, float[] vector, int topK = 5)
    {
        var results = await _client.SearchAsync(collectionName, vector, limit: (ulong)topK);

        return results.Select(r => r.Payload.ToDictionary(k => k.Key, v => (object)v.Value)).ToList();
    }
}