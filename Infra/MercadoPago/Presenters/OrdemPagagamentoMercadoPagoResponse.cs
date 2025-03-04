namespace Infra.MercadoPago.Presenters
{
    public class OrdemPagagamentoMercadoPagoResponse(long id, string status, string external_reference)
    {
        public long id { get; set; } = id;
        public string status { get; set; } = status;
        public string external_reference { get; set; } = external_reference;
    }
}