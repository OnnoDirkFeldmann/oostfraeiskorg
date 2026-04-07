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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    private const int DelayMilliseconds = 500;
    private const int PollTimeoutSeconds = 60;
    private const int MaxTtsCacheSize = 50;
    private static readonly string ApiSaveFeedbackUrl = "https://vanmoders114-ooversetter-feedback.hf.space/gradio_api/call/save_translation";
    private static readonly string ApiTtsUrl = "https://vanmoders114-east-frisian-tts.hf.space/gradio_api/call/predict";
    private static readonly string ApiNllbUrl = "https://vanmoders114-east-frisian-nllb-translator.hf.space/gradio_api/call/translate";

    private readonly string BearerToken;
    private readonly string SmtpCredentialName;
    private readonly string SmtpCredentialPassword;

    private static readonly HttpClient SharedHttpClient = new();
    private static readonly ConcurrentDictionary<string, string> TtsCache = new();

    public List<string> LanguageOptions { get; } = ["Deutsch", "English", "Oostfräisk"];

    public bool DoubleTranslationEnabled { get; } = true;

    public int MaximumTextLength {get; } = MaxTextLength;
    public string InputTitle { get; set; } = "Deutsch";
    public string TranslationTitle { get; set; } = "Oostfräisk";
    public string InputText { get; set; } = "";
    public string InputPlaceholderText { get; set; } = "Hier steht der Text.";
    public string OutputText { get; set; } = "";
    public string TranslationPlaceholderText { get; set; } = "Hier steht die Übersetzung.";
    public string TranslationText { get; set; } = "Übersetzen";
    public bool ShowTranslationFeedback { get; set; } = false;
    public bool IsLoading { get; set; } = false;
    [Bind(Direction.ServerToClient)]
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
        NormalizeLanguageSelection();
        return base.Init();
    }

    public override Task PreRender()
    {
        NormalizeLanguageSelection();
        return base.PreRender();
    }

    public void SwitchTranslationMode()
    {
        var temp = InputTitle;
        InputTitle = TranslationTitle;
        TranslationTitle = temp;

        var tempText = InputText;
        InputText = OutputText;
        OutputText = tempText;

        NormalizeLanguageSelection();
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
        string markedOutput = OutputText + addedMarker;

        string gerText = InputTitle == "Deutsch"
            ? InputText
            : (TranslationTitle == "Deutsch" ? markedOutput : string.Empty);

        string frsText = InputTitle == "Oostfräisk"
            ? InputText
            : (TranslationTitle == "Oostfräisk" ? markedOutput : string.Empty);

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
                    Body = $"{InputTitle}: {InputText}\n\n{TranslationTitle}:\n\n{OutputText}{addedMarker}",
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
        if (!IsAllowedPair(InputTitle, TranslationTitle))
        {
            OutputText = "Diese Sprachkombination wird derzeit nicht unterstützt.";
            IsLoading = false;
            ShowTranslationFeedback = false;
            return;
        }

        OutputText = await Translate(InputText, InputTitle, TranslationTitle, ApiNllbUrl, BearerToken, MaxTextLength);
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
                IsTtsLoading = false;
                return;
            }

            string audio = await GenerateSpeech(textToSpeak, ApiTtsUrl, BearerToken);
            IsTtsLoading = false;

            if (!string.IsNullOrEmpty(audio))
            {
                if (TtsCache.Count >= MaxTtsCacheSize)
                    TtsCache.Clear();
                TtsCache[textToSpeak] = audio;
                TtsAudioUrl = audio;
            }
        }
    }

    public static async Task<string> GenerateSpeech(string text, string apiUrl, string bearerToken)
    {
        var requestBody = new
        {
            data = new string[] { text }
        };
        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

        try
        {
            // Step 1: POST to get event_id
            using var postRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            postRequest.Headers.Add("Authorization", $"Bearer {bearerToken}");
            postRequest.Headers.Add("Accept", "application/json");
            postRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await SharedHttpClient.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var jsonIdResponse = JObject.Parse(responseJson);
            string eventId = jsonIdResponse["event_id"]?.ToString();

            if (string.IsNullOrEmpty(eventId))
                throw new Exception("Failed to retrieve event_id from TTS API response.");

            // Step 2: Poll for result
            string resultUrl = $"{apiUrl}/{eventId}";
            var deadline = DateTime.UtcNow.AddSeconds(PollTimeoutSeconds);
            while (DateTime.UtcNow < deadline)
            {
                await Task.Delay(DelayMilliseconds);

                using var pollRequest = new HttpRequestMessage(HttpMethod.Get, resultUrl);
                pollRequest.Headers.Add("Authorization", $"Bearer {bearerToken}");
                pollRequest.Headers.Add("Accept", "application/json");

                HttpResponseMessage resultResponse = await SharedHttpClient.SendAsync(pollRequest);
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
                    using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, fileUrl);
                    downloadRequest.Headers.Add("Authorization", $"Bearer {bearerToken}");

                    HttpResponseMessage downloadResponse = await SharedHttpClient.SendAsync(downloadRequest);
                    byte[] wavBytes = await downloadResponse.Content.ReadAsByteArrayAsync();

                    // Convert to base64 for <audio> tag
                    string base64Audio = Convert.ToBase64String(wavBytes);

                    return $"data:audio/wav;base64,{base64Audio}";
                }
            }

            throw new Exception("TTS API polling timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TTS Error: {ex.Message}");
            return "";
        }
    }

    public static async Task<string> Translate(string text, string sourceLanguage, string targetLanguage, string apiUrl, string bearerToken, int maxTextLength)
    {
        if (text.Length > maxTextLength)
        {
            text = text.Substring(0, maxTextLength);
        }

        var requestBody = new
        {
            data = new string[] { text, sourceLanguage, targetLanguage }
        };

        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

        try
        {
            using var postRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            postRequest.Headers.Add("Authorization", $"Bearer {bearerToken}");
            postRequest.Headers.Add("Accept", "application/json");
            postRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await SharedHttpClient.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var jsonIdResponse = JObject.Parse(responseJson);
            string eventId = jsonIdResponse["event_id"]?.ToString();

            if (string.IsNullOrEmpty(eventId))
            {
                throw new Exception("Failed to retrieve event_id from API response.");
            }

            string resultUrl = $"{apiUrl}/{eventId}";

            var deadline = DateTime.UtcNow.AddSeconds(PollTimeoutSeconds);
            while (DateTime.UtcNow < deadline)
            {
                await Task.Delay(DelayMilliseconds);

                using var pollRequest = new HttpRequestMessage(HttpMethod.Get, resultUrl);
                pollRequest.Headers.Add("Authorization", $"Bearer {bearerToken}");
                pollRequest.Headers.Add("Accept", "application/json");

                HttpResponseMessage resultResponse = await SharedHttpClient.SendAsync(pollRequest);
                string jsonData = await resultResponse.Content.ReadAsStringAsync();

                if (jsonData.Contains("event: complete"))
                {
                    jsonData = ExtractJsonFromEventStream(jsonData);
                    JArray jsonResponse = JArray.Parse(jsonData);
                    string translation = jsonResponse[0]?.ToString();
                    return translation ?? throw new Exception("Translation response was null.");
                }

                if (jsonData.Contains("event: error"))
                {
                    throw new Exception($"API returned an error event for {apiUrl}.");
                }
            }

            throw new Exception($"API polling timed out for {apiUrl}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Translation error: {ex.Message}");
            return "Translation failed.";
        }
    }

    private bool IsAllowedPair(string sourceLanguage, string targetLanguage)
    {
        if (string.Equals(sourceLanguage, targetLanguage, StringComparison.Ordinal))
        {
            return false;
        }

        bool germanEnglishPair =
            (sourceLanguage == "Deutsch" && targetLanguage == "English") ||
            (sourceLanguage == "English" && targetLanguage == "Deutsch");

        return !germanEnglishPair;
    }

    public void NormalizeLanguageSelection()
    {
        if (IsAllowedPair(InputTitle, TranslationTitle))
        {
            return;
        }

        string allowedOutput = LanguageOptions.FirstOrDefault(language => IsAllowedPair(InputTitle, language));
        if (!string.IsNullOrEmpty(allowedOutput))
        {
            TranslationTitle = allowedOutput;
            return;
        }

        string allowedInput = LanguageOptions.FirstOrDefault(language => IsAllowedPair(language, TranslationTitle));
        if (!string.IsNullOrEmpty(allowedInput))
        {
            InputTitle = allowedInput;
            return;
        }

        InputTitle = "Deutsch";
        TranslationTitle = "Oostfräisk";
    }
}