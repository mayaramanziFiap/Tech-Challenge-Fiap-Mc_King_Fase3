using Infra.MercadoPago.Presenters;
using InterfaceAdapters.Pedidos.Controllers.Interfaces;
using InterfaceAdapters.Pedidos.Enums;
using InterfaceAdapters.Pedidos.Presenters.Requests;
using InterfaceAdapters.Pedidos.Presenters.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Pedidos
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController(IPedidoController pedidoController) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IList<PedidoResponse>> Consultar()
        {
            return pedidoController.Consultar();
        }

        [HttpGet]
        [Route("{codigo}")]
        public ActionResult<PedidoResponse> Consultar(int codigo)
        {
            return pedidoController.Consultar(codigo);
        }

        [HttpGet]
        [Route("Status/{codigo}")]
        public ActionResult<PedidoStatusResponse> ConsultarStatus(int codigo)
        {
            return pedidoController.ConsultarStatus(codigo);
        }

        [HttpGet]
        [Route("Preparar")]
        public ActionResult<IList<PedidoCozinhaResponse>> ConsultarPedidoCozinha()
        {
            return pedidoController.ObterPedidosCozinha();
        }

        [HttpGet]
        [Route("Monitor")]
        public ActionResult<IList<PedidoStatusMonitorResponse>> ConsultarPedidoMonitor()
        {
            return pedidoController.ObterPedidosMonitor();
        }

        [HttpPost]
        public ActionResult<PedidoResponse> Inserir(PedidoRequest pedidoRequest)
        {

            return pedidoController.Inserir(pedidoRequest);
        }

        [HttpPut]
        [Route("Cancelar/{codigo}")]
        public ActionResult<PedidoResponse> Cancelar(int codigo)
        {
            return pedidoController.AlterarStatus(codigo, StatusPedido.Cancelado);
        }

        [HttpPut]
        [Route("AlterarStatus/{codigo}")]
        public ActionResult<PedidoResponse> AlterarStatus(int codigo, StatusPedido statusPedido)
        {
            return pedidoController.AlterarStatus(codigo, statusPedido);
        }

        [HttpPut]
        [Route("Finalizar/{codigo}")]
        public ActionResult<PedidoResponse> Finalizar(int codigo)
        {

            return pedidoController.AlterarStatus(codigo, StatusPedido.Finalizado);
        }

        [HttpPut]
        [Route("InserirCombo/{codigo}")]
        public ActionResult<PedidoResponse> InserirCombo(int codigo, PedidoComboRequest pedidoComboRequest)
        {
            return pedidoController.InserirCombo(codigo, pedidoComboRequest);
        }

        [HttpDelete]
        [Route("RemoverCombo/{codigo}/{CodigoCombo}")]
        public ActionResult<PedidoResponse> RemoverCombo(int codigo, int codigoCombo)
        {

            return pedidoController.RemoverCombo(codigo, codigoCombo);
        }

        [HttpPost]
        [Route("ConfirmarPedido/{codigo}")]
        public async Task<ActionResult<PedidoPagamentoResponse>> ConfirmarPedido(int codigo, PedidoPagamentoRequest pedidoPagamentoRequest, bool mock = false)
        {
            return await pedidoController.ConfirmarPedido(codigo, pedidoPagamentoRequest, mock ? MetodoPagamentoEnum.MercadoPagoMock : MetodoPagamentoEnum.MercadoPago);
        }

        [HttpPost]
        [Route("webhook/ConfirmarPagamento")]
        public async Task<PedidoResponse> ConfirmarPagamento([FromBody] PagamentoMercadoPagoRequest pagamentoMercadoPagoRequest, bool mock = false)
        {
            return await pedidoController.ConfirmarPagamento(pagamentoMercadoPagoRequest.id, mock ? MetodoPagamentoEnum.MercadoPagoMock : MetodoPagamentoEnum.MercadoPago);
        }
    }
}
