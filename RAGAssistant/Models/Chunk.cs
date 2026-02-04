namespace RAGAssistant.Models
{
    public class Chunk
    {
        public string Id { get; set; }           // Unique ID
        public string Text { get; set; }         // Text chunk
        public string Domain { get; set; }       // e.g., "gaming" or "pharma"
        public string Source { get; set; }       // Optional metadata
        public DateTime CreatedAt { get; set; }
    }
}
