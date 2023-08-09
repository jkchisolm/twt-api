using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;
using TwitterAPI.Interfaces;

namespace TwitterAPI.Helpers;

public class SecretHelper : ISecretHelper
{
    private readonly IConfiguration _configuration;
    private readonly SecretClient _secretClient;

    public SecretHelper(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretClient = GetSecretClient();
    }
    public SecretClient GetSecretClient()
    {
        return new SecretClient(new Uri(_configuration["KeyVaultConfiguration:Vault"]),
            new ClientSecretCredential(tenantId: _configuration["KeyVaultConfiguration:TenantId"],
                clientId: _configuration["KeyVaultConfiguration:ClientId"],
                clientSecret: _configuration["KeyVaultConfiguration:ClientSecret"]));
    }
    
    public string GetSecret(string secretName)
    {
        return _secretClient.GetSecret(secretName).Value.Value;
    }
    
    public async Task<string> GetSecretAsync(string secretName)
    {
        return (await _secretClient.GetSecretAsync(secretName)).Value.Value;
    }
}