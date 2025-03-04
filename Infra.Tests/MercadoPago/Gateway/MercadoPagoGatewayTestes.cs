using CasosDeUso.Pedidos.Comandos;
using FluentAssertions;
using Infra.MercadoPago.Gateway;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Infra.Tests.MercadoPago
{
    public class MercadoPagoGatewayTests
    {
        private readonly MercadoPagoGateway _sut;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MercadoPagoGatewayTests()
        {
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _configuration = Substitute.For<IConfiguration>();

            _configuration["MercadoPago.API:AccessToken"].Returns("");
            _configuration["MercadoPago.API:ClientId"].Returns("test_client_id");
            _configuration["MercadoPago.API:ClientSecret"].Returns("test_client_secret");
            _configuration["MercadoPago.API:client_credentials"].Returns("test_credentials");
            _configuration["MercadoPago.API:UserId"].Returns("test_user_id");
            _configuration["MercadoPago.API:PosId"].Returns("test_pos_id");
            _configuration["MercadoPago.API:gateway"].Returns("https://api.test.com");

            _sut = new MercadoPagoGateway(_httpClientFactory, _configuration);
        }

        [Fact]
        public async Task DeveAutenticarERetornarAccessToken_QuandoAccessTokenEstaVazio()
        {
            var respostaMock = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { access_token = "token_teste" }), Encoding.UTF8, "application/json")
            };

            var httpClient = new HttpClient(new ManipuladorHttpMock(respostaMock))
            {
                BaseAddress = new Uri("https://api.test.com")
            };
            _httpClientFactory.CreateClient("MercadoPago.API").Returns(httpClient);

            var resultado = await _sut.Autenticar();

            resultado.access_token.Should().Be("token_teste");
        }


        [Fact]
        public async Task DeveBuscarOrdemDePagamentoERetornarReferenciaExterna_QuandoCodigoForValido()
        {
            var respostaMock = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { external_reference = "referencia_teste" }), Encoding.UTF8, "application/json")
            };

            var httpClient = new HttpClient(new ManipuladorHttpMock(respostaMock))
            {
                BaseAddress = new Uri("https://api.test.com")
            };
            _httpClientFactory.CreateClient("MercadoPago.API").Returns(httpClient);

            var resultado = await _sut.BuscarOrdemPagamento("123");

            resultado.Should().Be("referencia_teste");
        }

        [Fact]
        public async Task DeveGerarQrCodeERetornarDados_QuandoPedidoComandoForValido()
        {
            var pedidoComando = new PedidoComando
            {
                Codigo = 123,
                PedidoStatus = [],
                PedidoCombo = []
            };

            var respostaMock = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { qr_data = "dados_qr_code_teste" }), Encoding.UTF8, "application/json")
            };

            var httpClient = new HttpClient(new ManipuladorHttpMock(respostaMock))
            {
                BaseAddress = new Uri("https://api.test.com") // BaseAddress configurado
            };
            _httpClientFactory.CreateClient("MercadoPago.API").Returns(httpClient);

            var resultado = await _sut.GerarQrCode(pedidoComando);

            resultado.Should().Be("dados_qr_code_teste");
        }

        [Fact]
        public async Task DeveLancarExcecao_QuandoGerarQrCodeFalharNaApi()
        {
            var pedidoComando = new PedidoComando
            {
                Codigo = 123,
                PedidoStatus = [],
                PedidoCombo = []
            };

            var respostaMock = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            var httpClient = new HttpClient(new ManipuladorHttpMock(respostaMock))
            {
                BaseAddress = new Uri("https://api.test.com") // BaseAddress configurado
            };
            _httpClientFactory.CreateClient("MercadoPago.API").Returns(httpClient);

            await _sut.Invoking(async x => await x.GerarQrCode(pedidoComando))
                .Should().ThrowAsync<HttpRequestException>();
        }

    }

    public class ManipuladorHttpMock : HttpMessageHandler
    {
        private readonly HttpResponseMessage _resposta;

        public ManipuladorHttpMock(HttpResponseMessage resposta)
        {
            _resposta = resposta;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requisicao, CancellationToken cancelamento)
        {
            return await Task.FromResult(_resposta);
        }
    }
}
