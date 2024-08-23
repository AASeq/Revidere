namespace Revidere;

using System;
using System.Threading;

internal sealed class RandomCheck : Check {

    internal RandomCheck(CommonCheckProperties commonProperties)
        : base(commonProperties) {
        var seed = 0;
        foreach (var ch in commonProperties.Target) {
            seed = (seed << 5) ^ (seed << 3) ^ (seed << 1) ^ ch;
        }
        Random = (seed == 0) ? new Random((int)DateTime.Now.Ticks) : new Random(seed);
    }

    private readonly Random Random;


    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        return (Random.Next() & 0x01) != 0;
    }

}
