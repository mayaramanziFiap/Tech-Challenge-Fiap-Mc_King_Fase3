namespace Infra.MercadoPago.Presenters
{
    public class PedidoMercadoPagoRequest
    {
        public string external_reference { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string notification_url { get; set; }
        public decimal total_amount { get; set; }
        public IList<PedidoItemMercadoPagoRequest> items { get; set; }
    }
}
