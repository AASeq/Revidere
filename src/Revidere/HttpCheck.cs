namespace Revidere;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using Serilog;

internal sealed class HttpCheck : Check {

    internal HttpCheck(CheckProperties checkProperties)
        : base(checkProperties) {

        Method = checkProperties.Kind switch {
            "GET" => HttpMethod.Get,
            "HEAD" => HttpMethod.Head,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            _ => throw new ArgumentException($"Invalid HTTP method: {checkProperties.Kind}", nameof(checkProperties)),
        };

        TargetUrl = new Uri(checkProperties.Target);
    }

    private readonly Uri TargetUrl;
    private readonly HttpMethod Method;


    private static readonly HttpClient HttpClient = new();

    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        var sw = Stopwatch.StartNew();
        try {
            var timeoutCancelSource = new CancellationTokenSource(Properties.CheckProfile.Timeout);
            var linkedCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancelSource.Token);

            var request = new HttpRequestMessage(Method, TargetUrl);
            var response = HttpClient.SendAsync(request, linkedCancelSource.Token).Result;
            var isHealthy = response.IsSuccessStatusCode;
            Log.Verbose("{Check} status: {Status} ({Code}; {Duration}ms)", this, isHealthy ? "Healthy" : "Unhealthy", (int)response.StatusCode, sw.ElapsedMilliseconds);
            return isHealthy;
        } catch (OperationCanceledException) {
            Log.Verbose("{Check} status: {Status} ({Error}; {Duration}ms)", this, "Unhealthy", "Timeout", sw.ElapsedMilliseconds);
            return false;
        } catch (Exception ex) {
            while(ex.InnerException != null) { ex = ex.InnerException; }  // unwrap all exceptions
            if (ex is HttpRequestException hrex) {
                Log.Verbose("{Check} status: {Status} ({Error}; {Duration}ms)", this, "Unhealthy", hrex.Message, sw.ElapsedMilliseconds);
            } else {
                Log.Verbose("{Check} status: {Status} ({Error}; {Duration}ms)", this, "Unhealthy", ex.Message, sw.ElapsedMilliseconds);
            }
            return false;
        }
    }

}
