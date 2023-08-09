using Azure.Security.KeyVault.Secrets;

namespace TwitterAPI.Interfaces;

public interface ISecretHelper
{
    SecretClient GetSecretClient();
    string GetSecret(string secretName);
    Task<string> GetSecretAsync(string secretName);
}