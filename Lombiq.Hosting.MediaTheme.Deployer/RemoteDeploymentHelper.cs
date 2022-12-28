namespace Lombiq.Hosting.MediaTheme.Deployer;

internal static class RemoteDeploymentHelper
{
    private static readonly HttpClient _httpClient = new();

    public static async Task DeployAsync(CommandLineOptions options, string deploymentPackagePath)
    {
        if (string.IsNullOrEmpty(options.RemoteDeploymenClientName) || string.IsNullOrEmpty(options.RemoteDeploymenClientApiKey))
        {
            throw new InvalidOperationException(
                "When doing a Remote Deployment, both the Client Name and Client API Key should be provided.");
        }

        // The below code is largely taken from Orchard's ExportRemoteInstanceController.
        HttpResponseMessage response;

        try
        {
            // It's disposed via requestContent.
#pragma warning disable CA2000 // Dispose objects before losing scope
            using var requestContent = new MultipartFormDataContent
            {
                {
                    new StreamContent(
                    new FileStream(
                        deploymentPackagePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite,
                        1,
                        FileOptions.Asynchronous | FileOptions.SequentialScan)
                ),
                    "Content",
                    Path.GetFileName(deploymentPackagePath)
                },
                { new StringContent(options.RemoteDeploymenClientName), "ClientName" },
                { new StringContent(options.RemoteDeploymenClientApiKey), "ApiKey" },
            };
#pragma warning restore CA2000 // Dispose objects before losing scope

            response = await _httpClient.PostAsync(options.RemoteDeploymenUrl, requestContent);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Remote deployment to {0} succeeded.", options.RemoteDeploymenUrl);
            }
            else
            {
                throw new HttpRequestException(
                    $"Remote deployment to {options.RemoteDeploymenUrl} failed with the HTTP code " +
                    $"{response.StatusCode} and message \"{response.RequestMessage}\".");
            }
        }
        finally
        {
            // Not deleting only for debugging.
            ////File.Delete(deploymentPackagePath);
        }
    }
}
