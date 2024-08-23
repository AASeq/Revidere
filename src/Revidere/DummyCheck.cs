namespace Revidere;

using System.Collections.Generic;
using System.Threading;
using Serilog;

internal sealed class DummyCheck : Check {

    internal DummyCheck(CheckProperties checkProperties)
        : base(checkProperties) {
    }

    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        Log.Verbose("DUMMY {Target} status: {Status}", Target, "Healthy");
        return true;
    }

}
