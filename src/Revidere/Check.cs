namespace Revidere;

using System;
using System.Text.RegularExpressions;

internal partial class Check {

    private Check(string kind, string target, string title, string? name, IChecker checker, CheckProfile profile) {
        Kind = kind;
        Target = target;
        Title = title;
        Name = name;
        Checker = checker;
        CheckProfile = profile ?? throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
    }


    public string Kind { get; }
    public string Target { get; }
    public string? Name { get; }
    public string Title { get; }

    /// <summary>
    /// Gets the checker for this target.
    /// </summary>
    public IChecker Checker { get; }

    public CheckProfile CheckProfile { get; }



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

        IChecker checker = kind switch {
            "dummy" => new DummyChecker(),
            "get" => new HttpChecker(new Uri(target)),
            "ping" => new PingChecker(target),
            "random" => new RandomChecker(target),
            _ => throw new NotSupportedException($"Kind '{kind}' is not supported."),
        };

        return new Check(kind, target, title, name, checker, profile);
    }


    private static readonly Regex NameRegex = MyRegex();

    [GeneratedRegex(@"^[a-z0-9-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MyRegex();


    override public string ToString() => Title.ToString();
}
