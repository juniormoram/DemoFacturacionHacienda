namespace DemoFacturacionHacienda.Models.Entities
{
    public class LineaDetalle
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public int NumeroLinea { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PorcentajeIVA { get; set; } = 13; 
        public decimal MontoIVA => Cantidad * PrecioUnitario * (PorcentajeIVA / 100);
        public decimal SubTotal => Cantidad * PrecioUnitario;
        public decimal Total => SubTotal + MontoIVA;

        // Navegación
        public Factura? Factura { get; set; }
    }
}
