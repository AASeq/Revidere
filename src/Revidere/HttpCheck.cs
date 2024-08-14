namespace Revidere;

using System;
using System.Net.Http;
using System.Threading;
using Serilog;

internal sealed class HttpCheck : Check {

    internal HttpCheck(string kind, string target, string title, string? name, bool isVisible, bool isBreak, CheckProfile profile)
        : base(kind, target, title, name, isVisible, isBreak, profile) {
        Method = Kind switch {
            "GET" => HttpMethod.Get,
            "HEAD" => HttpMethod.Head,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            _ => throw new ArgumentException($"Invalid HTTP method: {kind}", nameof(kind)),
        };
        TargetUrl = new Uri(Target);
    }

    private readonly Uri TargetUrl;
    private readonly HttpMethod Method;

    private static readonly HttpClient HttpClient = new();

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        try {
            var timeoutCancelSource = new CancellationTokenSource(CheckProfile.Timeout);
            var linkedCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancelSource.Token);

            var request = new HttpRequestMessage(Method, TargetUrl);
            var response = HttpClient.SendAsync(request, linkedCancelSource.Token).Result;
            Log.Verbose("HTTP {Method} check for {Uri}: {Status}", Method, TargetUrl, response.StatusCode);
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Log.Verbose("HTTP {Method} check for {Uri}: {Status}", Method, TargetUrl, ex);
            return false;
        }
    }

}
