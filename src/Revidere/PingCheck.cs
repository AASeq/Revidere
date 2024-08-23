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
            PingReply reply = pingSender.Send(Host, (int)Properties.CheckProfile.Timeout.TotalMilliseconds);
            var isHealthy = (reply.Status == IPStatus.Success);
            Log.Verbose("{Check} status: {Status} ({Code})", this, isHealthy ? "Healthy" : "Unhealthy", reply.Status);
            return isHealthy;
        } catch (Exception ex) {
            Log.Verbose("{Check} status: {Status} ({Error})", this, "Unhealthy", ex.Message);
            return false;
        }
    }

}
