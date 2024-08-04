namespace Revidere;

using System;
using System.Threading;

internal class DummyChecker : IChecker {

    public DummyChecker() {
    }

    public bool CheckIsHealthy(CancellationToken cancellationToken, TimeSpan timeout) {
        return true;
    }

}
