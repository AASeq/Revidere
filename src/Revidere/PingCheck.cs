namespace Revidere;

using System;
using System.Net.NetworkInformation;
using System.Threading;
using Serilog;

internal sealed class PingCheck : Check {

    internal PingCheck(CommonCheckProperties commonProperties)
        : base(commonProperties) {

        Host = commonProperties.Target;
    }

    private readonly string Host;


    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        try {
            var pingSender = new Ping();
            PingReply reply = pingSender.Send(Host, (int)CheckProfile.Timeout.TotalMilliseconds);
            Log.Verbose("Ping {Host} status: {Status}", Host, reply.Status);
            return (reply.Status == IPStatus.Success);
        } catch (Exception ex) {
            Log.Verbose("Ping {Host} error: {Exception}", Host, ex);
            return false;
        }
    }

}
