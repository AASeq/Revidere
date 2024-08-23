namespace Revidere;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Serilog;

internal sealed class HttpCheck : Check {

    internal HttpCheck(CheckProperties checkProperties)
        : base(checkProperties) {

        Method = Kind switch {
            "GET" => HttpMethod.Get,
            "HEAD" => HttpMethod.Head,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            _ => throw new ArgumentException($"Invalid HTTP method: {checkProperties.Kind}", nameof(checkProperties)),
        };

        TargetUrl = new Uri(Target);
    }

    private readonly Uri TargetUrl;
    private readonly HttpMethod Method;


    private static readonly HttpClient HttpClient = new();

    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        try {
            var timeoutCancelSource = new CancellationTokenSource(CheckProfile.Timeout);
            var linkedCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancelSource.Token);

            var request = new HttpRequestMessage(Method, TargetUrl);
            var response = HttpClient.SendAsync(request, linkedCancelSource.Token).Result;
            Log.Verbose("HTTP {Method} check for {Uri}: {Status}", Method, TargetUrl, response.StatusCode);
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            if (ex.InnerException is HttpRequestException hrex) {
                Log.Debug("HTTP {Method} check for {Uri}: {Status}", Method, TargetUrl, hrex.Message);
            } else {
                Log.Verbose("HTTP {Method} check for {Uri}: {Status}", Method, TargetUrl, ex);
            }
            return false;
        }
    }

}
