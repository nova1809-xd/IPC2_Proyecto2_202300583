using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Models
{
    public class InstanteTiempo
    {
        public int Segundo { get; set; }

        public ListaEnlazada<AccionDron> Acciones { get; private set; }

        public InstanteTiempo(int segundo)
        {
            Segundo = segundo;
            Acciones = new ListaEnlazada<AccionDron>();
        }
    }
}
