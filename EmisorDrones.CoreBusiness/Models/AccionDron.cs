namespace EmisorDrones.CoreBusiness.Models
{
    public class AccionDron
    {
        public string NombreDron { get; set; }

        public string Movimiento { get; set; }

        public AccionDron(string nombreDron, string movimiento)
        {
            NombreDron = nombreDron;
            Movimiento = movimiento;
        }
    }
}
