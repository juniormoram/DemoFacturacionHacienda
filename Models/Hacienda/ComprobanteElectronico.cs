namespace DemoFacturacionHacienda.Models.Hacienda
{
    public class ComprobanteElectronico
    {
        public string Clave { get; set; } = string.Empty;
        public string CodigoActividad { get; set; } = "12345"; 
        public string NumeroConsecutivo { get; set; } = string.Empty;
        public string FechaEmision { get; set; } = string.Empty;

        public EmisorReceptor Emisor { get; set; } = new();
        public EmisorReceptor Receptor { get; set; } = new();

        public List<LineaDetalleXml> DetalleServicio { get; set; } = new();

        public ResumenFactura ResumenFactura { get; set; } = new();
    }

    public class EmisorReceptor
    {
        public string Nombre { get; set; } = string.Empty;
        public Identificacion Identificacion { get; set; } = new();
        public string CorreoElectronico { get; set; } = string.Empty;
    }

    public class Identificacion
    {
        public string Tipo { get; set; } = "01"; 
        public string Numero { get; set; } = string.Empty;
    }

    public class LineaDetalleXml
    {
        public int NumeroLinea { get; set; }
        public string Codigo { get; set; } = "01";
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; } = "Sp"; 
        public string Detalle { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SubTotal { get; set; }
        public ImpuestoXml Impuesto { get; set; } = new();
        public decimal MontoTotalLinea { get; set; }
    }

    public class ImpuestoXml
    {
        public string Codigo { get; set; } = "01"; 
        public string CodigoTarifa { get; set; } = "08";
        public decimal Tarifa { get; set; } = 13;
        public decimal Monto { get; set; }
    }

    public class ResumenFactura
    {
        public string CodigoTipoMoneda { get; set; } = "CRC";
        public decimal TotalServGravados { get; set; }
        public decimal TotalGravado { get; set; }
        public decimal TotalVenta { get; set; }
        public decimal TotalDescuentos { get; set; } = 0;
        public decimal TotalVentaNeta { get; set; }
        public decimal TotalImpuesto { get; set; }
        public decimal TotalComprobante { get; set; }
    }
}
