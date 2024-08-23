namespace Revidere;

using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Serilog;

internal sealed class TcpCheck : Check {

    internal TcpCheck(string kind, string target, string? title, string? name, bool isVisible, bool isBreak, CheckProfile profile)
        : base(kind, target, title, name, isVisible, isBreak, profile) {
        var targetParts = target.Split(':', StringSplitOptions.TrimEntries);
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

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        try {
            var timeoutCancelSource = new CancellationTokenSource(CheckProfile.Timeout);
            var linkedCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancelSource.Token);

            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(Host, Port, linkedCancelSource.Token);
            connectTask.ConfigureAwait(false).GetAwaiter().GetResult();

            Log.Verbose("TCP {Host} status: {Status}", Target, "Fail");
            return true;
        } catch (OperationCanceledException) {
            Log.Verbose("TCP {Host} status: {Status}", Target, "Timeout");
            return false;
        } catch (Exception ex) {
            Log.Verbose("TCP {Host} error: {Exception}", Target, ex);
            return false;
        }
    }

}
