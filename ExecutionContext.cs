using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdventOfCode;

public static class ExecutionContext
{
    private const int UnparsedNumber = 0;
    private const string DaysArgPrefix = "days=";
    private const string YearsArgPrefix = "years=";
    private const string SettingsFileName = "settings.local.json";

    private static readonly IReadOnlySet<int> Days;
    private static readonly IReadOnlySet<int> Years;
    private static readonly string InputCacheFolder = ".\\data";
    private static readonly string SessionCookie;
    
    static ExecutionContext()
    {
        var args = Environment.GetCommandLineArgs();
        Mode = args.All(a => !StringComparer.OrdinalIgnoreCase.Equals(a.Trim('-', '/'), "debug"))
            ? ExecutionMode.Default
            : ExecutionMode.Debug;
        
        Days = ReadNumbers(args, DaysArgPrefix);
        Years = ReadNumbers(args, YearsArgPrefix);

        if (File.Exists(SettingsFileName))
        {
            var settingsJson = File.ReadAllText(SettingsFileName);
            var settings = JsonSerializer.Deserialize<JsonElement>(settingsJson);
            if (settings.TryGetProperty("SessionCookie", out var cookie))
            {
                SessionCookie = cookie.GetString();
            }

            if (settings.TryGetProperty("InputCacheFolder", out var folder))
            {
                InputCacheFolder = folder.GetString();
            }
        }
    }

    private static HashSet<int> ReadNumbers(string[] args, string prefix)
    {
        var arg = args.FirstOrDefault(a => a.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        var value = arg?[prefix.Length..];
        return value?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out var d) ? d : UnparsedNumber)
            .Where(d => d != UnparsedNumber)
            .ToHashSet();
    }

    public static ExecutionMode Mode { get; }
    
    public static bool YearIsSelected(int year) => Years == null || Years.Count == 0 || Years.Contains(year);
    
    public static bool DayIsSelected(int day) => Days == null || Days.Count == 0 || Days.Contains(day);
    
    public static T LoadInput<T>(Day day, Func<string, T> factory)
    {
        var dayOfYear = new DayOfYear(day.GetType());
        
        var path = Path.Combine(InputCacheFolder, $"{dayOfYear.Year}\\Day{dayOfYear.DayOfMonth}.txt");
        if (!File.Exists(path))
        {
            if (string.IsNullOrEmpty(SessionCookie))
            {
                throw new InvalidCredentialException("No session cookie provided for download.");
            }

            var data = DownloadFile(dayOfYear.Year, dayOfYear.DayOfMonth).Result;
            File.WriteAllBytes(path, data);
        }

        return factory(path);
    }

    private static async Task<byte[]> DownloadFile(int year, int day)
    {
        var url = new Uri("https://adventofcode.com");
        
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;
        using var client = new HttpClient(handler);
        client.BaseAddress = url;
        cookieContainer.Add(url, new Cookie("session", SessionCookie));
        return await client.GetByteArrayAsync($"/{year}/day/{day}/input");
    }
}