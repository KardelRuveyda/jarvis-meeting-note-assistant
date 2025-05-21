using AIComparisonConsole.Services;
using Microsoft.Extensions.Configuration;

// 1️⃣ Konfigürasyonu yükle
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) // appsettings.json bulunduğu klasör
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var service = new OpenAIService(configuration);
var kernel = await service.InitializeKernelAsync();

while (true)
{
    Console.Write("Soru: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    if (input.ToLower().Contains("süre") || input.ToLower().Contains("ne kadar sürdü"))
    {
        var result = await kernel.InvokeAsync("TranscriptPlugin", "GetMeetingDuration");
        Console.WriteLine($"\nYanıt:\n• {result.GetValue<string>()}\n");
        continue;
    }

    var response = await service.GetChatResponseAsync(kernel, input);

    Console.WriteLine("\nYanıt:");
    foreach (var line in response.Split(['.', '!', '?']))
    {
        var trimmed = line.Trim();
        if (!string.IsNullOrWhiteSpace(trimmed))
            Console.WriteLine($"• {trimmed}.");
    }

    Console.WriteLine(); // boşluk bırak
}
