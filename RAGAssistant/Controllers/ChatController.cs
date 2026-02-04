using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly VectorDbService _vectorDb;
    private readonly GeminiService _gemini;

    public ChatController()
    {
        _vectorDb = new VectorDbService();
        _gemini = new GeminiService("API-KEY");
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {

        var questionEmbedding = await GetEmbedding(request.Question);

        var contextChunks = await _vectorDb.SearchAsync(request.Domain, questionEmbedding, topK: 5);

        var contextText = string.Join("\n", contextChunks.Select(c => c["text"].ToString()));

        var prompt = $@"
            You are a {request.Domain} assistant.
            Answer ONLY using the context below.
            If the answer is not in the context, say 'I don't have enough data.'

            Context:
            {contextText}

            Question:
            {request.Question}";

        var answer = await GetLlmAnswer(prompt);

        return Ok(new { answer });
    }

    private async Task<float[]> GetEmbedding(string text)
    {
        return await _gemini.GetEmbeddingAsync(text);
    }

    private async Task<string> GetLlmAnswer(string prompt)
    {
        return await _gemini.GetLlmAnswerAsync(prompt);
    }
}

public class ChatRequest
{
    public string Domain { get; set; }
    public string Question { get; set; }
}
