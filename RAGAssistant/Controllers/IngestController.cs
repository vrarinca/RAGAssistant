using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class IngestController : ControllerBase
{
    private readonly VectorDbService _vectorDb;
    private readonly GeminiService _gemini;

    public IngestController()
    {
        _vectorDb = new VectorDbService();
        _gemini = new GeminiService("API-KEY");
    }

    [HttpPost]
    public async Task<IActionResult> Ingest([FromBody] IngestRequest request)
    {
        var chunks = SplitText(request.Text, 500);

        var rng = new Random();

        foreach (var chunk in chunks)
        {
            var vector = await _gemini.GetEmbeddingAsync(chunk);

            var payload = new Dictionary<string, object>
        {
            { "text", chunk },
            { "source", request.Source ?? "unknown" },
            { "created_at", DateTime.UtcNow.ToString("o") }
        };

            ulong id = (ulong)rng.NextInt64(1, long.MaxValue);

            await _vectorDb.UpsertAsync(request.Domain, id, vector, payload);
        }

        return Ok(new { chunksStored = chunks.Count });
    }

    private List<string> SplitText(string text, int chunkSize)
    {
        var words = text.Split(' ');
        var chunks = new List<string>();
        for (int i = 0; i < words.Length; i += chunkSize)
        {
            chunks.Add(string.Join(" ", words.Skip(i).Take(chunkSize)));
        }
        return chunks;
    }

    private async Task<float[]> GetEmbedding(string text)
    {
        return await _gemini.GetEmbeddingAsync(text);
    }
}

public class IngestRequest
{
    public string Domain { get; set; }
    public string Text { get; set; }
    public string Source { get; set; }
}
