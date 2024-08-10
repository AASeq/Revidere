namespace Revidere;

using System;
using System.Threading;

internal class RandomChecker : IChecker {

    public RandomChecker(string target) {
        var seed = 0;
        foreach (var ch in target) {
            seed = (seed << 5) ^ (seed << 3) ^ (seed << 1) ^ ch;
        }
        _random = (seed == 0) ? new Random((int)DateTime.Now.Ticks) : new Random(seed);
    }

    private readonly Random _random;

    public bool CheckIsHealthy(CancellationToken cancellationToken, TimeSpan timeout) {
        return (_random.Next() & 0x01) != 0;
    }

}
