namespace Revidere;

using System;
using System.Text.RegularExpressions;

internal partial class Target {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="name">Name of target.</param>
    /// <param name="title">Title of target.</param>
    /// <param name="targetUri">Target URL</param>
    /// <param name="profile">Check profile.</param>
    /// <exception cref="ArgumentNullException">Name cannot be null. -or- Target URI cannot be null. -or- Profile cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Name cannot be can consist only of lowecase alphanumeric, numbers, dash (-), and underscore (_) characters.</exception>
    internal Target(string? name, string title, Uri targetUri, CheckProfile profile) {
        if ((name != null) && !NameRegex.IsMatch(name)) { throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be can consist only of lowecase alphanumeric, numbers, dash (-), and underscore (_) characters."); }
        if (targetUri == null) { throw new ArgumentNullException(nameof(targetUri), "Target URI cannot be null."); }

        Name = name;
        Title = title ?? name ?? string.Empty;
        TargetUri = targetUri;
        CheckProfile = profile ?? throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");

        Checker = targetUri.Scheme switch {
            "dummy" => new DummyChecker(),
            "http" => new HttpChecker(targetUri),
            "https" => new HttpChecker(targetUri),
            "ping" => new PingChecker(targetUri),
            "random" => new RandomChecker(targetUri),
            _ => throw new NotSupportedException($"Scheme '{targetUri.Scheme}' is not supported."),
        };
    }


    public string? Name { get; }
    public string Title { get; }
    public Uri TargetUri { get; }
    public CheckProfile CheckProfile { get; }


    /// <summary>
    /// Gets the checker for this target.
    /// </summary>
    public IChecker Checker { get; }


    private static readonly Regex NameRegex = MyRegex();

    [GeneratedRegex(@"^[a-z0-9-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MyRegex();


    override public string ToString() => TargetUri.ToString();
}
