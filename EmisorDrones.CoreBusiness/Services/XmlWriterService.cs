using System.Xml;
using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Services
{
    public class SalidaMensajeXml
    {
        public string NombreMensaje { get; private set; }

        public string NombreSistema { get; private set; }

        public string MensajeRecibido { get; private set; }

        public ListaEnlazada<InstanteTiempo> Timeline { get; private set; }

        public SalidaMensajeXml(
            string nombreMensaje,
            string nombreSistema,
            string mensajeRecibido,
            ListaEnlazada<InstanteTiempo> timeline)
        {
            NombreMensaje = nombreMensaje ?? string.Empty;
            NombreSistema = nombreSistema ?? string.Empty;
            MensajeRecibido = mensajeRecibido ?? string.Empty;
            Timeline = timeline ?? new ListaEnlazada<InstanteTiempo>();
        }
    }

    public class XmlWriterService
    {
        public void GenerarSalidaXml(ListaEnlazada<SalidaMensajeXml> mensajesSalida, string rutaSalidaXml)
        {
            if (mensajesSalida == null)
                throw new ArgumentNullException(nameof(mensajesSalida));

            if (string.IsNullOrWhiteSpace(rutaSalidaXml))
                throw new ArgumentException("la ruta de salida xml es obligatoria");

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8,
                OmitXmlDeclaration = false
            };

            using (XmlWriter writer = XmlWriter.Create(rutaSalidaXml, settings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("respuesta");
                writer.WriteStartElement("listaMensajes");

                NodoGenerico<SalidaMensajeXml> nodoMensaje = mensajesSalida.Cabeza;
                while (nodoMensaje != null)
                {
                    EscribirMensaje(writer, nodoMensaje.Dato);
                    nodoMensaje = nodoMensaje.Siguiente;
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void GenerarSalidaXml(
            ListaEnlazada<InstanteTiempo> timeline,
            string rutaSalidaXml,
            string nombreMensaje,
            string nombreSistema,
            string mensajeRecibido)
        {
            if (timeline == null)
                throw new ArgumentNullException(nameof(timeline));

            if (string.IsNullOrWhiteSpace(rutaSalidaXml))
                throw new ArgumentException("la ruta de salida xml es obligatoria");

            ListaEnlazada<SalidaMensajeXml> mensajes = new ListaEnlazada<SalidaMensajeXml>();
            mensajes.AgregarAlFinal(new SalidaMensajeXml(nombreMensaje, nombreSistema, mensajeRecibido, timeline));

            GenerarSalidaXml(mensajes, rutaSalidaXml);
        }

        private void EscribirMensaje(XmlWriter writer, SalidaMensajeXml mensaje)
        {
            writer.WriteStartElement("mensaje");
            writer.WriteAttributeString("nombre", mensaje.NombreMensaje);

            writer.WriteStartElement("sistemaDrones");
            writer.WriteString(mensaje.NombreSistema);
            writer.WriteEndElement();

            writer.WriteStartElement("tiempoOptimo");
            writer.WriteString(ObtenerTiempoOptimo(mensaje.Timeline).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("mensajeRecibido");
            writer.WriteString(mensaje.MensajeRecibido);
            writer.WriteEndElement();

            writer.WriteStartElement("instrucciones");

            NodoGenerico<InstanteTiempo> nodoInstante = mensaje.Timeline.Cabeza;
            while (nodoInstante != null)
            {
                InstanteTiempo instante = nodoInstante.Dato;

                writer.WriteStartElement("tiempo");
                writer.WriteAttributeString("valor", instante.Segundo.ToString());

                writer.WriteStartElement("acciones");

                NodoGenerico<AccionDron> nodoAccion = instante.Acciones.Cabeza;
                while (nodoAccion != null)
                {
                    AccionDron accion = nodoAccion.Dato;

                    writer.WriteStartElement("dron");
                    writer.WriteAttributeString("nombre", accion.NombreDron ?? string.Empty);
                    writer.WriteString(accion.Movimiento ?? "Esperar");
                    writer.WriteEndElement();

                    nodoAccion = nodoAccion.Siguiente;
                }

                writer.WriteEndElement();
                writer.WriteEndElement();

                nodoInstante = nodoInstante.Siguiente;
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private int ObtenerTiempoOptimo(ListaEnlazada<InstanteTiempo> timeline)
        {
            int maxSegundo = 0;
            NodoGenerico<InstanteTiempo> nodo = timeline.Cabeza;

            while (nodo != null)
            {
                if (nodo.Dato.Segundo > maxSegundo)
                    maxSegundo = nodo.Dato.Segundo;

                nodo = nodo.Siguiente;
            }

            return maxSegundo;
        }
    }
}
