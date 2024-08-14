namespace Revidere;

using System;
using System.Net.NetworkInformation;
using System.Threading;
using Serilog;

internal sealed class PingCheck : Check {

    internal PingCheck(string kind, string target, string? title, string? name, bool isVisible, CheckProfile profile)
        : base(kind, target, title, name, isVisible, profile) {
    }

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        try {
            var pingSender = new Ping();
            PingReply reply = pingSender.Send(Target, (int)CheckProfile.Timeout.TotalMilliseconds);
            Log.Verbose("Ping {Host} status: {Status}", Target, reply.Status);
            return (reply.Status == IPStatus.Success);
        } catch (Exception ex) {
            Log.Verbose("Ping {Host} error: {Exception}", Target, ex);
            return false;
        }
    }

}
