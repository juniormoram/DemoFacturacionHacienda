using DemoFacturacionHacienda.Data;
using DemoFacturacionHacienda.Models.Entities;
using DemoFacturacionHacienda.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoFacturacionHacienda.Controllers
{
    public class FacturaController : Controller
    {
        private readonly AppDbContext _db;
        private readonly XmlService _xmlService;

        public FacturaController(AppDbContext db, XmlService xmlService)
        {
            _db = db;
            _xmlService = xmlService;
        }

        public async Task<IActionResult> Index()
        {
            var facturas = await _db.Facturas
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();
            return View(facturas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Factura factura,
            List<string> descripciones, List<decimal> cantidades,
            List<decimal> precios, List<decimal> ivaPorcentajes)
        {
            for (int i = 0; i < descripciones.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(descripciones[i]))
                {
                    factura.Lineas.Add(new LineaDetalle
                    {
                        NumeroLinea = i + 1,
                        Descripcion = descripciones[i],
                        Cantidad = cantidades[i],
                        PrecioUnitario = precios[i],
                        PorcentajeIVA = ivaPorcentajes[i]
                    });
                }
            }

            factura.TotalVenta = factura.Lineas.Sum(l => l.SubTotal);
            factura.TotalImpuesto = factura.Lineas.Sum(l => l.MontoIVA);
            factura.TotalComprobante = factura.Lineas.Sum(l => l.Total);

            int count = await _db.Facturas.CountAsync();
            factura.NumeroConsecutivo = $"00100001010{(count + 1):D10}";
            factura.FechaEmision = DateTime.Now;
            factura.Estado = EstadoFactura.Borrador;

            // Generar XML al guardar
            factura.XmlGenerado = _xmlService.GenerarXml(factura);

            _db.Facturas.Add(factura);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = factura.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var factura = await _db.Facturas
                .Include(f => f.Lineas)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null) return NotFound();
            return View(factura);
        }

        // Enviar a Hacienda (por ahora solo genera XML y marca como Enviada)
        [HttpPost]
        public async Task<IActionResult> Enviar(int id)
        {
            var factura = await _db.Facturas
                .Include(f => f.Lineas)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null) return NotFound();

            // Regenerar XML si no tiene
            if (string.IsNullOrEmpty(factura.XmlGenerado))
                factura.XmlGenerado = _xmlService.GenerarXml(factura);

            factura.Estado = EstadoFactura.Enviada;
            factura.FechaEnvio = DateTime.Now;
            factura.MensajeHacienda = "XML generado correctamente. Pendiente integración con API.";

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}