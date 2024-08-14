namespace Revidere;

using System;
using System.Threading;

internal sealed class RandomCheck : Check {

    internal RandomCheck(string kind, string target, string? title, string? name, bool isVisible, bool isBreak, CheckProfile profile)
        : base(kind, target, title, name, isVisible, isBreak, profile) {
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
