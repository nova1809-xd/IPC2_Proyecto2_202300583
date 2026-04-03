using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Models
{
    public class SistemaDronesConfig
    {
        public string NombreSistema { get; set; }

        public int AlturaMaxima { get; set; }

        public int CantidadDrones { get; set; }

        public ListaEnlazada<ContenidoSistemaDrones> Contenidos { get; private set; }

        public SistemaDronesConfig(string nombreSistema, int alturaMaxima, int cantidadDrones)
        {
            NombreSistema = nombreSistema;
            AlturaMaxima = alturaMaxima;
            CantidadDrones = cantidadDrones;
            Contenidos = new ListaEnlazada<ContenidoSistemaDrones>();
        }
    }
}
