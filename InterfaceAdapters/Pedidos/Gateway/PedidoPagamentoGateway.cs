using CasosDeUso.Pedidos.Comandos;
using CasosDeUso.Pedidos.Enums;
using CasosDeUso.Pedidos.Interfaces.Gateway;
using Entidades.Pedidos;
using Entidades.Util;
using InterfaceAdapters.Bases.Gateway;
using InterfaceAdapters.MercadoPago.Gateway.Interfaces;
using NHibernate;
namespace InterfaceAdapters.Pedidos.Gateway
{

    public class PedidoPagamentoGateway(ISession session, IMercadoPagoGateway mercadoPagoGateway) : BaseGateway<PedidoPagamento>(session), IPedidoPagamentoGateway
    {
        public async Task<string> RealizarPagamento(PedidoComando pedidoComando, MetodoPagamentoEnum metodoPagamento)
        {
            return metodoPagamento switch
            {
                MetodoPagamentoEnum.MercadoPago => await mercadoPagoGateway.GerarQrCode(pedidoComando),
                MetodoPagamentoEnum.MercadoPagoMock => await mercadoPagoGateway.GerarQrCodeMock(pedidoComando),
                _ => throw new RegraNegocioException("Pagamento não implementado"),
            };
        }

        public async Task<string> ObterPagamento(string codigoPagamento, MetodoPagamentoEnum metodoPagamento)
        {
            return metodoPagamento switch
            {
                MetodoPagamentoEnum.MercadoPago => await mercadoPagoGateway.BuscarOrdemPagamento(codigoPagamento),
                MetodoPagamentoEnum.MercadoPagoMock => await mercadoPagoGateway.BuscarOrdemPagamentoMock(codigoPagamento),
                _ => throw new RegraNegocioException("Pagamento não implementado")
            };
        }
    }

}
