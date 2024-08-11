namespace Revidere;

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

internal abstract partial class Check {

    private protected Check(string kind, string target, string title, string? name, CheckProfile profile) {
        Kind = kind;
        Target = target;
        Title = title;
        Name = name;
        CheckProfile = profile ?? throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
    }


    /// <summary>
    /// Gets check kind.
    /// </summary>
    public string Kind { get; }

    /// <summary>
    /// Gets check target.
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// Get check display title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets check name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Gets check profile.
    /// </summary>
    public CheckProfile CheckProfile { get; }

    /// <summary>
    /// Performs a health check.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public abstract bool CheckIsHealthy(CancellationToken cancellationToken);


    /// <summary>
    /// Creates a new instance based on config data.
    /// </summary>
    /// <param name="kind">Kind of target.</param>
    /// <param name="target">Target URL</param>
    /// <param name="title">Title of target.</param>
    /// <param name="name">Name of target.</param>
    /// <param name="profile">Check profile.</param>
    /// <exception cref="ArgumentNullException">Name cannot be null. -or- Target URI cannot be null. -or- Profile cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Name cannot be can consist only of lowecase alphanumeric, numbers, dash (-), and underscore (_) characters.</exception>
    internal static Check FromConfigData(string kind, string target, string title, string? name, CheckProfile profile) {
        if (kind == null) { throw new ArgumentNullException(nameof(target), "Target URI cannot be null."); }
        if (target == null) { throw new ArgumentNullException(nameof(target), "Target URI cannot be null."); }
        if (title == null) { throw new ArgumentNullException(nameof(target), "Target URI cannot be null."); }
        if ((name != null) && !NameRegex.IsMatch(name)) { throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be can consist only of lowecase alphanumeric, numbers, dash (-), and underscore (_) characters."); }

        if (kind.Equals("dummy", StringComparison.OrdinalIgnoreCase)) {
            return new DummyCheck("dummy", target, title, name, profile);
        } else if (kind.Equals("get", StringComparison.OrdinalIgnoreCase)) {
            return new HttpCheck("get", new Uri(target).ToString(), title, name, profile);
        } else if (kind.Equals("ping", StringComparison.OrdinalIgnoreCase)) {
            return new PingCheck("ping", target, title, name, profile);
        } else if (kind.Equals("random", StringComparison.OrdinalIgnoreCase)) {
            return new RandomCheck("random", target, title, name, profile);
        } else {
            throw new NotSupportedException($"Kind '{kind}' is not supported.");
        }
    }


    private static readonly Regex NameRegex = MyRegex();

    [GeneratedRegex(@"^[a-z0-9-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MyRegex();

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    override public string ToString() {
        if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Target)) {
            return $"{Name} {Kind.ToUpper()} {Target}";
        } else if (!string.IsNullOrEmpty(Name)) {
            return $"{Name} {Kind.ToUpper()}";
        } else if (!string.IsNullOrEmpty(Target)) {
            return $"{Kind.ToUpper()} {Target}";
        } else {
            return $"{Kind.ToUpper()}";
        }
    }
}
