namespace Revidere;

using System.Threading;

internal sealed class DummyCheck : Check {

    internal DummyCheck(string kind, string target, string title, string? name, CheckProfile profile)
        : base(kind, target, title, name, profile) {
    }

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        return true;
    }

}
