using System.Xml;
using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Services
{
    public class ResultadoCargaConfigXml
    {
        public bool Exito { get; private set; }

        public ListaEnlazada<string> Errores { get; private set; }

        public ListaEnlazada<string> NombresDrones { get; private set; }

        public ListaEnlazada<SistemaDronesConfig> SistemasDrones { get; private set; }

        public ListaEnlazada<MensajeConfig> Mensajes { get; private set; }

        public ResultadoCargaConfigXml()
        {
            Exito = true;
            Errores = new ListaEnlazada<string>();
            NombresDrones = new ListaEnlazada<string>();
            SistemasDrones = new ListaEnlazada<SistemaDronesConfig>();
            Mensajes = new ListaEnlazada<MensajeConfig>();
        }

        public void AgregarError(string error)
        {
            Exito = false;
            Errores.AgregarAlFinal(error);
        }
    }

    public class XmlReaderService
    {
        public ResultadoCargaConfigXml CargarEntradaIncremental(string rutaArchivoXml)
        {
            return CargarEntradaIncremental(rutaArchivoXml, null);
        }

        public ResultadoCargaConfigXml CargarEntradaIncremental(string rutaArchivoXml, EstadoSistemaService? estadoSistema)
        {
            ResultadoCargaConfigXml resultado = new ResultadoCargaConfigXml();

            if (string.IsNullOrWhiteSpace(rutaArchivoXml))
            {
                resultado.AgregarError("la ruta del archivo xml es obligatoria");
                return resultado;
            }

            if (!File.Exists(rutaArchivoXml))
            {
                resultado.AgregarError("no existe el archivo xml en la ruta indicada");
                return resultado;
            }

            XmlDocument documento = new XmlDocument();
            documento.PreserveWhitespace = true;

            try
            {
                documento.Load(rutaArchivoXml);
            }
            catch (Exception ex)
            {
                resultado.AgregarError("no se pudo cargar el xml: " + ex.Message);
                return resultado;
            }

            XmlNode? nodoConfig = documento.SelectSingleNode("/config");
            if (nodoConfig == null)
            {
                resultado.AgregarError("el xml no contiene el nodo raiz config");
                return resultado;
            }

            CargarListaDrones(nodoConfig, resultado);
            CargarListaSistemas(nodoConfig, resultado);
            CargarListaMensajes(nodoConfig, resultado);

            if (resultado.NombresDrones.EstaVacia)
            {
                resultado.AgregarError("listaDrones no contiene drones validos");
            }

            ValidarReferenciasCruzadas(resultado, estadoSistema);

            if (resultado.Exito && estadoSistema != null)
            {
                TransferirAlEstado(resultado, estadoSistema);
            }

            return resultado;
        }

        private void CargarListaDrones(XmlNode nodoConfig, ResultadoCargaConfigXml resultado)
        {
            XmlNodeList? nodosDron = nodoConfig.SelectNodes("listaDrones/dron");
            if (nodosDron == null)
            {
                resultado.AgregarError("no se encontro la seccion listaDrones");
                return;
            }

            if (nodosDron.Count == 0)
            {
                resultado.AgregarError("listaDrones no contiene nodos dron");
                return;
            }

            for (int i = 0; i < nodosDron.Count; i++)
            {
                XmlNode? nodoDron = nodosDron.Item(i);
                string nombreDron = ObtenerTextoNormalizado(nodoDron);

                if (string.IsNullOrWhiteSpace(nombreDron))
                {
                    resultado.AgregarError("se encontro un dron vacio en listaDrones");
                    continue;
                }

                if (ExisteNombreDrone(resultado.NombresDrones, nombreDron))
                {
                    resultado.AgregarError("el dron " + nombreDron + " esta repetido en listaDrones");
                    continue;
                }

                resultado.NombresDrones.AgregarAlFinal(nombreDron);
            }
        }

        private void CargarListaSistemas(XmlNode nodoConfig, ResultadoCargaConfigXml resultado)
        {
            XmlNodeList? nodosSistema = nodoConfig.SelectNodes("listaSistemasDrones/sistemaDrones");
            if (nodosSistema == null)
            {
                resultado.AgregarError("no se encontro la seccion listaSistemasDrones");
                return;
            }

            if (nodosSistema.Count == 0)
            {
                resultado.AgregarError("listaSistemasDrones no contiene nodos sistemaDrones");
                return;
            }

            for (int i = 0; i < nodosSistema.Count; i++)
            {
                XmlNode? nodoSistema = nodosSistema.Item(i);
                string nombreSistema = ObtenerAtributo(nodoSistema, "nombre");

                if (string.IsNullOrWhiteSpace(nombreSistema))
                {
                    resultado.AgregarError("se encontro un sistemaDrones sin atributo nombre");
                    continue;
                }

                if (ExisteSistema(resultado.SistemasDrones, nombreSistema))
                {
                    resultado.AgregarError("el sistema " + nombreSistema + " esta repetido");
                    continue;
                }

                int alturaMaxima;
                int cantidadDrones;

                if (!TryParseNodoEntero(nodoSistema, "alturaMaxima", out alturaMaxima))
                {
                    resultado.AgregarError("el sistema " + nombreSistema + " tiene alturaMaxima invalida");
                    continue;
                }

                if (!TryParseNodoEntero(nodoSistema, "cantidadDrones", out cantidadDrones))
                {
                    resultado.AgregarError("el sistema " + nombreSistema + " tiene cantidadDrones invalida");
                    continue;
                }

                SistemaDronesConfig sistema = new SistemaDronesConfig(nombreSistema, alturaMaxima, cantidadDrones);

                XmlNodeList? nodosContenido = nodoSistema?.SelectNodes("contenido");
                if (nodosContenido == null || nodosContenido.Count == 0)
                {
                    resultado.AgregarError("el sistema " + nombreSistema + " no tiene nodos contenido");
                    continue;
                }

                for (int j = 0; j < nodosContenido.Count; j++)
                {
                    XmlNode? nodoContenido = nodosContenido.Item(j);
                    string nombreDron = ObtenerTextoNormalizado(nodoContenido?.SelectSingleNode("dron"));

                    if (string.IsNullOrWhiteSpace(nombreDron))
                    {
                        resultado.AgregarError("el sistema " + nombreSistema + " tiene un contenido sin dron");
                        continue;
                    }

                    ContenidoSistemaDrones contenido = new ContenidoSistemaDrones(nombreDron);

                    XmlNodeList? nodosAltura = nodoContenido?.SelectNodes("alturas/altura");
                    if (nodosAltura == null || nodosAltura.Count == 0)
                    {
                        resultado.AgregarError("el sistema " + nombreSistema + " no tiene alturas para dron " + nombreDron);
                        continue;
                    }

                    for (int k = 0; k < nodosAltura.Count; k++)
                    {
                        XmlNode? nodoAltura = nodosAltura.Item(k);
                        string valorTexto = ObtenerAtributo(nodoAltura, "valor");
                        int valorAltura;

                        if (!int.TryParse(valorTexto, out valorAltura))
                        {
                            resultado.AgregarError("altura invalida en sistema " + nombreSistema + ", dron " + nombreDron);
                            continue;
                        }

                        if (valorAltura < 1 || valorAltura > alturaMaxima)
                        {
                            resultado.AgregarError("altura fuera de rango en sistema " + nombreSistema + ", dron " + nombreDron);
                            continue;
                        }

                        string simbolo = ObtenerSimboloAltura(nodoAltura);
                        if (string.IsNullOrEmpty(simbolo))
                        {
                            resultado.AgregarError("simbolo vacio en sistema " + nombreSistema + ", dron " + nombreDron);
                            continue;
                        }

                        contenido.Alturas.AgregarAlFinal(new AlturaSimbolo(valorAltura, simbolo));
                    }

                    if (contenido.Alturas.Cantidad > 0)
                    {
                        sistema.Contenidos.AgregarAlFinal(contenido);
                    }
                }

                if (sistema.Contenidos.Cantidad == 0)
                {
                    resultado.AgregarError("el sistema " + nombreSistema + " no tuvo contenidos validos");
                    continue;
                }

                resultado.SistemasDrones.AgregarAlFinal(sistema);
            }
        }

        private void CargarListaMensajes(XmlNode nodoConfig, ResultadoCargaConfigXml resultado)
        {
            XmlNodeList? nodosMensaje = nodoConfig.SelectNodes("listaMensajes/Mensaje");
            if (nodosMensaje == null)
            {
                resultado.AgregarError("no se encontro la seccion listaMensajes");
                return;
            }

            if (nodosMensaje.Count == 0)
            {
                resultado.AgregarError("listaMensajes no contiene nodos Mensaje");
                return;
            }

            for (int i = 0; i < nodosMensaje.Count; i++)
            {
                XmlNode? nodoMensaje = nodosMensaje.Item(i);
                string nombreMensaje = ObtenerAtributo(nodoMensaje, "nombre");

                if (string.IsNullOrWhiteSpace(nombreMensaje))
                {
                    resultado.AgregarError("se encontro un Mensaje sin atributo nombre");
                    continue;
                }

                string nombreSistema = ObtenerTextoNormalizado(nodoMensaje?.SelectSingleNode("sistemaDrones"));
                if (string.IsNullOrWhiteSpace(nombreSistema))
                {
                    resultado.AgregarError("el mensaje " + nombreMensaje + " no define sistemaDrones");
                    continue;
                }

                MensajeConfig mensaje = new MensajeConfig(nombreMensaje, nombreSistema);
                XmlNodeList? nodosInstruccion = nodoMensaje?.SelectNodes("instrucciones/instruccion");

                if (nodosInstruccion == null || nodosInstruccion.Count == 0)
                {
                    resultado.AgregarError("el mensaje " + nombreMensaje + " no contiene instrucciones");
                    continue;
                }

                for (int j = 0; j < nodosInstruccion.Count; j++)
                {
                    XmlNode? nodoInstruccion = nodosInstruccion.Item(j);
                    string nombreDron = ObtenerAtributo(nodoInstruccion, "dron");

                    if (string.IsNullOrWhiteSpace(nombreDron))
                    {
                        resultado.AgregarError("mensaje " + nombreMensaje + ": instruccion sin atributo dron");
                        continue;
                    }

                    string valorTexto = ObtenerTextoNormalizado(nodoInstruccion);
                    int alturaObjetivo;

                    if (!int.TryParse(valorTexto, out alturaObjetivo))
                    {
                        resultado.AgregarError("mensaje " + nombreMensaje + ": altura invalida para dron " + nombreDron);
                        continue;
                    }

                    mensaje.Instrucciones.AgregarAlFinal(new InstruccionMensajeConfig(nombreDron, alturaObjetivo));
                }

                if (mensaje.Instrucciones.Cantidad == 0)
                {
                    resultado.AgregarError("el mensaje " + nombreMensaje + " no tuvo instrucciones validas");
                    continue;
                }

                resultado.Mensajes.AgregarAlFinal(mensaje);
            }
        }

        private string ObtenerTextoNormalizado(XmlNode? nodo)
        {
            if (nodo == null || nodo.InnerText == null)
                return string.Empty;

            return nodo.InnerText.Trim();
        }

        private string ObtenerSimboloAltura(XmlNode? nodoAltura)
        {
            if (nodoAltura == null || nodoAltura.InnerText == null)
                return string.Empty;

            string textoCrudo = nodoAltura.InnerText;
            string textoNormalizado = textoCrudo.Trim();

            if (!string.IsNullOrEmpty(textoNormalizado))
                return textoNormalizado;

            if (textoCrudo.Length > 0)
                return " ";

            return string.Empty;
        }

        private string ObtenerAtributo(XmlNode? nodo, string nombreAtributo)
        {
            if (nodo == null || nodo.Attributes == null)
                return string.Empty;

            XmlAttribute? atributo = nodo.Attributes[nombreAtributo];
            if (atributo == null)
                return string.Empty;

            return atributo.Value == null ? string.Empty : atributo.Value.Trim();
        }

        private bool TryParseNodoEntero(XmlNode? nodoPadre, string nombreNodoHijo, out int valor)
        {
            valor = 0;
            if (nodoPadre == null)
                return false;

            XmlNode? nodoHijo = nodoPadre.SelectSingleNode(nombreNodoHijo);
            if (nodoHijo == null)
                return false;

            string texto = ObtenerTextoNormalizado(nodoHijo);
            return int.TryParse(texto, out valor);
        }

        private void ValidarReferenciasCruzadas(ResultadoCargaConfigXml resultado, EstadoSistemaService? estadoSistema)
        {
            NodoGenerico<SistemaDronesConfig> nodoSistema = resultado.SistemasDrones.Cabeza;

            while (nodoSistema != null)
            {
                SistemaDronesConfig sistema = nodoSistema.Dato;

                if (estadoSistema != null && ExisteSistema(estadoSistema.SistemasDrones, sistema.NombreSistema))
                {
                    resultado.AgregarError("el sistema " + sistema.NombreSistema + " ya existe en memoria");
                }

                if (sistema.CantidadDrones > 0 && sistema.Contenidos.Cantidad != sistema.CantidadDrones)
                {
                    resultado.AgregarError("el sistema " + sistema.NombreSistema + " reporta cantidadDrones=" + sistema.CantidadDrones +
                        " pero se cargaron " + sistema.Contenidos.Cantidad + " contenidos validos");
                }

                NodoGenerico<ContenidoSistemaDrones> nodoContenido = sistema.Contenidos.Cabeza;
                while (nodoContenido != null)
                {
                    if (!ExisteNombreDroneTotal(resultado, estadoSistema, nodoContenido.Dato.NombreDron))
                    {
                        resultado.AgregarError("el sistema " + sistema.NombreSistema + " usa dron inexistente: " + nodoContenido.Dato.NombreDron);
                    }

                    nodoContenido = nodoContenido.Siguiente;
                }

                nodoSistema = nodoSistema.Siguiente;
            }

            NodoGenerico<MensajeConfig> nodoMensaje = resultado.Mensajes.Cabeza;
            while (nodoMensaje != null)
            {
                MensajeConfig mensaje = nodoMensaje.Dato;

                if (estadoSistema != null && ExisteMensaje(estadoSistema.Mensajes, mensaje.NombreMensaje))
                {
                    resultado.AgregarError("el mensaje " + mensaje.NombreMensaje + " ya existe en memoria");
                }

                SistemaDronesConfig? sistema = BuscarSistema(resultado.SistemasDrones, mensaje.NombreSistemaDrones);
                if (sistema == null && estadoSistema != null)
                {
                    sistema = BuscarSistema(estadoSistema.SistemasDrones, mensaje.NombreSistemaDrones);
                }

                if (sistema == null)
                {
                    resultado.AgregarError("el mensaje " + mensaje.NombreMensaje + " referencia un sistema inexistente: " + mensaje.NombreSistemaDrones);
                    nodoMensaje = nodoMensaje.Siguiente;
                    continue;
                }

                NodoGenerico<InstruccionMensajeConfig> nodoInstruccion = mensaje.Instrucciones.Cabeza;
                while (nodoInstruccion != null)
                {
                    string nombreDron = nodoInstruccion.Dato.NombreDron;
                    int altura = nodoInstruccion.Dato.AlturaObjetivo;

                    if (!ExisteNombreDroneTotal(resultado, estadoSistema, nombreDron))
                    {
                        resultado.AgregarError("mensaje " + mensaje.NombreMensaje + ": dron inexistente " + nombreDron);
                    }
                    else if (!ExisteDronEnSistema(sistema, nombreDron))
                    {
                        resultado.AgregarError("mensaje " + mensaje.NombreMensaje + ": el dron " + nombreDron + " no pertenece al sistema " + mensaje.NombreSistemaDrones);
                    }

                    if (altura < 1 || altura > sistema.AlturaMaxima)
                    {
                        resultado.AgregarError("mensaje " + mensaje.NombreMensaje + ": altura fuera de rango para dron " + nombreDron);
                    }

                    nodoInstruccion = nodoInstruccion.Siguiente;
                }

                nodoMensaje = nodoMensaje.Siguiente;
            }

            NodoGenerico<string> nodoDronTemporal = resultado.NombresDrones.Cabeza;
            while (nodoDronTemporal != null)
            {
                if (estadoSistema != null && ExisteNombreDrone(estadoSistema.NombresDrones, nodoDronTemporal.Dato))
                {
                    resultado.AgregarError("el dron " + nodoDronTemporal.Dato + " ya existe en memoria");
                }

                nodoDronTemporal = nodoDronTemporal.Siguiente;
            }
        }

        private bool ExisteNombreDroneTotal(ResultadoCargaConfigXml resultado, EstadoSistemaService? estadoSistema, string nombreDron)
        {
            if (ExisteNombreDrone(resultado.NombresDrones, nombreDron))
                return true;

            if (estadoSistema != null && ExisteNombreDrone(estadoSistema.NombresDrones, nombreDron))
                return true;

            return false;
        }

        private bool ExisteNombreDrone(ListaEnlazada<string> listaDrones, string nombreDron)
        {
            NodoGenerico<string> actual = listaDrones.Cabeza;

            while (actual != null)
            {
                if (string.Equals(actual.Dato, nombreDron, StringComparison.OrdinalIgnoreCase))
                    return true;

                actual = actual.Siguiente;
            }

            return false;
        }

        private bool ExisteSistema(ListaEnlazada<SistemaDronesConfig> sistemas, string nombreSistema)
        {
            return BuscarSistema(sistemas, nombreSistema) != null;
        }

        private bool ExisteMensaje(ListaEnlazada<MensajeConfig> mensajes, string nombreMensaje)
        {
            NodoGenerico<MensajeConfig> actual = mensajes.Cabeza;

            while (actual != null)
            {
                if (string.Equals(actual.Dato.NombreMensaje, nombreMensaje, StringComparison.OrdinalIgnoreCase))
                    return true;

                actual = actual.Siguiente;
            }

            return false;
        }

        private SistemaDronesConfig? BuscarSistema(ListaEnlazada<SistemaDronesConfig> sistemas, string nombreSistema)
        {
            NodoGenerico<SistemaDronesConfig> actual = sistemas.Cabeza;

            while (actual != null)
            {
                if (string.Equals(actual.Dato.NombreSistema, nombreSistema, StringComparison.OrdinalIgnoreCase))
                    return actual.Dato;

                actual = actual.Siguiente;
            }

            return null;
        }

        private bool ExisteDronEnSistema(SistemaDronesConfig sistema, string nombreDron)
        {
            NodoGenerico<ContenidoSistemaDrones> actual = sistema.Contenidos.Cabeza;

            while (actual != null)
            {
                if (string.Equals(actual.Dato.NombreDron, nombreDron, StringComparison.OrdinalIgnoreCase))
                    return true;

                actual = actual.Siguiente;
            }

            return false;
        }

        private void TransferirAlEstado(ResultadoCargaConfigXml resultado, EstadoSistemaService estadoSistema)
        {
            NodoGenerico<string> nodoDron = resultado.NombresDrones.Cabeza;
            while (nodoDron != null)
            {
                estadoSistema.NombresDrones.AgregarAlFinal(nodoDron.Dato);
                nodoDron = nodoDron.Siguiente;
            }

            NodoGenerico<SistemaDronesConfig> nodoSistema = resultado.SistemasDrones.Cabeza;
            while (nodoSistema != null)
            {
                estadoSistema.SistemasDrones.AgregarAlFinal(nodoSistema.Dato);
                nodoSistema = nodoSistema.Siguiente;
            }

            NodoGenerico<MensajeConfig> nodoMensaje = resultado.Mensajes.Cabeza;
            while (nodoMensaje != null)
            {
                estadoSistema.Mensajes.AgregarAlFinal(nodoMensaje.Dato);
                nodoMensaje = nodoMensaje.Siguiente;
            }
        }
    }
}
