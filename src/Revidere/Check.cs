namespace Revidere;

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Serilog;

internal abstract partial class Check {

    private protected Check(string kind, string target, string? title, string? name, bool isVisible, bool isBreak, CheckProfile profile) {
        Kind = kind.ToUpperInvariant();  // normalize to upper-case
        Target = target;
        Title = title ?? name ?? kind;
        Name = name;
        IsVisible = isVisible;
        IsBreak = isBreak;
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
    /// Gets if check is to be made visible.
    /// </summary>
    public bool IsVisible { get; }

    /// <summary>
    /// Gets if check is the last in line.
    /// </summary>
    public bool IsBreak { get; }

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
    internal static Check? FromConfigData(string kind, string target, string? title, string? name, bool isVisible, bool isBreak, CheckProfile profile) {
        if (kind == null) { throw new ArgumentNullException(nameof(kind), "Target URI cannot be null."); }
        if (target == null) { throw new ArgumentNullException(nameof(target), "Target URI cannot be null."); }
        if (name != null) {
            name = name.Trim();
            if (!NameRegex().IsMatch(name)) { throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be can consist only of lowecase alphanumeric, numbers, dash (-), and underscore (_) characters."); }
        }

        kind = kind.Trim();
        target = target.Trim();
        title = title?.Trim() ?? name ?? kind;

        if (kind.Equals("dummy", StringComparison.OrdinalIgnoreCase)) {
            if (!string.IsNullOrEmpty(target)) { Log.Information("Target is not used when kind is dummy"); }
            return new DummyCheck(kind, target, title, name, isVisible, isBreak, profile);
        } else if (kind.Equals("get", StringComparison.OrdinalIgnoreCase)
            || kind.Equals("head", StringComparison.OrdinalIgnoreCase)
            || kind.Equals("head", StringComparison.OrdinalIgnoreCase)
            || kind.Equals("post", StringComparison.OrdinalIgnoreCase)
            || kind.Equals("put", StringComparison.OrdinalIgnoreCase)
            || kind.Equals("delete", StringComparison.OrdinalIgnoreCase)) {
            if (Uri.TryCreate(target, UriKind.Absolute, out var uri)) {
                return new HttpCheck(kind, uri.ToString(), title, name, isVisible, isBreak, profile);
            } else {
                if (!string.IsNullOrEmpty(target)) { Log.Warning($"Cannot parse target URL '{target}'"); }
                return null;
            }
        } else if (kind.Equals("ping", StringComparison.OrdinalIgnoreCase)) {
            if (IPAddressRegex().IsMatch(target) || HostRegex().IsMatch(target)) {
                return new PingCheck(kind, target, title, name, isVisible, isBreak, profile);
            } else {
                if (!string.IsNullOrEmpty(target)) { Log.Warning($"Cannot parse '{target} as hostname or IP address'"); }
                return null;
            }
        } else if (kind.Equals("random", StringComparison.OrdinalIgnoreCase)) {
            return new RandomCheck(kind, target, title, name, isVisible, isBreak, profile);
        } else if (kind.Equals("tcp", StringComparison.OrdinalIgnoreCase)) {
            var targetParts = target.Split(':', StringSplitOptions.TrimEntries);
            if (targetParts.Length == 2) {
                if (IPAddressRegex().IsMatch(targetParts[0]) || HostRegex().IsMatch(targetParts[0])) {
                    if (int.TryParse(targetParts[1], out var port) && (port is > 0 and < 65536)) {
                        return new TcpCheck(kind, targetParts[0] + ":" + port.ToString(CultureInfo.InvariantCulture), title, name, isVisible, isBreak, profile);
                    } else {
                        Log.Warning($"Cannot parse '{targetParts[1]} as port number'");
                        return null;
                    }
                } else {
                    if (!string.IsNullOrEmpty(target)) { Log.Warning($"Cannot parse '{targetParts[0]} as hostname or IP address'"); }
                    return null;
                }
            } else {
                Log.Warning($"Cannot parse '{target} as hostname or IP address + port pair'");
                return null;
            }
        } else {
            if (!string.IsNullOrEmpty(target)) { Log.Warning($"Unrecognized check kind '{kind}'"); }
            return null;
        }
    }


    [GeneratedRegex(@"^[a-z0-9-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex NameRegex();

    [GeneratedRegex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex IPAddressRegex();

    [GeneratedRegex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex HostRegex();


    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    override public string ToString() {
        if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Target)) {
            return $"{Name} {Kind} {Target}";
        } else if (!string.IsNullOrEmpty(Name)) {
            return $"{Name} {Kind}";
        } else if (!string.IsNullOrEmpty(Target)) {
            return $"{Kind} {Target}";
        } else {
            return $"{Kind}";
        }
    }
}
