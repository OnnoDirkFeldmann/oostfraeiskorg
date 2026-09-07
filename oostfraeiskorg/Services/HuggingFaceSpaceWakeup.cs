using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace oostfraeiskorg.Services;

// One instance per Space, shared by all synthesis requests in this server process.
public sealed class HuggingFaceSpaceWakeup
{
    private readonly HttpClient client;
    private readonly string hubUrl;
    private readonly SemaphoreSlim gate = new(1, 1);

    public HuggingFaceSpaceWakeup(HttpClient client, string spaceId)
    {
        this.client = client;
        hubUrl = $"https://huggingface.co/api/spaces/{spaceId}";
    }

    public async Task EnsureRunningAsync(string token, CancellationToken cancellationToken, string restartToken = null)
    {
        string lastState = "not yet checked";
        bool entered = false;
        bool restartSent = false;
        try
        {
            await gate.WaitAsync(cancellationToken);
            entered = true;
            while (true)
            {
                using var request = CreateRequest(HttpMethod.Get, "runtime", token);
                using var response = await client.SendAsync(request, cancellationToken);
                if (IsTransient(response.StatusCode))
                {
                    Console.WriteLine($"TTS runtime check received HTTP {(int)response.StatusCode}; retrying in 5 seconds.");
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    continue;
                }
                CheckResponse(response, "runtime check");
                using var runtime = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
                string stage = runtime.RootElement.GetProperty("stage").GetString();
                if (stage != lastState)
                    Console.WriteLine($"TTS Space runtime: {stage}.");
                lastState = stage;

                switch (stage)
                {
                    case "RUNNING":
                        return;
                    case "SLEEPING":
                        if (!restartSent)
                        {
                            // A GET to hf.space can stay at 503 indefinitely. Use
                            // the Hub lifecycle API, without factory rebuilding.
                            // Do not retry this POST: a lost response could still
                            // mean it was accepted, and another POST resets startup.
                            restartSent = true;
                            using var restart = CreateRequest(HttpMethod.Post, "restart", string.IsNullOrWhiteSpace(restartToken) ? token : restartToken);
                            using var restartResponse = await client.SendAsync(restart, cancellationToken);
                            CheckResponse(restartResponse, "restart");
                            Console.WriteLine("TTS Space restart accepted; waiting for RUNNING.");
                        }
                        break;
                    case "BUILDING":
                    case "APP_STARTING":
                    case "RUNNING_BUILDING":
                    case "RUNNING_APP_STARTING":
                        break;
                    case "PAUSED":
                        throw new InvalidOperationException("TTS Space is manually PAUSED; its owner must resume it on Hugging Face.");
                    default:
                        throw new InvalidOperationException($"TTS Space cannot start: runtime stage is '{stage}'. Check the Space build/runtime logs.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"TTS Space wake-up timed out. Last runtime state: {lastState}; restart requested: {restartSent}.", ex);
        }
        finally
        {
            if (entered)
                gate.Release();
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string action, string token)
    {
        var request = new HttpRequestMessage(method, $"{hubUrl}/{action}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.ParseAdd("application/json");
        return request;
    }

    private static void CheckResponse(HttpResponseMessage response, string operation)
    {
        if (response.IsSuccessStatusCode)
            return;
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            throw new InvalidOperationException(operation == "restart"
                ? $"TTS Hub restart denied (HTTP {(int)response.StatusCode}). TranslatorConfig:TtsRestartToken (or BearerToken when omitted) must belong to the Space owner and allow restarting VanModers114/East_Frisian_TTS."
                : $"TTS Hub runtime check denied (HTTP {(int)response.StatusCode}). Check TranslatorConfig:BearerToken has read access to the TTS Space.");
        throw new InvalidOperationException($"TTS Hub {operation} failed with HTTP {(int)response.StatusCode}.");
    }

    private static bool IsTransient(HttpStatusCode status) =>
        status == HttpStatusCode.RequestTimeout || status == HttpStatusCode.TooManyRequests ||
        status == HttpStatusCode.InternalServerError || status == HttpStatusCode.BadGateway ||
        status == HttpStatusCode.ServiceUnavailable || status == HttpStatusCode.GatewayTimeout;
}
