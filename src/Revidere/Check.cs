namespace Revidere;

using System;
using System.Text.RegularExpressions;

internal partial class Check {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="name">Name of target.</param>
    /// <param name="title">Title of target.</param>
    /// <param name="target">Target URL</param>
    /// <param name="profile">Check profile.</param>
    /// <exception cref="ArgumentNullException">Name cannot be null. -or- Target URI cannot be null. -or- Profile cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Name cannot be can consist only of lowecase alphanumeric, numbers, dash (-), and underscore (_) characters.</exception>
    internal Check(string? name, string title, Uri target, CheckProfile profile) {
        if ((name != null) && !NameRegex.IsMatch(name)) { throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be can consist only of lowecase alphanumeric, numbers, dash (-), and underscore (_) characters."); }
        if (target == null) { throw new ArgumentNullException(nameof(target), "Target URI cannot be null."); }

        Name = name;
        Title = title ?? name ?? string.Empty;
        Target = target;
        CheckProfile = profile ?? throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");

        Checker = target.Scheme switch {
            "dummy" => new DummyChecker(),
            "http" => new HttpChecker(target),
            "https" => new HttpChecker(target),
            "ping" => new PingChecker(target),
            "random" => new RandomChecker(target),
            _ => throw new NotSupportedException($"Scheme '{target.Scheme}' is not supported."),
        };
    }


    public string? Name { get; }
    public string Title { get; }
    public Uri Target { get; }
    public CheckProfile CheckProfile { get; }


    /// <summary>
    /// Gets the checker for this target.
    /// </summary>
    public IChecker Checker { get; }


    private static readonly Regex NameRegex = MyRegex();

    [GeneratedRegex(@"^[a-z0-9-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MyRegex();


    override public string ToString() => Target.ToString();
}
