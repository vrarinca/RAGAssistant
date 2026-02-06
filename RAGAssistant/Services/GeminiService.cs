using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class GeminiService
{
    private readonly string _apiKey;
    private readonly RestClient _client;

    public GeminiService(IConfiguration config)
    {
        _apiKey = config["Gemini:ApiKey"];
        _client = new RestClient("https://generativelanguage.googleapis.com/v1beta");
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var request = new RestRequest("models/gemini-embedding-001:embedContent", Method.Post);

        request.AddQueryParameter("key", _apiKey);

        var body = new
        {
            model = "models/gemini-embedding-001",
            content = new { parts = new[] { new { text = text } } },
            // 2. Add task_type (RETRIEVAL_DOCUMENT for indexing, RETRIEVAL_QUERY for searching)
            task_type = "RETRIEVAL_DOCUMENT",
            outputDimensionality = 768
        };
        request.AddJsonBody(body);

        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful)
            throw new Exception($"Gemini embedding error: {response.Content}");

        dynamic result = JsonConvert.DeserializeObject(response.Content);

        var values = result.embedding.values;
        return ((IEnumerable<dynamic>)values).Select(v => (float)v).ToArray();
    }

    public async Task<string> GetLlmAnswerAsync(string prompt)
    {
        var request = new RestRequest("models/gemini-3-flash-preview:generateContent", Method.Post);
        request.AddQueryParameter("key", _apiKey);

        var body = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                temperature = 0.1,
                maxOutputTokens = 1024
            }
        };
        request.AddJsonBody(body);

        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful)
            throw new Exception($"Gemini LLM error: {response.Content}");

        dynamic result = JsonConvert.DeserializeObject(response.Content);

        string answer = result.candidates[0].content.parts[0].text;
        return answer;
    }
}