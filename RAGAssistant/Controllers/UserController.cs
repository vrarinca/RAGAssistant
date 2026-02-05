using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("{domain}/questions")]
public class UserController : ControllerBase
{
    private readonly VectorDbService _vectorDb;
    private readonly GeminiService _gemini;

    public UserController(VectorDbService vectorDb, GeminiService gemini)
    {
        _vectorDb = vectorDb;
        _gemini = gemini;
    }

    [HttpPost]
    public async Task<IActionResult> AskQuestion([FromBody] ChatRequest request)
    {
        var embedding = await _gemini.GetEmbeddingAsync(request.Question);
        var chunks = await _vectorDb.SearchAsync(request.Domain, embedding, topK: 5);
        var context = string.Join("\n", chunks.Select(c => c["text"].ToString()));

        var prompt = $@"
            You are a {request.Domain} assistant.
            Answer ONLY using the context below.
            If the answer is not in the context, say 'I don't have enough data.'

            Context:
            {context}

            Question:
            {request.Question}";

        var answer = await _gemini.GetLlmAnswerAsync(prompt);
        return Ok(new { answer });
    }
}
