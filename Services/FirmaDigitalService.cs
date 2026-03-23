using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DemoFacturacionHacienda.Services
{
    public class FirmaDigitalService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public FirmaDigitalService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public string FirmarXml(string xmlContent)
        {
            try
            {
                var certPath = Path.Combine(
                    _env.ContentRootPath,
                    _config["Hacienda:CertificadoPath"]!);
                var pin = _config["Hacienda:CertificadoPin"]!;

                // Cargar el .p12
                var cert = new X509Certificate2(
                    certPath,
                    pin,
                    X509KeyStorageFlags.Exportable);

                // Por ahora retornamos el XML en Base64 (simulando firma)
                // En producción aquí va la firma XAdES-BES completa
                var xmlBytes = Encoding.UTF8.GetBytes(xmlContent);
                return Convert.ToBase64String(xmlBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al firmar XML: {ex.Message}");
            }
        }

        public string ObtenerBase64Xml(string xmlContent)
        {
            var xmlBytes = Encoding.UTF8.GetBytes(xmlContent);
            return Convert.ToBase64String(xmlBytes);
        }
    }
}
