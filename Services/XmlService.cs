using DemoFacturacionHacienda.Models.Entities;
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

            // Versión 4.4 — namespace actualizado
            writer.WriteStartElement("FacturaElectronica",
                "https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.4/facturaElectronica");
            writer.WriteAttributeString("xmlns", "xsi", null,
                "http://www.w3.org/2001/XMLSchema-instance");

            writer.WriteElementString("Clave", factura.Clave);
            writer.WriteElementString("CodigoActividad", "722000");
            writer.WriteElementString("NumeroConsecutivo", factura.NumeroConsecutivo);
            writer.WriteElementString("FechaEmision",
                factura.FechaEmision.ToString("yyyy-MM-ddTHH:mm:sszzz"));

            // Emisor
            writer.WriteStartElement("Emisor");
            writer.WriteElementString("Nombre", factura.EmisorNombre);
            writer.WriteStartElement("Identificacion");
            writer.WriteElementString("Tipo", "02");
            writer.WriteElementString("Numero",
                factura.EmisorCedula.Replace("-", "").Replace(" ", ""));
            writer.WriteEndElement();
            writer.WriteEndElement();

            // Receptor
            writer.WriteStartElement("Receptor");
            writer.WriteElementString("Nombre", factura.ReceptorNombre);
            writer.WriteStartElement("Identificacion");
            writer.WriteElementString("Tipo", "01");
            writer.WriteElementString("Numero",
                factura.ReceptorCedula.Replace("-", "").Replace(" ", ""));
            writer.WriteEndElement();
            writer.WriteElementString("CorreoElectronico", factura.ReceptorCorreo);
            writer.WriteEndElement();

            // Condición de venta y medio de pago (nuevos en v4.4)
            writer.WriteElementString("CondicionVenta", "01"); // 01 = Contado
            writer.WriteStartElement("MedioPago");
            writer.WriteElementString("Codigo", "01"); // 01 = Efectivo
            writer.WriteEndElement();

            // Detalle
            writer.WriteStartElement("DetalleServicio");
            foreach (var linea in factura.Lineas)
            {
                writer.WriteStartElement("LineaDetalle");
                writer.WriteElementString("NumeroLinea", linea.NumeroLinea.ToString());
                writer.WriteElementString("Cantidad", linea.Cantidad.ToString("F5"));
                writer.WriteElementString("UnidadMedida", "Sp");
                writer.WriteElementString("Detalle", linea.Descripcion);
                writer.WriteElementString("PrecioUnitario",
                    linea.PrecioUnitario.ToString("F5"));
                writer.WriteElementString("MontoTotal", linea.SubTotal.ToString("F5"));
                writer.WriteElementString("SubTotal", linea.SubTotal.ToString("F5"));

                writer.WriteStartElement("Impuesto");
                writer.WriteElementString("Codigo", "01");
                writer.WriteElementString("CodigoTarifa",
                    ObtenerCodigoTarifa(linea.PorcentajeIVA));
                writer.WriteElementString("Tarifa", linea.PorcentajeIVA.ToString("F2"));
                writer.WriteElementString("Monto", linea.MontoIVA.ToString("F5"));
                writer.WriteEndElement();

                writer.WriteElementString("ImpuestoNeto", linea.MontoIVA.ToString("F5"));
                writer.WriteElementString("MontoTotalLinea", linea.Total.ToString("F5"));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

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

            // Campo nuevo obligatorio en v4.4
            writer.WriteElementString("TipoCambio", "1.00000");
            writer.WriteEndElement();

            writer.WriteEndElement();
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