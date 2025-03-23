using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

public class CognitoService
{
    private readonly AmazonCognitoIdentityProviderClient _client;
    private readonly string _userPoolId;

    public CognitoService(string userPoolId)
    {
        _userPoolId = userPoolId;
        _client = new AmazonCognitoIdentityProviderClient();
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            string cpf = DecodeTokenAndGetCpf(token);


            var response = await _client.AdminGetUserAsync(new AdminGetUserRequest
            {
                UserPoolId = _userPoolId,
                Username = cpf
            });

            var storedToken = response.UserAttributes
                .FirstOrDefault(attr => attr.Name == "custom:jwtToken")?.Value;

            return storedToken == token;
        }
        catch
        {
            return false;
        }
    }





    // Método para decodificar o token e obter o valor do CPF
    public static string DecodeTokenAndGetCpf(string jwtToken)
    {
        // Dividir o token em suas partes (Header, Payload, Signature)
        string[] tokenParts = jwtToken.Split('.');
        if (tokenParts.Length != 3)
        {
            throw new ArgumentException("Token inválido ou malformado.");
        }

        // Decodificar o Payload (segunda parte do token)
        string payload = tokenParts[1];
        string decodedPayload = Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(payload)));

        // Analisar o JSON do Payload
        var jsonDoc = JsonDocument.Parse(decodedPayload);
        var root = jsonDoc.RootElement;

        // Extrair o atributo "cpf"
        string cpf = root.GetProperty("cpf").GetString();

        return cpf;
    }

    // Método para ajustar o padding do Base64 (caso necessário)
    private static string PadBase64(string base64)
    {
        int padding = 4 - (base64.Length % 4);
        if (padding > 0 && padding < 4)
        {
            base64 += new string('=', padding);
        }
        return base64;
    }



}
