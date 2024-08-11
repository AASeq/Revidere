namespace Revidere;

using System;
using System.Threading;

internal class DummyCheck : Check {

    public DummyCheck(string kind, string target, string title, string? name, CheckProfile profile)
        : base(kind, target, title, name, profile) {
    }

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        return true;
    }

}
