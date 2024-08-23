namespace Revidere;

using System.Threading;

internal sealed class DummyCheck : Check {

    internal DummyCheck(CommonCheckProperties commonProperties)
        : base(commonProperties) {
    }

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        return true;
    }

}
