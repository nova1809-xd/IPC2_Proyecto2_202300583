using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Models
{
    public class MensajeConfig
    {
        public string NombreMensaje { get; set; }

        public string NombreSistemaDrones { get; set; }

        public ListaEnlazada<InstruccionMensajeConfig> Instrucciones { get; private set; }

        public MensajeConfig(string nombreMensaje, string nombreSistemaDrones)
        {
            NombreMensaje = nombreMensaje;
            NombreSistemaDrones = nombreSistemaDrones;
            Instrucciones = new ListaEnlazada<InstruccionMensajeConfig>();
        }
    }
}
