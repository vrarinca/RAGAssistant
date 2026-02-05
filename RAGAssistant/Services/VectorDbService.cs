using Qdrant.Client;
using Qdrant.Client.Grpc; // All models now live here

public class VectorDbService
{
    private readonly QdrantClient _client;

    public VectorDbService(IConfiguration config)
    {
        var host = config["Qdrant:Host"];
        var port = int.Parse(config["Qdrant:GrpcPort"]);

        _client = new QdrantClient(host, port);
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