namespace Revidere;

using System;
using System.Net.Http;
using System.Threading;
using Serilog;

internal sealed class HttpCheck : Check {

    internal HttpCheck(string kind, string target, string title, string? name, CheckProfile profile)
        : base(kind, target, title, name, profile) {
    }

    private static readonly HttpClient HttpClient = new();

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        try {
            var timeoutCancelSource = new CancellationTokenSource(CheckProfile.Timeout);
            var linkedCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancelSource.Token);

            var request = new HttpRequestMessage(HttpMethod.Get, Target);
            var response = HttpClient.SendAsync(request, linkedCancelSource.Token).Result;
            Log.Verbose("HTTP check for {Uri}: {Status}", Target, response.StatusCode);
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Log.Verbose("HTTP check for {Uri}: {Status}", Target, ex);
            return false;
        }
    }

}
