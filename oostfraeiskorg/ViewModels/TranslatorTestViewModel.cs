using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace oostfraeiskorg.ViewModels;

public class TranslatorTestViewModel : MasterPageViewModel
{
    private const int MaxTextLength = 1000;
    private const int DelayMilliseconds = 50;
    private static readonly string ApiSaveFeedbackUrl = "https://vanmoders114-ooversetter-feedback.hf.space/gradio_api/call/save_translation";
    private static readonly string ApiTtsUrl = "https://vanmoders114-east-frisian-tts.hf.space/gradio_api/call/predict";

    // Share the same failover endpoint chains as TranslatorViewModel —
    // if the primary goes down on one page, it's locked out on both.
    private static readonly ApiEndpoints FrsEndpoints = TranslatorViewModel.SharedFrsEndpoints;
    private static readonly ApiEndpoints GerEndpoints = TranslatorViewModel.SharedGerEndpoints;

    private readonly string BearerToken;
    private readonly string SmtpCredentialName;
    private readonly string SmtpCredentialPassword;

    private static readonly Dictionary<string, string> TtsCache = new();


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
    public string TtsAudioUrl { get; set; } = "";
    public bool IsTtsLoading { get; set; } = false;


    public TranslatorTestViewModel(IConfiguration configuration)
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
        MasterPageTitle = "Ooversetter - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Ooversetter - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", ooversetter, übersetzer, translator ostfriesische Sprache, ostfriesisch, oostfräisk";
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

        await TranslatorViewModel.SaveFeedback(gerText, frsText, ApiSaveFeedbackUrl, BearerToken);
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

    public void PrepareTranslation()
    {
        OutputText = "";
        IsLoading = true;
        ShowTranslationFeedback = false;
    }

    public async Task TranslateAsync()
    {
        ApiEndpoints endpoints = TranslationText == "Übersetze" ? FrsEndpoints : GerEndpoints;
        // Perform translation with automatic failover
        OutputText = await TranslatorViewModel.TranslateWithFallback(InputText, endpoints, BearerToken, MaxTextLength);
        IsLoading = false;
        ShowTranslationFeedback = true;
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

    public void PrepareGenerateSpeech()
    {
        string textToSpeak = TranslationTitle == "Oostfräisk"
            ? OutputText
            : InputText;

        // Check cache first
        if (TtsCache.TryGetValue(textToSpeak, out string cachedAudio))
        {
            TtsAudioUrl = cachedAudio;
            IsTtsLoading = false;
            return;
        }

        IsTtsLoading = true;
    }

    public async Task GenerateSpeechAsync()
    {
        if (IsTtsLoading)
        {
            string textToSpeak = TranslationTitle == "Oostfräisk"
                ? OutputText
                : InputText;

            if (string.IsNullOrWhiteSpace(textToSpeak))
                return;

            // Check cache first
            if (TtsCache.TryGetValue(textToSpeak, out string cachedAudio))
            {
                TtsAudioUrl = cachedAudio;
                return;
            }

            string audio = await GenerateSpeech(textToSpeak, ApiTtsUrl, BearerToken);
            IsTtsLoading = false;

            if (!string.IsNullOrEmpty(audio))
            {
                TtsCache[textToSpeak] = audio;
                TtsAudioUrl = audio;
            }
        }
    }

    public static async Task<string> GenerateSpeech(string text, string apiUrl, string bearerToken)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        // Prepare request
        var requestBody = new
        {
            data = new string[] { text }
        };
        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            // Step 1: POST to get event_id
            HttpResponseMessage response = await client.PostAsync(ApiTtsUrl, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var jsonIdResponse = JObject.Parse(responseJson);
            string eventId = jsonIdResponse["event_id"]?.ToString();

            if (string.IsNullOrEmpty(eventId))
                throw new Exception("Failed to retrieve event_id from TTS API response.");

            // Step 2: Poll for result
            string resultUrl = $"{ApiTtsUrl}/{eventId}";
            while (true)
            {
                await Task.Delay(DelayMilliseconds);
                HttpResponseMessage resultResponse = await client.GetAsync(resultUrl);
                string jsonData = await resultResponse.Content.ReadAsStringAsync();

                if (jsonData.Contains("event: complete"))
                {
                    jsonData = ExtractJsonFromEventStream(jsonData);
                    JArray jsonResponse = JArray.Parse(jsonData);

                    // Extract HuggingFace "url" field
                    string fileUrl = jsonResponse[0]?["url"]?.ToString();
                    if (string.IsNullOrEmpty(fileUrl))
                        return "";

                    // Download WAV with Bearer token
                    using var downloadClient = new HttpClient();
                    downloadClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
                    byte[] wavBytes = await downloadClient.GetByteArrayAsync(fileUrl);

                    // Convert to base64 for <audio> tag
                    string base64Audio = Convert.ToBase64String(wavBytes);

                    return $"data:audio/wav;base64,{base64Audio}";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TTS Error: {ex.Message}");
            return "";
        }
    }
}