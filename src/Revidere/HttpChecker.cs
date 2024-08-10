namespace Revidere;

using System;
using System.Net.Http;
using System.Threading;
using Serilog;

internal class HttpChecker : IChecker {

    public HttpChecker(Uri target) {
        Target = target;
    }

    private readonly Uri Target;
    private static readonly HttpClient HttpClient = new();

    public bool CheckIsHealthy(CancellationToken cancellationToken, TimeSpan timeout) {
        try {
            var timeoutCancelSource = new CancellationTokenSource(timeout);
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
