using OrchardCore.DisplayManagement.Manifest;
using static Lombiq.Hosting.MediaTheme.Bridge.Constants.FeatureNames;

[assembly: Theme(
    Name = "Lombiq Hosting - Media Theme",
    Author = "Lombiq Technologies",
    Website = "https://github.com/Lombiq/Hosting-Tenants",
    Version = "1.0.0",
    Description = "Ability to host a theme from the Media Library.",
    Category = "Hosting",
    Dependencies = new[] { MediaThemeBridge }
)]
