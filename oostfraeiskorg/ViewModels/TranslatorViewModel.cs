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
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace oostfraeiskorg.ViewModels;

public class TranslatorViewModel : MasterPageViewModel
{
    private const int MaxTextLength = 300;
    private const int DelayMilliseconds = 50;
    private static readonly string ApiFrsUrl = "https://vanmoders114-east-frisian-translator.hf.space/gradio_api/call/predict";
    private static readonly string ApiGerUrl = "https://vanmoders114-east-frisian-german-translator.hf.space/gradio_api/call/predict";
    private static readonly string ApiSaveFeedbackUrl = "https://vanmoders114-ooversetter-feedback.hf.space/gradio_api/call/save_translation";
    //private static readonly string ApiUrl = "http://127.0.0.1:7860/gradio_api/call/predict";

    private readonly string BearerToken;


    public bool DoubleTranslationEnabled { get; } = true;

    public int MaximumTextLength {get; } = MaxTextLength;
    public string InputTitle { get; set; } = "Deutsch";
    public string TranslationTitle { get; set; } = "Oostfräisk";
    public string InputText { get; set; } = "";
    public string InputPlaceholderText { get; set; } = "Hier steht der deutsche Text.";
    public string OutputText { get; set; } = "";
    public string TranslationPlaceholderText { get; set; } = "Hir staajt däi oostfräisk tekst.";
    public string TranslationText { get; set; } = "Übersetze";
    public bool ShowTranslationFeedback { get; set; } = false;
    public bool IsLoading { get; set; } = false;


    public TranslatorViewModel(IConfiguration configuration)
    {
        // Load the BearerToken from appsettings.json
        BearerToken = configuration["TranslatorConfig:BearerToken"];
        if (string.IsNullOrEmpty(BearerToken))
        {
            throw new Exception("BearerToken is missing in the configuration.");
        }
    }

    public override Task Init()
    {
        MasterPageTitle = "Übersetzer - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Übersetzer - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", übersetzer, translator ostfriesische Sprache, ostfriesisch, oostfräisk";
        return base.Init();
    }

    public void SwitchTranslationMode()
    {
        var temp = InputTitle;
        InputTitle = TranslationTitle;
        TranslationTitle = temp;
        var tempText = InputText;
        InputText = OutputText;
        OutputText = tempText;
        var tempPlaceholder = InputPlaceholderText;
        InputPlaceholderText = TranslationPlaceholderText;
        TranslationPlaceholderText = tempPlaceholder;

        TranslationText = TranslationText == "Übersetze" ? "ooversetten" : "Übersetze";
    }

    public void ClearInput()
    {
        InputText = "";
        OutputText = "";
        ShowTranslationFeedback = false;
    }

    public void DeactivateFeedback()
    {
        ShowTranslationFeedback = false;
    }

    public async Task ReportTranslationIssue()
    {
        await SentReport("Translation Issue Report", " [FEELER]");
        ShowTranslationFeedback = false;
    }

    public async Task ReportTranslationSuccess()
    {
        await SentReport("Translation Success Report", "");
        ShowTranslationFeedback = false;
    }

    private async Task SentReport(string subject, string addedMarker)
    {
        string gerText = TranslationText == "Übersetze" ? InputText : OutputText + addedMarker;
        string frsText = TranslationText == "Übersetze" ? OutputText + addedMarker : InputText;

        await SaveFeedback(gerText, frsText, ApiSaveFeedbackUrl, BearerToken);
        try
        {
            using (var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("edufraeisk@gmail.com", "aqaxxjjwedmtvuic")
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("edufraeisk@gmail.com"),
                    Subject = subject,
                    Body = $"{gerText}\n\nOOVERSETTEN:\n\n{frsText}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add("oostfraeisk.ooversetter@gmail.com");

                client.Send(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

    public static async Task SaveFeedback(string gerText, string frsText, string apiUrl, string bearerToken)
    {
        using HttpClient client = new HttpClient();

        // Set up the headers
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        // JSON payload
        var requestBody = new
        {
            data = new string[] { gerText, frsText }
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

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void PrepareTranslation()
    {
        OutputText = "...";
        IsLoading = true;
        ShowTranslationFeedback = false;
    }

    public async Task TranslateAsync()
    {
        string apiUrl = TranslationText == "Übersetze" ? ApiFrsUrl : ApiGerUrl;
        // Perform translation
        OutputText = await Translate(InputText, apiUrl, BearerToken);
        IsLoading = false;
        ShowTranslationFeedback = true;
    }

    public static async Task<string> Translate(string text, string apiUrl, string bearerToken)
    {
        if (text.Length > MaxTextLength)
        {
            text = text.Substring(0, MaxTextLength);
        }

        using HttpClient client = new HttpClient();

        // Set up the headers
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

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
