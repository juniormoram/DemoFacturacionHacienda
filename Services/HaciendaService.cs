using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DemoFacturacionHacienda.Models.Entities;

namespace DemoFacturacionHacienda.Services
{
    public class HaciendaService
    {
        private readonly IConfiguration _config;
        private readonly FirmaDigitalService _firmaService;
        private readonly IHttpClientFactory _httpClientFactory;

        public HaciendaService(
            IConfiguration config,
            FirmaDigitalService firmaService,
            IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _firmaService = firmaService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> ObtenerTokenAsync()
        {
            var url = _config["Hacienda:UrlToken"]!;
            var usuario = _config["Hacienda:UsuarioApi"]!;
            var password = _config["Hacienda:PasswordApi"]!;

            var client = _httpClientFactory.CreateClient();

            var body = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", "api-stag"),
                new KeyValuePair<string, string>("username", usuario),
                new KeyValuePair<string, string>("password", password)
            });

            var response = await client.PostAsync(url, body);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error obteniendo token: {content}");

            var json = JsonDocument.Parse(content);
            return json.RootElement.GetProperty("access_token").GetString()!;
        }

        public async Task<(bool Exito, string Mensaje)> EnviarComprobanteAsync(
        Factura factura, string xmlContent)
        {
            try
            {
                var token = await ObtenerTokenAsync();
                var xmlBase64 = _firmaService.ObtenerBase64Xml(xmlContent);

                var payload = new
                {
                    clave = factura.Clave,
                    fecha = factura.FechaEmision.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    emisor = new
                    {
                        tipoIdentificacion = "02",
                        numeroIdentificacion = factura.EmisorCedula
                            .Replace("-", "").Replace(" ", "")
                    },
                    receptor = new
                    {
                        tipoIdentificacion = "01",
                        numeroIdentificacion = factura.ReceptorCedula
                            .Replace("-", "").Replace(" ", "")
                    },
                    comprobanteXml = xmlBase64
                };

                var json = JsonSerializer.Serialize(payload);

                var client = _httpClientFactory.CreateClient();
                var url = _config["Hacienda:UrlApi"]!;

                // Usar HttpRequestMessage para control total del header
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(
                    json, Encoding.UTF8, "application/json");

                // Agregar token directamente al mensaje, no al cliente
                request.Headers.TryAddWithoutValidation(
                    "authorization", $"bearer {token}");

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    return (true, "✅ Comprobante recibido por Hacienda. Procesando...");

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    return (true, "⚠️ Este comprobante ya fue enviado anteriormente.");

                return (false, $"Error {(int)response.StatusCode}: {responseContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<(string Estado, string Mensaje)> ConsultarEstadoAsync(string clave)
        {
            try
            {
                var token = await ObtenerTokenAsync();

                var client = _httpClientFactory.CreateClient();
                var url = $"{_config["Hacienda:UrlApi"]}/{clave}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.TryAddWithoutValidation(
                    "authorization", $"bearer {token}");

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return ("Error", $"Error consultando estado: {content}");

                var json = JsonDocument.Parse(content);
                var estado = json.RootElement
                    .GetProperty("indEstado").GetString() ?? "procesando";
                var mensaje = json.RootElement
                    .TryGetProperty("respuesta-xml", out var resp)
                        ? resp.GetString() ?? "Sin mensaje"
                        : "Sin mensaje adicional";

                return (estado, mensaje);
            }
            catch (Exception ex)
            {
                return ("Error", $"Error: {ex.Message}");
            }
        }
    }
}