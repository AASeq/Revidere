namespace Revidere;

using System;
using System.Net.NetworkInformation;
using System.Threading;
using Serilog;

internal class PingChecker : IChecker {

    public PingChecker(Uri target) {
        Host = target.Host;
    }

    private readonly string Host;

    public bool CheckIsHealthy(CancellationToken cancellationToken, TimeSpan timeout) {
        try {
            var pingSender = new Ping();
            PingReply reply = pingSender.Send(Host, (int)timeout.TotalMilliseconds);
            Log.Verbose("Ping {Host} status: {Status}", Host, reply.Status);
            return (reply.Status == IPStatus.Success);
        } catch (Exception ex) {
            Log.Verbose("Ping {Host} error: {Exception}", Host, ex);
            return false;
        }
    }

}
