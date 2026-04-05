using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Services
{
    public class EstadoSistemaService
    {
        public ListaEnlazada<string> NombresDrones { get; private set; }

        public ListaEnlazada<SistemaDronesConfig> SistemasDrones { get; private set; }

        public ListaEnlazada<MensajeConfig> Mensajes { get; private set; }

        public bool TieneDatosCargados =>
            NombresDrones.Cantidad > 0 || SistemasDrones.Cantidad > 0 || Mensajes.Cantidad > 0;

        public EstadoSistemaService()
        {
            NombresDrones = new ListaEnlazada<string>();
            SistemasDrones = new ListaEnlazada<SistemaDronesConfig>();
            Mensajes = new ListaEnlazada<MensajeConfig>();
        }

        public void CargarDesdeResultado(ResultadoCargaConfigXml resultado)
        {
            LimpiarTodo();

            if (resultado == null)
                return;

            CopiarStrings(resultado.NombresDrones, NombresDrones);
            CopiarSistemas(resultado.SistemasDrones, SistemasDrones);
            CopiarMensajes(resultado.Mensajes, Mensajes);
        }

        public void LimpiarTodo()
        {
            NombresDrones.Limpiar();
            SistemasDrones.Limpiar();
            Mensajes.Limpiar();
        }

        private void CopiarStrings(ListaEnlazada<string> origen, ListaEnlazada<string> destino)
        {
            NodoGenerico<string> actual = origen.Cabeza;
            while (actual != null)
            {
                destino.AgregarAlFinal(actual.Dato);
                actual = actual.Siguiente;
            }
        }

        private void CopiarSistemas(ListaEnlazada<SistemaDronesConfig> origen, ListaEnlazada<SistemaDronesConfig> destino)
        {
            NodoGenerico<SistemaDronesConfig> actualSistema = origen.Cabeza;
            while (actualSistema != null)
            {
                SistemaDronesConfig sistemaOrigen = actualSistema.Dato;
                SistemaDronesConfig copia = new SistemaDronesConfig(
                    sistemaOrigen.NombreSistema,
                    sistemaOrigen.AlturaMaxima,
                    sistemaOrigen.CantidadDrones);

                NodoGenerico<ContenidoSistemaDrones> actualContenido = sistemaOrigen.Contenidos.Cabeza;
                while (actualContenido != null)
                {
                    ContenidoSistemaDrones contenidoOrigen = actualContenido.Dato;
                    ContenidoSistemaDrones contenidoCopia = new ContenidoSistemaDrones(contenidoOrigen.NombreDron);

                    NodoGenerico<AlturaSimbolo> actualAltura = contenidoOrigen.Alturas.Cabeza;
                    while (actualAltura != null)
                    {
                        contenidoCopia.Alturas.AgregarAlFinal(
                            new AlturaSimbolo(actualAltura.Dato.ValorAltura, actualAltura.Dato.Simbolo));
                        actualAltura = actualAltura.Siguiente;
                    }

                    copia.Contenidos.AgregarAlFinal(contenidoCopia);
                    actualContenido = actualContenido.Siguiente;
                }

                destino.AgregarAlFinal(copia);
                actualSistema = actualSistema.Siguiente;
            }
        }

        private void CopiarMensajes(ListaEnlazada<MensajeConfig> origen, ListaEnlazada<MensajeConfig> destino)
        {
            NodoGenerico<MensajeConfig> actualMensaje = origen.Cabeza;
            while (actualMensaje != null)
            {
                MensajeConfig mensajeOrigen = actualMensaje.Dato;
                MensajeConfig copia = new MensajeConfig(
                    mensajeOrigen.NombreMensaje,
                    mensajeOrigen.NombreSistemaDrones);

                NodoGenerico<InstruccionMensajeConfig> actualInstruccion = mensajeOrigen.Instrucciones.Cabeza;
                while (actualInstruccion != null)
                {
                    copia.Instrucciones.AgregarAlFinal(
                        new InstruccionMensajeConfig(
                            actualInstruccion.Dato.NombreDron,
                            actualInstruccion.Dato.AlturaObjetivo));

                    actualInstruccion = actualInstruccion.Siguiente;
                }

                destino.AgregarAlFinal(copia);
                actualMensaje = actualMensaje.Siguiente;
            }
        }
    }
}
