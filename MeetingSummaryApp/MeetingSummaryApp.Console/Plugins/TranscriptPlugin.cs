using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MeetingSummaryApp.Console.Plugins;

public class TranscriptPlugin
{
    [KernelFunction, Description("Toplantı süresini transkript dosyasına göre dakika cinsinden hesaplar.")]
    public string GetMeetingDuration()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "sample_transcript.txt");
        if (!File.Exists(path))
        {
            return "Transkript dosyası bulunamadı.";
        }

        var lines = File.ReadAllLines(path);
        var timePattern = new Regex(@"^\[(\d{2}:\d{2})\]");

        var times = lines
            .Select(line => timePattern.Match(line))
            .Where(m => m.Success)
            .Select(m => DateTime.ParseExact(m.Groups[1].Value, "HH:mm", CultureInfo.InvariantCulture))
            .ToList();

        if (times.Count < 2)
            return "Toplantı süresi hesaplanamıyor. Yeterli zaman damgası yok.";

        var duration = times.Max() - times.Min();
        return $"Toplantı süresi yaklaşık {duration.TotalMinutes:F0} dakika.";
    }
}
