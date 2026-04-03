namespace EmisorDrones.CoreBusiness.Models
{
    public class Dron
    {
        public int Id { get; set; }

        public string NombreDron { get; set; }

        public int AlturaActual { get; set; }

        public int AlturaMinima { get; set; }

        public int AlturaMaxima { get; set; }

        public Dron(int id, string nombreDron, int alturaInicial, int alturaMinima, int alturaMaxima)
        {
            Id = id;
            NombreDron = nombreDron;
            AlturaActual = alturaInicial;
            AlturaMinima = alturaMinima;
            AlturaMaxima = alturaMaxima;
        }

        public bool SubirUnMetro()
        {
            if (AlturaActual >= AlturaMaxima)
                return false;

            AlturaActual++;
            return true;
        }

        public bool BajarUnMetro()
        {
            if (AlturaActual <= AlturaMinima)
                return false;

            AlturaActual--;
            return true;
        }
    }
}
