namespace DemoFacturacionHacienda.Services
{
    public class ClaveService
    {
        // Genera la clave numérica de 50 dígitos según especificación Hacienda
        public string GenerarClave(string consecutivo, string cedulaEmisor)
        {
            string codigoPais = "506";

            string fecha = DateTime.Now.ToString("ddMMyyyy");
                        
            string cedula = cedulaEmisor.Replace("-", "").Replace(" ", "").PadLeft(12, '0');
            if (cedula.Length > 12) cedula = cedula[..12];
                        
            string consec = consecutivo.PadLeft(20, '0');
            if (consec.Length > 20) consec = consec[..20];

            string situacion = "1";

            string seguridad = new Random().Next(10000000, 99999999).ToString();

            string clave = $"{codigoPais}{fecha}{cedula}{consec}{situacion}{seguridad}";
                        
            return clave.PadLeft(50, '0')[..50];
        }
    }
}
