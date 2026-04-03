namespace EmisorDrones.CoreBusiness.Models
{
    public class InstruccionMensajeConfig
    {
        public string NombreDron { get; set; }

        public int AlturaObjetivo { get; set; }

        public InstruccionMensajeConfig(string nombreDron, int alturaObjetivo)
        {
            NombreDron = nombreDron;
            AlturaObjetivo = alturaObjetivo;
        }
    }
}
