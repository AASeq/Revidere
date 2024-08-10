namespace Revidere;

using System;
using System.Net.NetworkInformation;
using System.Threading;
using Serilog;

internal class PingChecker : IChecker {

    public PingChecker(string hostName) {
        HostName = hostName;
    }

    private readonly string HostName;

    public bool CheckIsHealthy(CancellationToken cancellationToken, TimeSpan timeout) {
        try {
            var pingSender = new Ping();
            PingReply reply = pingSender.Send(HostName, (int)timeout.TotalMilliseconds);
            Log.Verbose("Ping {Host} status: {Status}", HostName, reply.Status);
            return (reply.Status == IPStatus.Success);
        } catch (Exception ex) {
            Log.Verbose("Ping {Host} error: {Exception}", HostName, ex);
            return false;
        }
    }

}
