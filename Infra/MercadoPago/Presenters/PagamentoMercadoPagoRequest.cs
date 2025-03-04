namespace Infra.MercadoPago.Presenters
{
    public class PagamentoMercadoPagoRequest
    {
        public string action { get; set; }
        public string application_id { get; set; }
        public PagamentoMercadoPagoDataRequest data { get; set; }
        public DateTime date_created { get; set; }
        public string id { get; set; }
        public bool live_mode { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public long user_id { get; set; }
        public int version { get; set; }
    }
}
