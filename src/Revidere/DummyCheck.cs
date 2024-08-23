namespace Revidere;

using System.Threading;

internal sealed class DummyCheck : Check {

    internal DummyCheck(CheckProperties checkProperties)
        : base(checkProperties) {
    }

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        return true;
    }

}
