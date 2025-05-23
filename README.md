# Jarvis Meeting Note Assistant




Jarvis is a .NET 9 and C# 13.0-based console application integrated with the OpenAI API. It takes user questions in natural language and responds with intelligent, context-aware, and user-friendly answers based on meeting transcripts. The app is ideal for meeting summarization, decision tracking, and contextual Q&A tasks.

---

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Requirements](#requirements)
- [Contributing](#contributing)
- [License](#license)
- [Notes](#notes)

---

## Features

- ✅ Console-based interaction: Real-time Q&A via CLI
- 🧠 OpenAI GPT integration: GPT-4o or similar models
- 📄 Memory-based RAG: Contextual answers from embedded transcript chunks
- 🔧 Easy configuration: Control API key and model via `appsettings.json`
- 📁 File-based testing: Use `Data/sample_transcript.txt` for testing
- 🧩 Modular architecture: Extensible service structure via `OpenAIService`

---

## Installation

1. **Clone the Repository**

```bash
git clone https://github.com/yourusername/MeetingSummaryApp.Console.git
cd MeetingSummaryApp.Console
```

2. **Restore Dependencies**

```bash
dotnet restore
```

3. **Configure API Key**

Edit `appsettings.json`:

```json
{
  "LLMProviders": {
    "OpenAI": {
      "ApiKey": "YOUR_OPENAI_API_KEY",
      "Model": "gpt-4o"
    }
  }
}
```
> ⚠️ Do not share your API key publicly.

4. **Build and Run**

```bash
dotnet run --project MeetingSummaryApp.Console
```

---

## Configuration

- Set your OpenAI API key and preferred model in `appsettings.json`
- Supported models include `gpt-4o`, `gpt-3.5-turbo`, etc.

---

## Usage

Upon launch, you will see a `Soru:` prompt. Enter your question and press Enter. The app uses Semantic Kernel and OpenAI to extract relevant transcript content and generate a response. To exit, just press Enter on an empty line.

### Example Session

```
Soru: What decisions were made during the meeting?
Yanıt:
• The logistics partner will be warned.
• GPT-powered support automation will be launched.
• Scaling will be applied to the order-confirmation service.
```

---

## Project Structure

```
MeetingSummaryApp.Console/
├── appsettings.json             # API key and model config
├── Data/
│   └── sample_transcript.txt    # Sample transcript file
├── Program.cs                   # Entry point
├── Services/
│   └── OpenAIService.cs         # Core logic and LLM interaction
└── Plugins/
    └── TranscriptPlugin.cs      # Custom SK plugin for transcript analysis
```

---

## Requirements

- .NET 9 SDK
- A valid OpenAI API key
- Internet connection

---

## Contributing

Feel free to fork this repo, create a feature branch, and submit a pull request. Contributions via Issues and PRs are highly welcome!

---

## License

This project is licensed under the MIT License.

---

## Notes

> ⚠️ This application uses the OpenAI API, which may incur costs. You are responsible for any charges associated with your API usage.
