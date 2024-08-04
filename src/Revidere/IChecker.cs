namespace Revidere;

using System;
using System.Threading;

internal interface IChecker {

    public bool CheckIsHealthy(CancellationToken cancellationToken, TimeSpan timeout);

}
