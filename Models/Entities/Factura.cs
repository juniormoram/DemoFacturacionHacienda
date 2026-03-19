using System.ComponentModel.DataAnnotations;

namespace DemoFacturacionHacienda.Models.Entities
{
    public class Factura
    {
        public int Id { get; set; }

        [Display(Name = "Número de Factura")]
        public string NumeroConsecutivo { get; set; } = string.Empty;

        [Display(Name = "Clave")]
        public string Clave { get; set; } = string.Empty;

        [Display(Name = "Fecha de Emisión")]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        // Emisor (datos de empresa)
        public string EmisorNombre { get; set; } = string.Empty;
        public string EmisorCedula { get; set; } = string.Empty;

        // Receptor (cliente)
        public string ReceptorNombre { get; set; } = string.Empty;
        public string ReceptorCedula { get; set; } = string.Empty;
        public string ReceptorCorreo { get; set; } = string.Empty;

        // Totales
        public decimal TotalVenta { get; set; }
        public decimal TotalImpuesto { get; set; }
        public decimal TotalComprobante { get; set; }

        // Estado Hacienda
        [Display(Name = "Estado")]
        public EstadoFactura Estado { get; set; } = EstadoFactura.Borrador;
        public string? MensajeHacienda { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaRespuesta { get; set; }

        // XML generado
        public string? XmlGenerado { get; set; }

        // Relación con líneas
        public List<LineaDetalle> Lineas { get; set; } = new();
    }

    public enum EstadoFactura
    {
        Borrador,
        Enviada,
        Aceptada,
        Rechazada,
        Error
    }
}
