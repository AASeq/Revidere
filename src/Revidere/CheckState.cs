namespace Revidere;

using System;
using System.Collections.Generic;
using Serilog;

internal sealed class CheckState {

    internal CheckState(Check check) {
        if (check == null) { throw new ArgumentNullException(nameof(check), "Check cannot be null."); }

        Check = check;
        IsHealthy = null;
    }


    /// <summary>
    /// Gets target.
    /// </summary>
    public Check Check { get; }

    /// <summary>
    /// Gets if target is healthy
    /// </summary>
    public bool? IsHealthy { get; private set; }

    private readonly List<bool> _healthHistory = new();
    /// <summary>
    /// Gets check history.
    /// </summary>
    public IReadOnlyList<bool> HealthHistory => _healthHistory.AsReadOnly();


    /// <summary>
    /// Gets when the first update occurred.
    /// </summary>
    public DateTimeOffset? FirstUpdated { get; private set; }

    /// <summary>
    /// Gets when the last update occurred.
    /// </summary>
    public DateTimeOffset? LastUpdated { get; private set; }

    /// <summary>
    /// Gets when the last change occurred.
    /// </summary>
    public DateTimeOffset? LastChanged { get; private set; }


    /// <summary>
    /// Updates target state.
    /// </summary>
    /// <param name="isHealthy">Status.</param>
    public void UpdateCheck(bool isHealthy) {
        var timestamp = DateTimeOffset.Now;

        if (FirstUpdated == null) { FirstUpdated = timestamp; }
        LastUpdated = timestamp;

        _healthHistory.Add(isHealthy);
        while (_healthHistory.Count > 10) { _healthHistory.RemoveAt(0); }

        if (isHealthy != IsHealthy) {  // check only if status has changed
            var maxCount = isHealthy ? Check.CheckProfile.SuccessCount : Check.CheckProfile.FailureCount;
            if (_healthHistory.Count >= maxCount) {
                var anyNonMatching = false;
                for (var i = _healthHistory.Count - maxCount; i < _healthHistory.Count; i++) {
                    if (_healthHistory[i] != isHealthy) {
                        anyNonMatching = true;
                        break;
                    }
                }
                if (!anyNonMatching) {
                    IsHealthy = isHealthy;
                    Log.Debug("Changed state for {Check}: {Status} (after {Count} checks)", Check, isHealthy ? "healthy" : "unhealthy", maxCount);
                    LastChanged = timestamp;
                }
            }
        }

    }

}
