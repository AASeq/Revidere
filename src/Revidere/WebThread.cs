namespace Revidere;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

internal static class WebThread {

    public static void Start(WebConfiguration configuration, IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        Log.Verbose("Starting Web thread");

        Configuration = configuration;
        CheckStates = checkStates;
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
            Name = "Web",
        };
        Thread.Start();
    }

    public static void Stop() {
        Log.Verbose("Stopping Web thread");
        Thread?.Join();
    }


    private static WebConfiguration? Configuration;
    private static IReadOnlyList<CheckState>? CheckStates;
    private static Thread? Thread;
    private static CancellationToken? CancellationToken;


    private static async Task Run() {
        Log.Debug("Started Web thread");

        var cancellationToken = CancellationToken!.Value;
        var checkStates = CheckStates;

        var listener = new HttpListener();
        foreach (var prefix in Configuration!.Prefixes) {
            Log.Verbose("Adding {Prefix} as listener", prefix);
            listener.Prefixes.Add(prefix);
        }
        listener.Start();

        while (!cancellationToken.IsCancellationRequested) {
            try {
                var context = await listener.GetContextAsync().WaitAsync(cancellationToken); ;
                ProcessHttp(context, checkStates!);
            } catch (TaskCanceledException) { }
        }

        Log.Debug("Stopped Web thread");
    }

    private static bool ProcessHttp(HttpListenerContext context, IReadOnlyList<CheckState> checkStates) {
        var request = context.Request;
        var path = request.Url?.AbsolutePath ?? string.Empty;
        Log.Verbose("Processing HTTP request for {Path}", path);

        using var response = context.Response;
        if (path == "/") {
            Html.FillResponse(response, checkStates, Configuration!.Title, Configuration!.RefreshInterval);
            return true;
        } else if (path.Equals("/healthz", StringComparison.OrdinalIgnoreCase)) {
            HealthZ.FillRootResponse(response, checkStates);
            return true;
        } else if (path.StartsWith("/healthz/", StringComparison.OrdinalIgnoreCase)) {  // individual healthcheck
            var checkName = path["/healthz/".Length..];
            if (string.IsNullOrEmpty(checkName)) {
                HealthZ.FillRootResponse(response, checkStates);
            } else {
                foreach (var checkState in checkStates) {
                    if (string.Equals(checkState.Check.Properties.Name, checkName, StringComparison.OrdinalIgnoreCase)) {
                        HealthZ.FillCheckResponse(response, checkState);
                        return true;
                    }
                }
            }
            Log.Debug("Cannot find check named {CheckName}", checkName);
        }

        Log.Debug("URL at {Path} not found", path);
        response.StatusCode = (int)HttpStatusCode.NotFound;
        return false;
    }

}
