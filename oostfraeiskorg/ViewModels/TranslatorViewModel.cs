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

    public string GermanText { get; set; } = "";

    public string EastFrisianText { get; set; } = "";

    public override Task Init()
    {

        return base.Init();
    }

    public async Task TranslateAsync()
    {
        // Perform translation
        EastFrisianText = await Translate(GermanText);
    }

    private static readonly string apiUrl = "https://vanmoders114-east-frisian-translator.hf.space/gradio_api/call/predict";

    public static async Task<string> Translate(string text)
    {
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
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var jsonIdResponse = JObject.Parse(responseJson);
            string eventId = jsonIdResponse["event_id"]?.ToString();

            if (string.IsNullOrEmpty(eventId))
            {
                throw new Exception("Failed to retrieve event_id from API response.");
            }

            // Step 2: Fetch the translation result using event ID
            string resultUrl = $"{apiUrl}/{eventId}";

            while (true)
            {
                await Task.Delay(500); // Wait before checking the result
                HttpResponseMessage resultResponse = await client.GetAsync(resultUrl);
                string jsonData = await resultResponse.Content.ReadAsStringAsync();

                jsonData = ExtractJsonFromEventStream(jsonData);
                JArray jsonResponse = JArray.Parse(jsonData);

                // Extract the translation from the "data" array
                string translation = jsonResponse[0]?.ToString();
                return translation ?? "Translation failed.";
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

        foreach (string line in lines)
        {
            if (line.StartsWith("data: "))
            {
                // Extract the JSON part after "data: "
                return line.Substring(6).Trim();
            }
        }
        return null;
    }
}
