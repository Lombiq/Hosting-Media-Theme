using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Lombiq Hosting - Media Theme Bridge",
    Author = "Lombiq Technologies",
    Website = "https://github.com/Lombiq/Hosting-Tenants",
    Version = "1.0.0",
    Description = "Ability to host a theme from the Media Library. Used along with the Media Theme Orchard Core theme.",
    Category = "Hosting",
    Dependencies = new[] { "OrchardCore.Media", "OrchardCore.Users", "OrchardCore.Deployment" }
)]
