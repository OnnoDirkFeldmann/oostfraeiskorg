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
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using oostfraeiskorg.Services;

namespace oostfraeiskorg.ViewModels;

public class TranslatorTestViewModel : MasterPageViewModel
{
    private const int MaxTextLength = 1000;
    private const int TtsRequestTimeoutSeconds = 300;
    private const int MaxTtsRetryDelaySeconds = 20;
    private const int MaxTtsCacheSize = 50;
    private static readonly string ApiSaveFeedbackUrl = "https://vanmoders114-ooversetter-feedback.hf.space/gradio_api/call/save_translation";
    private static readonly string ApiTtsUrl = "https://vanmoders114-east-frisian-tts.hf.space/gradio_api/call/predict";

    private readonly string BearerToken;
    private readonly string SmtpCredentialName;
    private readonly string SmtpCredentialPassword;
    private readonly TranslationCounterService _counterService;

    private static readonly HttpClient SharedHttpClient = new()
    {
        // A sleeping Hugging Face Space can take longer than HttpClient's
        // default 100 seconds. Each operation has its own bounded token below.
        Timeout = Timeout.InfiniteTimeSpan
    };
    private static readonly ConcurrentDictionary<string, string> TtsCache = new();
    private static readonly HuggingFaceSpaceWakeup TtsSpaceWakeup = new(
        SharedHttpClient, "VanModers114/East_Frisian_TTS");

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
    [Bind(Direction.ServerToClient)]
    public string TtsErrorMessage { get; set; } = "";
    public bool IsTtsLoading { get; set; } = false;
    public int TranslationCount { get; set; }
    public string TranslationCountStartDate { get; set; }


