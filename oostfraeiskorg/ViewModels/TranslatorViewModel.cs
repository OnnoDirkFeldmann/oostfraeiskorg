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

/// <summary>
/// Holds an ordered list of API URLs and tracks which one is currently active.
/// Once a URL fails, the next one is tried and the failed one is never retried.
/// Thread-safe so concurrent requests don't corrupt the index.
/// </summary>
public class ApiEndpoints
{
    private readonly string[] _urls;
    private volatile int _currentIndex;

    public ApiEndpoints(params string[] urls)
    {
        if (urls is null || urls.Length == 0)
            throw new ArgumentException("At least one URL must be provided.", nameof(urls));
        _urls = urls;
    }

    public string CurrentUrl => _urls[_currentIndex];

    /// <summary>
    /// Advances to the next URL if the current one matches <paramref name="failedUrl"/>.
    /// Returns true if a fallback URL is available, false if all URLs are exhausted.
    /// </summary>
    public bool TryAdvance(string failedUrl)
    {
        lock (_urls)
        {
            // Only advance if the failure is for the URL we're currently on
            if (_currentIndex < _urls.Length && _urls[_currentIndex] == failedUrl)
            {
                _currentIndex++;
            }
            return _currentIndex < _urls.Length;
        }
    }

    public bool IsExhausted => _currentIndex >= _urls.Length;

    /// <summary>
    /// Resets the endpoint chain to the first URL, allowing retries after exhaustion.
    /// Thread-safe.
    /// </summary>
    public void Reset()
    {
        lock (_urls)
        {
            _currentIndex = 0;
        }
    }
}

public class TranslatorViewModel : MasterPageViewModel
{
    private const int MaxTextLength = 300;
    private const int DelayMilliseconds = 50;

    // Shared endpoint chains — exposed so TranslatorTestViewModel can reuse them.
    // Failover state persists for the lifetime of the app process and is shared across both pages.
    public static readonly ApiEndpoints SharedFrsEndpoints = new(
        "https://vanmoders114-east-frisian-translator.hf.space/gradio_api/call/translate",
        "https://vanmoders114-east-frisian-translator-backup.hf.space/gradio_api/call/translate",
        "http://127.0.0.1:7860/gradio_api/call/translate"
    );

    public static readonly ApiEndpoints SharedGerEndpoints = new(
        "https://vanmoders114-east-frisian-german-translator.hf.space/gradio_api/call/translate",
        "https://vanmoders114-east-frisian-german-translator-backup.hf.space/gradio_api/call/translate",
        "http://127.0.0.1:7861/gradio_api/call/translate"
    );

    private static readonly string ApiSaveFeedbackUrl = "https://vanmoders114-ooversetter-feedback.hf.space/gradio_api/call/save_translation";

    private readonly string BearerToken;
    private readonly string SmtpCredentialName;
    private readonly string SmtpCredentialPassword;


    public bool DoubleTranslationEnabled { get; } = true;

    public int MaximumTextLength { get; } = MaxTextLength;
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
        // Load settings
        BearerToken = configuration["TranslatorConfig:BearerToken"];
        if (string.IsNullOrEmpty(BearerToken))
        {
            throw new Exception("BearerToken is missing in the configuration.");
        }
        SmtpCredentialName = configuration["TranslatorConfig:SmtpCredentialName"];
        if (string.IsNullOrEmpty(SmtpCredentialName))
        {
            throw new Exception("SmtpCredentialName is missing in the configuration.");
        }
        SmtpCredentialPassword = configuration["TranslatorConfig:SmtpCredentialPassword"];
        if (string.IsNullOrEmpty(SmtpCredentialPassword))
        {
            throw new Exception("SmtpCredentialPassword is missing in the configuration.");
        }
    }

    public override Task Init()
    {
        MasterPageTitle = "Ooversetter - Übersetzer für das Ostfriesische Platt - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Ooversetter - Übersetzer für das Ostfriesische Platt - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", ooversetter, übersetzer, ostfriesisches platt, translator ostfriesische Sprache, ostfriesisch, oostfräisk, ostfriesisches plattdeutsch";
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
                Credentials = new NetworkCredential(SmtpCredentialName, SmtpCredentialPassword)
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
        OutputText = "";
        IsLoading = true;
        ShowTranslationFeedback = false;
    }

    public async Task TranslateAsync()
    {
        ApiEndpoints endpoints = TranslationText == "Übersetze" ? SharedFrsEndpoints : SharedGerEndpoints;
        // Perform translation with automatic failover
        OutputText = await TranslateWithFallback(InputText, endpoints, BearerToken, MaxTextLength);
        IsLoading = false;
        ShowTranslationFeedback = true;
    }

    /// <summary>
    /// Attempts translation using the current endpoint. On failure, advances to the next
    /// backup URL and retries. Stops when a translation succeeds or all URLs are exhausted.
    /// Resets the endpoint chain after exhaustion to allow retries on future attempts.
    /// </summary>
    public static async Task<string> TranslateWithFallback(string text, ApiEndpoints endpoints, string bearerToken, int maxTextLength)
    {
        while (!endpoints.IsExhausted)
        {
            string currentUrl = endpoints.CurrentUrl;
            try
            {
                return await Translate(text, currentUrl, bearerToken, maxTextLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Translation failed for {currentUrl}: {ex.Message}");
                if (!endpoints.TryAdvance(currentUrl))
                {
                    break; // No more fallback URLs
                }
                Console.WriteLine($"Falling back to {endpoints.CurrentUrl}");
            }
        }

        // Reset endpoints for future retry attempts
        endpoints.Reset();
        return "Translation failed.";
    }

    /// <summary>
    /// Performs the actual API call. Throws on failure so the caller can handle failover.
    /// </summary>
    public static async Task<string> Translate(string text, string apiUrl, string bearerToken, int maxTextLength)
    {
        if (text.Length > maxTextLength)
        {
            text = text.Substring(0, maxTextLength);
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
                return translation ?? throw new Exception("Translation response was null.");
            }

            if (jsonData.Contains("event: error"))
            {
                throw new Exception($"API returned an error event for {apiUrl}.");
            }
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
