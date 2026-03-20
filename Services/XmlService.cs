using DemoFacturacionHacienda.Models.Entities;
using DemoFacturacionHacienda.Models.Hacienda;
using System.Text;
using System.Xml;

namespace DemoFacturacionHacienda.Services
{
    public class XmlService
    {
        private readonly ClaveService _claveService;

        public XmlService(ClaveService claveService)
        {
            _claveService = claveService;
        }

        public string GenerarXml(Factura factura)
        {
            // Generar clave si no tiene
            if (string.IsNullOrEmpty(factura.Clave))
                factura.Clave = _claveService.GenerarClave(
                    factura.NumeroConsecutivo,
                    factura.EmisorCedula);

            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false
            };

            using var writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("FacturaElectronica",
                "https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/facturaElectronica");

            // Atributos del namespace
            writer.WriteAttributeString("xmlns", "xsi", null,
                "http://www.w3.org/2001/XMLSchema-instance");

            // Clave
            writer.WriteElementString("Clave", factura.Clave);

            // Código Actividad
            writer.WriteElementString("CodigoActividad", "12345");

            // Número Consecutivo
            writer.WriteElementString("NumeroConsecutivo", factura.NumeroConsecutivo);

            // Fecha Emisión 
            writer.WriteElementString("FechaEmision",
                factura.FechaEmision.ToString("yyyy-MM-ddTHH:mm:sszzz"));

            // Emisor
            writer.WriteStartElement("Emisor");
            writer.WriteElementString("Nombre", factura.EmisorNombre);
            writer.WriteStartElement("Identificacion");
            writer.WriteElementString("Tipo", "02"); 
            writer.WriteElementString("Numero",
                factura.EmisorCedula.Replace("-", "").Replace(" ", ""));
            writer.WriteEndElement(); // Identificacion
            writer.WriteEndElement(); // Emisor

            // Receptor
            writer.WriteStartElement("Receptor");
            writer.WriteElementString("Nombre", factura.ReceptorNombre);
            writer.WriteStartElement("Identificacion");
            writer.WriteElementString("Tipo", "01"); 
            writer.WriteElementString("Numero",
                factura.ReceptorCedula.Replace("-", "").Replace(" ", ""));
            writer.WriteEndElement(); // Identificacion
            writer.WriteElementString("CorreoElectronico", factura.ReceptorCorreo);
            writer.WriteEndElement(); // Receptor

            // Detalle de servicios
            writer.WriteStartElement("DetalleServicio");
            foreach (var linea in factura.Lineas)
            {
                writer.WriteStartElement("LineaDetalle");
                writer.WriteElementString("NumeroLinea",
                    linea.NumeroLinea.ToString());
                writer.WriteElementString("Cantidad",
                    linea.Cantidad.ToString("F5"));
                writer.WriteElementString("UnidadMedida", "Sp");
                writer.WriteElementString("Detalle", linea.Descripcion);
                writer.WriteElementString("PrecioUnitario",
                    linea.PrecioUnitario.ToString("F5"));
                writer.WriteElementString("MontoTotal",
                    linea.SubTotal.ToString("F5"));
                writer.WriteElementString("SubTotal",
                    linea.SubTotal.ToString("F5"));

                // Impuesto
                writer.WriteStartElement("Impuesto");
                writer.WriteElementString("Codigo", "01"); // IVA
                writer.WriteElementString("CodigoTarifa",
                    ObtenerCodigoTarifa(linea.PorcentajeIVA));
                writer.WriteElementString("Tarifa",
                    linea.PorcentajeIVA.ToString("F2"));
                writer.WriteElementString("Monto",
                    linea.MontoIVA.ToString("F5"));
                writer.WriteEndElement(); // Impuesto

                writer.WriteElementString("MontoTotalLinea",
                    linea.Total.ToString("F5"));
                writer.WriteEndElement(); // LineaDetalle
            }
            writer.WriteEndElement(); // DetalleServicio

            // Resumen
            writer.WriteStartElement("ResumenFactura");
            writer.WriteElementString("CodigoTipoMoneda", "CRC");
            writer.WriteElementString("TotalServGravados",
                factura.TotalVenta.ToString("F5"));
            writer.WriteElementString("TotalGravado",
                factura.TotalVenta.ToString("F5"));
            writer.WriteElementString("TotalVenta",
                factura.TotalVenta.ToString("F5"));
            writer.WriteElementString("TotalDescuentos", "0.00000");
            writer.WriteElementString("TotalVentaNeta",
                factura.TotalVenta.ToString("F5"));
            writer.WriteElementString("TotalImpuesto",
                factura.TotalImpuesto.ToString("F5"));
            writer.WriteElementString("TotalComprobante",
                factura.TotalComprobante.ToString("F5"));
            writer.WriteEndElement(); // ResumenFactura

            writer.WriteEndElement(); // FacturaElectronica
            writer.WriteEndDocument();
            writer.Flush();

            return sb.ToString();
        }

        private string ObtenerCodigoTarifa(decimal porcentaje) => porcentaje switch
        {
            13 => "08",
            4 => "04",
            2 => "03",
            1 => "02",
            0 => "01",
            _ => "08"
        };
    }
}