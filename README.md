# 🧠 RAG Assistant (.NET + Gemini + Qdrant)

A **domain-agnostic Retrieval-Augmented Generation (RAG) backend** built with **.NET**, **Google Gemini API**, and **Qdrant**.

This application allows you to:
- Ingest domain-specific knowledge (gaming, pharma, finance, etc.)
- Store it as vector embeddings
- Ask questions that are answered **only using the ingested data**

Think:  
> “ChatGPT, but it only knows what I feed it.”

---

## ✨ Features

- ✅ Multi-domain support (each domain has isolated knowledge)
- ✅ RAG architecture (hallucination-resistant by design)
- ✅ Google Gemini embeddings + LLM
- ✅ Qdrant vector database
- ✅ Swagger UI for easy testing
- ✅ Ready to plug into OpenWebUI or a custom frontend

---

## 🏗️ Architecture Overview

Client (Swagger / OpenWebUI)
        |
        v
.NET Web API
        |
        ├── Gemini Embeddings API
        ├── Qdrant Vector DB
        └── Gemini LLM API

### Request Flow

1. **Ingest**
   - Text → chunking → embeddings → Qdrant
2. **Chat**
   - Question → embedding → vector search
   - Retrieved context → Gemini LLM
   - Grounded answer returned

---

## 📦 Tech Stack

- **Backend**: .NET 6+ Web API
- **LLM & Embeddings**: Google Gemini API
- **Vector DB**: Qdrant
- **HTTP Client**: RestSharp
- **Serialization**: Newtonsoft.Json

---

## 🚀 Getting Started

### 1️⃣ Prerequisites

- .NET 6 or newer
- Docker
- Google Gemini API key

---

### 2️⃣ Run Qdrant

```bash
docker run -p 6333:6333 -p 6334:6334 qdrant/qdrant
```
🔑 Configure Gemini API Key

In GeminiService.cs, set your API key:
```bash
_gemini = new GeminiService("YOUR_GEMINI_API_KEY");
```

⚠️ Never commit your real API key to Git.
Use environment variables or secrets for production.

▶️ Run the API

Start the .NET Web API:
```bash
dotnet run
```

Open Swagger UI in your browser:
```bash
https://localhost:7046/swagger
```

Note: The port may differ depending on your environment.

🧪 Testing via Swagger
🔹 Ingest Knowledge

POST /api/ingest
```bash
{
  "domain": "pharma",
  "text": "Paracetamol is used for pain and fever. It can be combined with ibuprofen but should be avoided with alcohol. Patients with liver disease should use it cautiously.",
  "source": "Pharma Guide 2026"
}
```

Response:
```bash
{
  "chunksStored": 1
}
```
🔹 Ask Questions

POST /api/chat
```bash
{
  "domain": "pharma",
  "question": "Can paracetamol be taken with ibuprofen?"
}
```

Response:
```bash
{
  "answer": "Yes, paracetamol can be combined with ibuprofen, but it should be avoided with alcohol."
}
```

If the information is missing from the knowledge base, the assistant will reply:
```bash
I don't have enough data.
```
🧠 Prompting Rules

The assistant is instructed to:

Answer only using retrieved context

Never invent information

Explicitly state when data is missing

This makes the system suitable for sensitive domains such as pharma, legal, and finance.

🗂️ Multi-Domain Support

Each domain is isolated:
```bash
"domain": "gaming"
"domain": "pharma"
"domain": "finance"
```

Each domain maps to its own Qdrant collection.

🔒 Security Notes

* Authentication is not implemented

Do not expose this API publicly without:

* Authentication

* Rate limiting

* Per-client domain isolation

🛣️ Roadmap

* File ingestion (PDF, CSV, DOCX)

* Source citations in responses

* Multi-tenant support

* OpenWebUI integration

* Admin dashboard

* Relevance scoring and reranking
