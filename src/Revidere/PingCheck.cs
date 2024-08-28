namespace Revidere;

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        var sw = Stopwatch.StartNew();
        try {
            var pingSender = new Ping();
            PingReply reply = pingSender.Send(Host, (int)Properties.CheckProfile.Timeout.TotalMilliseconds);
            var isHealthy = (reply.Status == IPStatus.Success);
            Log.Verbose("{Check} status: {Status} ({Code}; {Duration}ms)", this, isHealthy ? "Healthy" : "Unhealthy", reply.Status, sw.ElapsedMilliseconds);
            return isHealthy;
        } catch (Exception ex) {
            while(ex.InnerException != null) { ex = ex.InnerException; }  // unwrap all exceptions
            Log.Verbose("{Check} status: {Status} ({Error}; {Duration}ms)", this, "Unhealthy", ex.Message, sw.ElapsedMilliseconds);
            return false;
        }
    }

}
