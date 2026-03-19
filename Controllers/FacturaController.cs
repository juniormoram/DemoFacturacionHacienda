using DemoFacturacionHacienda.Data;
using DemoFacturacionHacienda.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoFacturacionHacienda.Controllers
{
    public class FacturaController : Controller
    {
        private readonly AppDbContext _db;

        public FacturaController(AppDbContext db)
        {
            _db = db;
        }

        // Lista de facturas
        public async Task<IActionResult> Index()
        {
            var facturas = await _db.Facturas
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();
            return View(facturas);
        }

        // Formulario nueva factura
        public IActionResult Create()
        {
            return View();
        }

        // Guardar nueva factura
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

            _db.Facturas.Add(factura);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Detalle de factura
        public async Task<IActionResult> Details(int id)
        {
            var factura = await _db.Facturas
                .Include(f => f.Lineas)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null) return NotFound();
            return View(factura);
        }
    }
}