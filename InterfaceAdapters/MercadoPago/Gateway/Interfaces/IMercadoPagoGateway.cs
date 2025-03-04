using CasosDeUso.Pedidos.Comandos;

namespace InterfaceAdapters.MercadoPago.Gateway.Interfaces
{
    public interface IMercadoPagoGateway
    {
        Task<string> GerarQrCode(PedidoComando pedidoComando);
        Task<string> BuscarOrdemPagamento(string CodigoPagamento);
        Task<string> GerarQrCodeMock(PedidoComando pedidoComando);
        Task<string> BuscarOrdemPagamentoMock(string CodigoPagamento);
    }
}