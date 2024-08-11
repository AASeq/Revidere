namespace Revidere;

using System;
using System.Threading;

internal class RandomCheck : Check {

    public RandomCheck(string kind, string target, string title, string? name, CheckProfile profile)
        : base(kind, target, title, name, profile) {
        var seed = 0;
        foreach (var ch in target) {
            seed = (seed << 5) ^ (seed << 3) ^ (seed << 1) ^ ch;
        }
        Random = (seed == 0) ? new Random((int)DateTime.Now.Ticks) : new Random(seed);
    }

    private readonly Random Random;

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        return (Random.Next() & 0x01) != 0;
    }

}
