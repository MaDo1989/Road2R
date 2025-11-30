using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;

public class AIModel
{
    private readonly string apiKey;
    private readonly string model;
    private readonly string baseUrl;
    private readonly double temperature;
    private readonly double topP;
    private readonly int topK;
    private readonly int? maxTokens;
    private readonly HttpClient http = new HttpClient();

    public AIModel()
    {
        apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];
        model = ConfigurationManager.AppSettings["GeminiModel"] ?? "gemini-1.5-flash";
        baseUrl = ConfigurationManager.AppSettings["GeminiApiBaseUrl"];

        temperature = ParseDouble(ConfigurationManager.AppSettings["GeminiTemperature"], 0.5);
        topP = ParseDouble(ConfigurationManager.AppSettings["GeminiTopP"], 0.9);
        topK = ParseInt(ConfigurationManager.AppSettings["GeminiTopK"], 40);

        string maxTok = ConfigurationManager.AppSettings["GeminiMaxTokens"];
        if (string.IsNullOrWhiteSpace(maxTok))
        {
            maxTokens = null;
        }
        else
        {
            maxTokens = ParseInt(maxTok, 2048);
        }
    }

    private double ParseDouble(string val, double defaultValue)
    {
        double result;
        if (double.TryParse(val, out result))
        {
            return result;
        }
        return defaultValue;
    }

    private int ParseInt(string val, int defaultValue)
    {
        int result;
        if (int.TryParse(val, out result))
        {
            return result;
        }
        return defaultValue;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        string url = string.Format(
            "{0}{1}:generateContent?key={2}",
            baseUrl,
            model,
            apiKey
        );

        var contents = new[]
        {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        };

        object body;

        if (maxTokens.HasValue)
        {
            body = new
            {
                contents = contents,
                generationConfig = new
                {
                    temperature = temperature,
                    topP = topP,
                    topK = topK,
                    maxOutputTokens = maxTokens.Value
                }
            };
        }
        else
        {
            body = new
            {
                contents = contents,
                generationConfig = new
                {
                    temperature = temperature,
                    topP = topP,
                    topK = topK
                }
            };
        }

        string json = JsonConvert.SerializeObject(body);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await http.PostAsync(url, content);
        string responseJson = await response.Content.ReadAsStringAsync();

        return responseJson;
    }
}
