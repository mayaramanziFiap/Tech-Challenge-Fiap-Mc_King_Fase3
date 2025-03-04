namespace Infra.MercadoPago.Presenters
{
    public class AutenticacaoRequest(string client_secret, string client_id, string grant_type, string test_token)
    {
        public string client_secret { get; set; } = client_secret;
        public string client_id { get; set; } = client_id;
        public string grant_type { get; set; } = grant_type;
        public string test_token { get; set; } = test_token;
    }
}