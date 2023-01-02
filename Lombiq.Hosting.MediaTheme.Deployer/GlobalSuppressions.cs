using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Globalization",
    "CA1303:Do not pass literals as localized parameters",
    Justification = "It's a developer console application, it doesn't need localization.",
    Scope = "module")]
