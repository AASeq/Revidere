namespace Revidere;

using System;
using System.Net.Http;
using System.Threading;
using Serilog;

internal class HttpChecker : IChecker {

    public HttpChecker(Uri targetUri) {
        TargetUri = targetUri;
    }

    private readonly Uri TargetUri;
    private static readonly HttpClient HttpClient = new();

    public bool CheckIsHealthy(CancellationToken cancellationToken, TimeSpan timeout) {
        try {
            var timeoutCancelSource = new CancellationTokenSource(timeout);
            var linkedCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancelSource.Token);

            var request = new HttpRequestMessage(HttpMethod.Get, TargetUri);
            var response = HttpClient.SendAsync(request, linkedCancelSource.Token).Result;
            Log.Verbose("HTTP check for {Uri}: {Status}", TargetUri, response.StatusCode);
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Log.Verbose("HTTP check for {Uri}: {Status}", TargetUri, ex);
            return false;
        }
    }

}
