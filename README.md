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

