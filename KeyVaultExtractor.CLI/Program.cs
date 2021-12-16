using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace KeyVaultExtractor.CLI;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length < 1) throw new Exception("Name of the KeyVault should be specified");
        
        // Get a cancellation token
        CancellationToken cancellationToken = default;

        // Get the KeyVault name and create a service URI for it
        var keyVaultName = args[0];
        var keyVaultUri = new UriBuilder("https", $"{keyVaultName}.vault.azure.net").Uri;
        
        // Use the default credentials to access KeyVault
        var credential = new DefaultAzureCredential();
        var secretsClient = new SecretClient(keyVaultUri, credential);

        // Iterate over each secret and get it's value
        await foreach (var secretProperty in secretsClient.GetPropertiesOfSecretsAsync(cancellationToken))
        {
            var secret = await secretsClient.GetSecretAsync(secretProperty.Name, cancellationToken: cancellationToken);
            
            // Write out the secret name and value
            Console.WriteLine($"{secret.Value.Name}: {secret.Value.Value}");
        }
    }
}
