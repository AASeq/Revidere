namespace Revidere;

using System.Threading;

internal sealed class DummyCheck : Check {

    internal DummyCheck(string kind, string target, string? title, string? name, bool isVisible, bool isBreak, CheckProfile profile)
        : base(kind, target, title, name, isVisible, isBreak, profile) {
    }

    public override bool CheckIsHealthy(CancellationToken cancellationToken) {
        return true;
    }

}
