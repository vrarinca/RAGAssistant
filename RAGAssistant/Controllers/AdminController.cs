using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("admin/{domain}/ingest")]
[Authorize] // only logged-in admins
public class AdminController : ControllerBase
{
    private readonly VectorDbService _vectorDb;
    private readonly GeminiService _gemini;

    public AdminController(VectorDbService vectorDb, GeminiService gemini)
    {
        _vectorDb = vectorDb;
        _gemini = gemini;
    }

    [HttpPost("text")]
    public async Task<IActionResult> AddText([FromBody] IngestRequest request)
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

    [HttpPost("file")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        var parts = fileName.Split('_');

        if (parts.Length < 2)
            return BadRequest("Filename must be domain_source.ext");

        var domain = parts[0];
        var source = string.Join("_", parts.Skip(1)) + Path.GetExtension(file.FileName);

        string text;

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            text = await reader.ReadToEndAsync();
        }

        var chunks = SplitText(text, 500);
        var rng = new Random();


        foreach (var chunk in chunks)
        {
            var vector = await _gemini.GetEmbeddingAsync(chunk);

            var payload = new Dictionary<string, object>
        {
            { "text", chunk },
            { "source", source },
            { "created_at", DateTime.UtcNow.ToString("o") }
        };

            ulong id = (ulong)rng.NextInt64(1, long.MaxValue);
            await _vectorDb.UpsertAsync(domain, id, vector, payload);
        }

        return Ok(new
        {
            domain,
            source,
            chunksStored = chunks.Count
        });
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

}
