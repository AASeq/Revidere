namespace Revidere;

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Serilog;

internal sealed class PingCheck : Check {

    internal PingCheck(CheckProperties checkProperties)
        : base(checkProperties) {

        Host = checkProperties.Target;
    }

    private readonly string Host;


    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
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
