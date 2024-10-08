namespace Tests;

using Revidere;

internal static class Helpers {

    public static CheckProperties GetProperties(string kind, string target) {
        return new CheckProperties(
            kind,                 // Kind
            target,               // Target
            null,                 // Title
            null,                 // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            CheckProfile.Default  // Profile
        );
    }

    public static CheckProperties GetProperties(string kind, string target, int percentThreshold) {
        return new CheckProperties(
            kind,                 // Kind
            target,               // Target
            null,                 // Title
            null,                 // Name
            false,                // IsVisible
            false,                // IsBreak
            percentThreshold,     // PercentThreshold
            CheckProfile.Default  // Profile
        );
    }

   public static CheckProperties GetProperties(string kind, string target, string name) {
        return new CheckProperties(
            kind,                 // Kind
            target,               // Target
            null,                 // Title
            name,                 // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            CheckProfile.Default  // Profile
        );
    }

    public static CheckProperties GetProperties(string kind, string target, CheckProfile profile) {
        return new CheckProperties(
            kind,                 // Kind
            target,               // Target
            null,                 // Title
            null,                 // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            profile               // Profile
        );
    }

}
