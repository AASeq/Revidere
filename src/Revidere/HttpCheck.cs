namespace Revidere;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
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

        var handler = checkProperties.AllowInsecure switch {
            true => new HttpClientHandler {
                AllowAutoRedirect = checkProperties.FollowRedirect,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            },
            false => new HttpClientHandler {
                AllowAutoRedirect = checkProperties.FollowRedirect,
            },
        };
        HttpClient = new(handler);

    }

    private readonly Uri TargetUrl;
    private readonly HttpMethod Method;
    private readonly HttpClient HttpClient;

    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        var sw = Stopwatch.StartNew();
        try {
            var timeoutCancelSource = new CancellationTokenSource(Properties.CheckProfile.Timeout);
            var linkedCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancelSource.Token);

            var request = new HttpRequestMessage(Method, TargetUrl);
            var response = HttpClient.SendAsync(request, linkedCancelSource.Token).Result;
            if (response.StatusCode == HttpStatusCode.Redirect) {
                var redirectLocation = response.Headers.Location;
                Log.Verbose("{Check} status: {Status} ({Code} {Location}; {Duration}ms)", this, "Redirect", (int)response.StatusCode,redirectLocation,  sw.ElapsedMilliseconds);
                return false;
            } else {
                var isHealthy = response.IsSuccessStatusCode;
                Log.Verbose("{Check} status: {Status} ({Code}; {Duration}ms)", this, isHealthy ? "Healthy" : "Unhealthy", (int)response.StatusCode, sw.ElapsedMilliseconds);
                return isHealthy;
            }
        } catch (OperationCanceledException) {
            Log.Verbose("{Check} status: {Status} ({Error}; {Duration}ms)", this, "Unhealthy", "Timeout", sw.ElapsedMilliseconds);
            return false;
        } catch (Exception ex) {
            while (ex.InnerException != null) { ex = ex.InnerException; }  // unwrap all exceptions
            if (ex is HttpRequestException hrex) {
                Log.Verbose("{Check} status: {Status} ({Error}; {Duration}ms)", this, "Unhealthy", hrex.Message, sw.ElapsedMilliseconds);
            } else {
                Log.Verbose("{Check} status: {Status} ({Error}; {Duration}ms)", this, "Unhealthy", ex.Message, sw.ElapsedMilliseconds);
            }
            return false;
        }
    }

}
