using DotVVM.Framework.Controls;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using DotVVM.Framework.Binding;

namespace oostfraeiskorg.ViewModels;

public class TranslatorViewModel : MasterPageViewModel
{
    private const int MaxTextLength = 500;
    private const int DelayMilliseconds = 500;
    private static readonly string ApiUrl = "https://vanmoders114-east-frisian-translator.hf.space/gradio_api/call/predict";

    public string GermanText { get; set; } = "";
    public string EastFrisianText { get; set; } = "";

    public override Task Init()
    {
        MasterPageTitle = "Übersetzer - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Übersetzer - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", übersetzer, translator ostfriesische Sprache, ostfriesisch, oostfräisk";
        return base.Init();
    }

    public async Task TranslateAsync()
    {
        // Perform translation
        EastFrisianText = await Translate(GermanText);
    }

    public static async Task<string> Translate(string text)
    {
        if (text.Length > MaxTextLength)
        {
            text = text.Substring(0, MaxTextLength);
        }

        using HttpClient client = new HttpClient();

        // JSON payload
        var requestBody = new
        {
            data = new string[] { text }
        };

        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            // Step 1: Send POST request to get the event ID
            HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var jsonIdResponse = JObject.Parse(responseJson);
            string eventId = jsonIdResponse["event_id"]?.ToString();

            if (string.IsNullOrEmpty(eventId))
            {
                throw new Exception("Failed to retrieve event_id from API response.");
            }

            // Step 2: Fetch the translation result using event ID
            string resultUrl = $"{ApiUrl}/{eventId}";

            while (true)
            {
                await Task.Delay(DelayMilliseconds); // Wait before checking the result
                HttpResponseMessage resultResponse = await client.GetAsync(resultUrl);
                string jsonData = await resultResponse.Content.ReadAsStringAsync();

                if (jsonData.Contains("event: complete"))
                {
                    jsonData = ExtractJsonFromEventStream(jsonData);
                    JArray jsonResponse = JArray.Parse(jsonData);

                    // Extract the translation from the "data" array
                    string translation = jsonResponse[0]?.ToString();
                    return translation ?? "Translation failed.";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return "Translation failed.";
        }
    }

    private static string ExtractJsonFromEventStream(string rawResponse)
    {
        // Split response by lines
        string[] lines = rawResponse.Split('\n');
        string lastLine = "";

        foreach (string line in lines)
        {
            if (lastLine.StartsWith("event: complete") && line.StartsWith("data: "))
            {
                // Extract the JSON part after "data: "
                return line.Substring(6).Trim();
            }
            lastLine = line;
        }
        return null;
    }
}
