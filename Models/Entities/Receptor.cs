namespace DemoFacturacionHacienda.Models.Entities
{
    public class Receptor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string TipoCedula { get; set; } = "01";
        public string CorreoElectronico { get; set; } = string.Empty;
    }
}
