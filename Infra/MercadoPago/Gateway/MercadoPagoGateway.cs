using CasosDeUso.Pedidos.Comandos;
using Infra.MercadoPago.Presenters;
using InterfaceAdapters.MercadoPago.Gateway.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Infra.MercadoPago.Gateway
{
    public class MercadoPagoGateway(IHttpClientFactory HttpClientFactory, IConfiguration configuration) : IMercadoPagoGateway
    {
        private readonly string UserId = configuration["MercadoPago.API:UserId"];
        private readonly string PosId = configuration["MercadoPago.API:PosId"];
        private readonly string AccessToken = configuration["MercadoPago.API:AccessToken"];
        private readonly string ClientId = configuration["MercadoPago.API:ClientId"];
        private readonly string ClientSecret = configuration["MercadoPago.API:Client_secret"];
        private readonly string ClientCredentials = configuration["MercadoPago.API:client_credentials"];
        private readonly string Gateway = configuration["MercadoPago.API:gateway"];

        public async Task<AutenticacaoResponse> Autenticar()
        {
            if (AccessToken == "")
            {
                var httpClient = HttpClientFactory.CreateClient("MercadoPago.API");

                var AutenticacaoRequest = new AutenticacaoRequest(ClientSecret, ClientId, ClientCredentials, "false");

                var content = JsonSerializer.Serialize(AutenticacaoRequest);

                var response = await httpClient.PostAsync("oauth/token", new StringContent(content, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();

                string retorno = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<AutenticacaoResponse>(retorno);
            }

            var autenticacaoResponse = new AutenticacaoResponse
            {
                access_token = AccessToken
            };

            return autenticacaoResponse;
        }

        public async Task<string> BuscarOrdemPagamento(string Codigo)
        {
            using var httpClient = HttpClientFactory.CreateClient("MercadoPago.API");
            {
                var accessToken = Autenticar().Result.access_token;

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await httpClient.GetAsync($"merchant_orders/{Codigo}");

                response.EnsureSuccessStatusCode();

                string retorno = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<OrdemPagagamentoMercadoPagoResponse>(retorno);

                return result.external_reference;
            }
        }

        public async Task<string> GerarQrCode(PedidoComando pedidoComando)
        {

            var pagamentoMercadoPagoRequest = new PedidoMercadoPagoRequest
            {
                external_reference = pedidoComando.Codigo.ToString(),
                title = $"MCKing - Pedido - {pedidoComando.Codigo}",
                description = $"Compra de lanche no MCKing - Pedido-{pedidoComando.Codigo}",
                items = [],
                //notification_url = $"{Gateway}/api/Pedidos/webhook/ConfirmarPagamento",
                total_amount = pedidoComando.PedidoCombo.Sum(l => l.Preco)
            };

            foreach (var pedidoCombo in pedidoComando.PedidoCombo)
            {
                pagamentoMercadoPagoRequest.items.Add(new PedidoItemMercadoPagoRequest
                {
                    sku_number = $"{pedidoCombo.Codigo}",
                    category = "Combo",
                    title = $"Combo-{pedidoCombo.Codigo}",
                    description = $"Combo-{pedidoCombo.Codigo}",
                    unit_price = pedidoCombo.Preco,
                    quantity = 1,
                    unit_measure = "unit",
                    total_amount = pedidoCombo.Preco
                });
            }

            using var httpClient = HttpClientFactory.CreateClient("MercadoPago.API");
            {
                var accessToken = Autenticar().Result.access_token;

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var content = JsonSerializer.Serialize(pagamentoMercadoPagoRequest);

                var response = await httpClient.PostAsync($"instore/orders/qr/seller/collectors/{UserId}/pos/{PosId}/qrs", new StringContent(content, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();

                string retorno = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<PedidoMercadoPagoResponse>(retorno);

                return result.qr_data;
            }
        }

        public async Task<string> BuscarOrdemPagamentoMock(string Codigo)
        {
            return Codigo;
        }

        public async Task<string> GerarQrCodeMock(PedidoComando pedidoComando)
        {
            return "00020101021226900014BR.GOV.BCB.PIX2568pix-qr.mercadopago.com/instore/p/v2/cddbd6a056c9401eb90cb45c2418030543540016com.mercadolibre0130https://mpago.la/pos/1083218685204000053039865802BR5915renan eustaquio6009SAO PAULO62070503***63041E31";
        }
    }
}