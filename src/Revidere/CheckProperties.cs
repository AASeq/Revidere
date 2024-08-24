using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;

namespace Revidere;

internal partial record CheckProperties {

    public CheckProperties(string kind, string? target, string? title, string? name, bool isVisible, bool isBreak, int? percentThreshold, CheckProfile profile) {
        kind = kind.Trim();
        name = string.IsNullOrEmpty(name) ? null : name.Trim();  // if empty, assume null

        Kind = kind.ToUpperInvariant();  // normalize to upper-case
        Target = target?.Trim() ?? string.Empty;
        Name = name;
        Title = title ?? Name ?? kind;
        IsVisible = isVisible;
        IsBreak = isBreak;
        PercentThreshold = percentThreshold;
        CheckProfile = profile ?? CheckProfile.Default;

        // fixups
        if ((Name != null) && !NameRegex().IsMatch(Name)) {
            Log.Warning("Check '{Name}' has invalid name; ignoring", Name);
            Name = null;
        }
    }


    /// <summary>
    /// Gets check index.
    /// </summary>
    public int Index { get; }

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
    /// Gets percentage of success necessary for being healthy.
    /// </summary>
    public int? PercentThreshold { get; }

    /// <summary>
    /// Gets check profile.
    /// </summary>
    public CheckProfile CheckProfile { get; }


    /// <summary>
    /// Returns string representation.
    /// </summary>
    public override string ToString() {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(Name)) { sb.Append(Name); } else { sb.Append('#').Append(Index.ToString(CultureInfo.InvariantCulture)); }

        sb.Append(' ');
        sb.Append(Kind);

        if (!string.IsNullOrEmpty(Target)) { sb.Append(' ').Append(Target); }

        return sb.ToString();
    }


    [GeneratedRegex(@"^[a-z0-9-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex NameRegex();

}