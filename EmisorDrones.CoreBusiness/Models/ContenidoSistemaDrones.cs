using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Models
{
    public class ContenidoSistemaDrones
    {
        public string NombreDron { get; set; }

        public ListaEnlazada<AlturaSimbolo> Alturas { get; private set; }

        public ContenidoSistemaDrones(string nombreDron)
        {
            NombreDron = nombreDron;
            Alturas = new ListaEnlazada<AlturaSimbolo>();
        }
    }
}
