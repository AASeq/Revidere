namespace Revidere;

using System;

internal sealed class CheckProfile {

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="period">How often to check the target.</param>
    /// <param name="timeout">How long to wait for the target to respond.</param>
    /// <param name="successCount">How many times in a row the target must be successful to be considered healthy.</param>
    /// <param name="failureCount">How many times in a row the target must fail to be considered unhealthy.</param>
    /// <exception cref="ArgumentOutOfRangeException">Period cannot be shorter than 1 second nor longer than 10 minutes. -or- Timeout cannot be shorter than 100 milliseconds nor longer than 1 minute. -or- Count must be between 1 and 10.</exception>
    public CheckProfile(int successCount, int failureCount) {
        if (successCount is < 1 or > 10) { throw new ArgumentOutOfRangeException(nameof(successCount), "Count must be between 1 and 10."); }
        if (failureCount is < 1 or > 10) { throw new ArgumentOutOfRangeException(nameof(failureCount), "Count must be between 1 and 10."); }

        Period = Default.Period;
        Timeout = Default.Timeout;
        SuccessCount = successCount;
        FailureCount = failureCount;
    }

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="period">How often to check the target.</param>
    /// <param name="timeout">How long to wait for the target to respond.</param>
    /// <param name="successCount">How many times in a row the target must be successful to be considered healthy.</param>
    /// <param name="failureCount">How many times in a row the target must fail to be considered unhealthy.</param>
    /// <exception cref="ArgumentOutOfRangeException">Period cannot be shorter than 1 second nor longer than 10 minutes. -or- Timeout cannot be shorter than 10 milliseconds nor longer than 10 seconds. -or- Count must be between 1 and 10.</exception>
    public CheckProfile(TimeSpan period, TimeSpan timeout, int successCount, int failureCount) {
        if (period.TotalSeconds is < 1 or > 600) { throw new ArgumentOutOfRangeException(nameof(period), "Period cannot be shorter than 1 second nor longer than 10 minutes."); }
        if (timeout.TotalSeconds is < 0.01 or > 10) { throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout cannot be shorter than 10 milliseconds nor longer than 10 seconds."); }
        if (successCount is < 1 or > 10) { throw new ArgumentOutOfRangeException(nameof(successCount), "Count must be between 1 and 10."); }
        if (failureCount is < 1 or > 10) { throw new ArgumentOutOfRangeException(nameof(failureCount), "Count must be between 1 and 10."); }

        if (timeout.TotalSeconds > period.TotalSeconds) { timeout = period; }

        Period = period;
        Timeout = timeout;
        SuccessCount = successCount;
        FailureCount = failureCount;
    }


    /// <summary>
    /// Gets how often to check the target.
    /// </summary>
    public TimeSpan Period { get; }

    /// <summary>
    /// Gets how long to wait for the target to respond.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Gets how many times in a row the target must be successful to be considered healthy.
    /// </summary>
    public int SuccessCount { get; }

    /// <summary>
    /// Gets how many times in a row the target must fail to be considered unhealthy.
    /// </summary>
    public int FailureCount { get; }

    /// <summary>
    /// Gets the default check profile.
    /// </summary>
    public static CheckProfile Default { get; } = new CheckProfile(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5), 2, 2);

}