    public TranslatorTestViewModel(IConfiguration configuration, TranslationCounterService counterService)
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
        _counterService = counterService;
    }

    public override Task Init()
    {
        MasterPageTitle = "Ooversetter - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Ooversetter - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das Ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", ooversetter, übersetzer, translator ostfriesische Sprache, ostfriesisch, oostfräisk";
        NormalizeLanguageSelection();

        // Load translation counter data
        TranslationCount = _counterService.GetCount();
        TranslationCountStartDate = _counterService.GetStartDate().ToString("dd.MM.yyyy");

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

        OutputText = await TranslatorViewModel.TranslateWithFallback(InputText, InputTitle, TranslationTitle, TranslatorViewModel.SharedNllbEndpoints, BearerToken, MaxTextLength);
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
        TtsAudioUrl = "";
        TtsErrorMessage = "";
        IsTtsLoading = true;
    }

    public async Task GenerateSpeechAsync()
    {
        if (!IsTtsLoading)
            return;

        try
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
            if (TtsCache.Count >= MaxTtsCacheSize)
                TtsCache.Clear();
            TtsCache[textToSpeak] = audio;
            TtsAudioUrl = audio;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TTS Error: {ex.Message}");
            TtsErrorMessage = "Die KI-Sprachausgabe konnte nicht gestartet werden. Bitte versuchen Sie es in einigen Minuten erneut.";
        }
        finally
        {
            IsTtsLoading = false;
        }
    }

    private static bool IsTransientTtsStatus(HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.RequestTimeout ||
               statusCode == HttpStatusCode.TooManyRequests ||
               statusCode == HttpStatusCode.InternalServerError ||
               statusCode == HttpStatusCode.BadGateway ||
               statusCode == HttpStatusCode.ServiceUnavailable ||
               statusCode == HttpStatusCode.GatewayTimeout;
    }

    private static async Task<HttpResponseMessage> SendTtsRequestWithRetry(
        Func<HttpRequestMessage> requestFactory,
        string operation,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                using HttpRequestMessage request = requestFactory();
                HttpResponseMessage response = await SharedHttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                    return response;

                if (!IsTransientTtsStatus(response.StatusCode))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    string body = await response.Content.ReadAsStringAsync(cancellationToken);
                    response.Dispose();
                    string preview = body.Length > 300 ? body[..300] : body;
                    throw new InvalidOperationException(
                        $"TTS {operation} failed with HTTP {(int)statusCode}: {preview}");
                }

                HttpStatusCode transientStatusCode = response.StatusCode;
                TimeSpan retryDelay = GetTtsRetryDelay(response, attempt);
                response.Dispose();
                Console.WriteLine(
                    $"TTS {operation} received HTTP {(int)transientStatusCode}; " +
                    $"retrying in {retryDelay.TotalSeconds:0} seconds (attempt {attempt}).");
                await Task.Delay(retryDelay, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                TimeSpan retryDelay = GetTtsRetryDelay(null, attempt);
                Console.WriteLine(
                    $"TTS {operation} request failed ({ex.GetType().Name}); " +
                    $"retrying in {retryDelay.TotalSeconds:0} seconds (attempt {attempt}).");
                await Task.Delay(retryDelay, cancellationToken);
            }
        }
    }

    private static TimeSpan GetTtsRetryDelay(HttpResponseMessage response, int attempt)
    {
        TimeSpan? retryAfter = response?.Headers.RetryAfter?.Delta;
        if (!retryAfter.HasValue && response?.Headers.RetryAfter?.Date is DateTimeOffset retryDate)
            retryAfter = retryDate - DateTimeOffset.UtcNow;

        TimeSpan delay = retryAfter.HasValue && retryAfter.Value > TimeSpan.Zero
            ? retryAfter.Value
            : TimeSpan.FromSeconds(Math.Pow(2, Math.Min(attempt, 4)));

        return delay > TimeSpan.FromSeconds(MaxTtsRetryDelaySeconds)
            ? TimeSpan.FromSeconds(MaxTtsRetryDelaySeconds)
            : delay;
    }

    public static async Task<string> GenerateSpeech(string text, string apiUrl, string bearerToken)
    {
        var requestBody = new
        {
            data = new string[] { text }
        };
        string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(TtsRequestTimeoutSeconds));

        if (string.Equals(apiUrl, ApiTtsUrl, StringComparison.OrdinalIgnoreCase))
            await TtsSpaceWakeup.EnsureRunningAsync(bearerToken, timeout.Token);

        // The app proxy can still be transitioning after the Hub reports RUNNING, so
        // transient submission failures use the same overall five-minute window.
        using HttpResponseMessage response = await SendTtsRequestWithRetry(
            () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("Authorization", $"Bearer {bearerToken}");
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                return request;
            },
            "submission",
            timeout.Token);
        string responseJson = await response.Content.ReadAsStringAsync(timeout.Token);
        var jsonIdResponse = JObject.Parse(responseJson);
        string eventId = jsonIdResponse["event_id"]?.ToString();

        if (string.IsNullOrEmpty(eventId))
            throw new InvalidOperationException("Failed to retrieve event_id from TTS API response.");

        // Gradio holds this SSE request open until the queued job completes.
        string resultUrl = $"{apiUrl}/{eventId}";
        using HttpResponseMessage resultResponse = await SendTtsRequestWithRetry(
            () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, resultUrl);
                request.Headers.Add("Authorization", $"Bearer {bearerToken}");
                request.Headers.Add("Accept", "text/event-stream");
                return request;
            },
            "result stream",
            timeout.Token);
        string jsonData = await resultResponse.Content.ReadAsStringAsync(timeout.Token);

        if (jsonData.Contains("event: error"))
            throw new InvalidOperationException("The TTS Space reported an inference error.");
        if (!jsonData.Contains("event: complete"))
            throw new InvalidOperationException("The TTS Space closed without a complete event.");

        jsonData = ExtractJsonFromEventStream(jsonData)
            ?? throw new InvalidOperationException("The TTS result did not contain JSON data.");
        JArray jsonResponse = JArray.Parse(jsonData);
        string fileUrl = jsonResponse[0]?["url"]?.ToString();
        if (string.IsNullOrEmpty(fileUrl))
            throw new InvalidOperationException("The TTS result did not contain an audio URL.");

        using HttpResponseMessage downloadResponse = await SendTtsRequestWithRetry(
            () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
                request.Headers.Add("Authorization", $"Bearer {bearerToken}");
                return request;
            },
            "audio download",
            timeout.Token);
        byte[] wavBytes = await downloadResponse.Content.ReadAsByteArrayAsync(timeout.Token);
        // The Space applies provenance before returning audio; preserve its bytes.
        if (wavBytes.Length < 12 || Encoding.ASCII.GetString(wavBytes, 0, 4) != "RIFF" ||
            Encoding.ASCII.GetString(wavBytes, 8, 4) != "WAVE")
            throw new InvalidOperationException("The TTS download was not a WAV file.");

        string base64Audio = Convert.ToBase64String(wavBytes);
        return $"data:audio/wav;base64,{base64Audio}";
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
