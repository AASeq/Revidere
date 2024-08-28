namespace Revidere;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Serilog;

internal sealed class TcpCheck : Check {

    internal TcpCheck(CheckProperties checkProperties)
        : base(checkProperties) {

        var targetParts = checkProperties.Target.Split(':', StringSplitOptions.TrimEntries);
        if (targetParts.Length == 2) {
            if (int.TryParse(targetParts[1], out var port) && (port is > 0 and < 65536)) {
                Host = targetParts[0];
                Port = port;
            } else {
                throw new InvalidOperationException($"Cannot parse '{targetParts[1]} as port number'");
            }
        } else {
            throw new InvalidOperationException("Cannot parse '{target} as hostname or IP address + port pair'");
        }
    }

    private readonly string Host;
    private readonly int Port;


    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        var sw = Stopwatch.StartNew();
        try {
            var timeoutCancelSource = new CancellationTokenSource(Properties.CheckProfile.Timeout);
            var linkedCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancelSource.Token);

            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(Host, Port, linkedCancelSource.Token);
            connectTask.ConfigureAwait(false).GetAwaiter().GetResult();

            Log.Verbose("{Check} status: {Status} ({Duration}ms)", this, "Healthy", sw.ElapsedMilliseconds);
            return true;
        } catch (OperationCanceledException) {
            Log.Verbose("{Check} status: {Status} ({Error}; ; {Duration}ms)", this, "Unhealthy", "Timeout", sw.ElapsedMilliseconds);
            return false;
        } catch (Exception ex) {
            Log.Verbose("{Check} status: {Status} ({Error}; {Duration}ms)", this, "Unhealthy", ex.Message, sw.ElapsedMilliseconds);
            return false;
        }
    }

}
