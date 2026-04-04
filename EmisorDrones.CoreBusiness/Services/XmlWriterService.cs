using System.Xml;
using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Services
{
    public class XmlWriterService
    {
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

                writer.WriteStartElement("mensaje");
                writer.WriteAttributeString("nombre", nombreMensaje ?? string.Empty);

                writer.WriteStartElement("sistemaDrones");
                writer.WriteString(nombreSistema ?? string.Empty);
                writer.WriteEndElement();

                writer.WriteStartElement("tiempoOptimo");
                writer.WriteString(ObtenerTiempoOptimo(timeline).ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("mensajeRecibido");
                writer.WriteString(mensajeRecibido ?? string.Empty);
                writer.WriteEndElement();

                writer.WriteStartElement("instrucciones");

                NodoGenerico<InstanteTiempo> nodoInstante = timeline.Cabeza;
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
                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
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
