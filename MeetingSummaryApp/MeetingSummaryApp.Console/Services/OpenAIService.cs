#pragma warning disable SKEXP0010

using MeetingSummaryApp.Console.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Text;

namespace AIComparisonConsole.Services;

public class OpenAIService
{
    private readonly IConfiguration _configuration;
    private ChatHistory _history = new();

    public OpenAIService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Kernel> InitializeKernelAsync()
    {
        var builder = Kernel.CreateBuilder();
        builder.Services.AddHttpClient();

        var model = _configuration["LLMProviders:OpenAI:Model"] ?? "gpt-4o";
        var apiKey = _configuration["LLMProviders:OpenAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is not configured. Please check your configuration.");

        builder.AddOpenAIChatCompletion(model, apiKey);
        builder.Services.AddSingleton<ITextEmbeddingGenerationService>(new OpenAITextEmbeddingGenerationService("text-embedding-ada-002", apiKey));

        var kernel = builder.Build();
        kernel.Plugins.AddFromType<TranscriptPlugin>();

        var systemPrompt = "Sen bir toplantı asistanısın. Aşağıdaki toplantı içeriğine göre kullanıcının sorusunu yanıtla.";
        _history = new ChatHistory();
        _history.AddSystemMessage(systemPrompt);

        return kernel;
    }

    public async Task<string> GetChatResponseAsync(Kernel kernel, string input)
    {
        // 📂 Transcript dosyasını oku ve böl
        var transcriptPath = Path.Combine(AppContext.BaseDirectory, "Data", "sample_transcript.txt");
        var transcript = await File.ReadAllTextAsync(transcriptPath);
#pragma warning disable SKEXP0050
        var chunked = TextChunker.SplitPlainTextLines(transcript, 500);
#pragma warning restore SKEXP0050

        // 🧠 Embed chunks in memory
        var embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
        var memory = new List<(string chunk, ReadOnlyMemory<float> vector)>();

        foreach (var chunk in chunked)
        {
            var vector = await embeddingService.GenerateEmbeddingAsync(chunk);
            memory.Add((chunk, vector));
        }

        // 🔍 Embed user query & find best chunk
        var queryEmbedding = await embeddingService.GenerateEmbeddingAsync(input);
        var topChunk = memory
            .OrderByDescending(e => CosineSimilarity(e.vector.Span, queryEmbedding.Span))
            .Select(e => e.chunk)
            .FirstOrDefault() ?? "[Uygun içerik bulunamadı]";

        _history.AddUserMessage($"{input}\n\n---\n{topChunk}");

        // 🧠 LLM'e gönder
        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var settings = new OpenAIPromptExecutionSettings { Temperature = 0.5 };

        var response = await chatService.GetChatMessageContentAsync(_history, settings, kernel);
        _history.AddAssistantMessage(response.Content);

        return response?.Content ?? "[Yanıt yok]";
    }

    private float CosineSimilarity(ReadOnlySpan<float> v1, ReadOnlySpan<float> v2)
    {
        float dot = 0, mag1 = 0, mag2 = 0;
        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
            mag1 += v1[i] * v1[i];
            mag2 += v2[i] * v2[i];
        }
        return dot / ((float)Math.Sqrt(mag1) * (float)Math.Sqrt(mag2));
    }
}

#pragma warning restore SKEXP0010