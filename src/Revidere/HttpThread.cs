namespace Revidere;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

internal static class HttpThread {

    public static void Start(string webUriPrefix, string webTitle, int refreshInterval, IReadOnlyList<TargetState> targetStates, CancellationToken cancellationToken) {
        Log.Verbose("Starting HttpThread");

        WebUriPrefix = webUriPrefix;
        WebTitle = webTitle;
        RefreshInterval = refreshInterval;
        TargetStates = targetStates;
        CancellationToken = cancellationToken;

        Thread = new Thread(() => {
            try {
                Task.Run(async () => { await Run(); }).Wait();
            } catch (AggregateException ex) {
                if (ex.InnerException == null) { throw; }
                throw ex.InnerException;  // just return the first one
            }
        }) {
            IsBackground = true,
            Name = "Listener",
        };
        Thread.Start();
    }

    public static void Stop() {
        Log.Verbose("Stopping HttpThread");
        Thread?.Join();
    }


    private static Thread? Thread;
    private static CancellationToken? CancellationToken;
    private static IReadOnlyList<TargetState>? TargetStates;
    private static string WebUriPrefix = string.Empty;
    private static string WebTitle = string.Empty;
    private static int RefreshInterval = 10;


    private static async Task Run() {
        Log.Debug("Started HttpThread");

        var cancellationToken = CancellationToken!.Value;
        var targetStates = TargetStates;

        var listener = new HttpListener();
        listener.Prefixes.Add(WebUriPrefix);
        listener.Start();

        while (!cancellationToken.IsCancellationRequested) {
            try {
                var context = await listener.GetContextAsync().WaitAsync(cancellationToken); ;
                ProcessHttp(context, targetStates!);
            } catch (TaskCanceledException) { }
        }

        Log.Debug("Stopped HttpThread");
    }

    private static bool ProcessHttp(HttpListenerContext context, IReadOnlyList<TargetState> targetStates) {
        var request = context.Request;
        var path = request.Url?.AbsolutePath ?? string.Empty;
        Log.Verbose("Processing HTTP request for {Path}", path);

        using var response = context.Response;
        if (path == "/") {
            Html.FillResponse(response, targetStates, WebTitle, RefreshInterval);
            return true;
        } else if (path.Equals("/healthz", StringComparison.OrdinalIgnoreCase)) {
            HealthZ.FillRootResponse(response, targetStates);
            return true;
        } else if (path.StartsWith("/healthz/", StringComparison.OrdinalIgnoreCase)) {  // individual healthcheck
            var targetName = path["/healthz/".Length..];
            if (string.IsNullOrEmpty(targetName)) {
                HealthZ.FillRootResponse(response, targetStates);
            } else {
                foreach (var targetState in targetStates) {
                    if (string.Equals(targetState.Target.Name, targetName, StringComparison.OrdinalIgnoreCase)) {
                        HealthZ.FillTargetResponse(response, targetState);
                        return true;
                    }
                }
            }
            Log.Debug("Cannot find target named {TargetName}", targetName);
        }

        Log.Debug("URL at {Path} not found", path);
        response.StatusCode = (int)HttpStatusCode.NotFound;
        return false;
    }

}
