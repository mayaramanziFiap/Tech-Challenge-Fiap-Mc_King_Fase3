namespace Infra.MercadoPago.Presenters
{
    public class PagamentoMercadoPagoDataRequest
    {
        public string currency_id { get; set; }
        public string marketplace { get; set; }
        public string status { get; set; }
    }
}
