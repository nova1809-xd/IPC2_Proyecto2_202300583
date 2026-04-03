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

            try
            {
                documento.Load(rutaArchivoXml);
            }
            catch (Exception ex)
            {
                resultado.AgregarError("no se pudo cargar el xml: " + ex.Message);
                return resultado;
            }

            XmlNode nodoConfig = documento.SelectSingleNode("/config");
            if (nodoConfig == null)
            {
                resultado.AgregarError("el xml no contiene el nodo raiz config");
                return resultado;
            }

            CargarListaDrones(nodoConfig, resultado);
            CargarListaSistemas(nodoConfig, resultado);
            CargarListaMensajes(nodoConfig, resultado);

            return resultado;
        }

        private void CargarListaDrones(XmlNode nodoConfig, ResultadoCargaConfigXml resultado)
        {
            XmlNodeList nodosDron = nodoConfig.SelectNodes("listaDrones/dron");
            if (nodosDron == null)
            {
                resultado.AgregarError("no se encontro la seccion listaDrones");
                return;
            }

            foreach (XmlNode nodoDron in nodosDron)
            {
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

            if (resultado.NombresDrones.EstaVacia)
            {
                resultado.AgregarError("listaDrones no contiene drones validos");
            }
        }

        private void CargarListaSistemas(XmlNode nodoConfig, ResultadoCargaConfigXml resultado)
        {
            XmlNodeList nodosSistema = nodoConfig.SelectNodes("listaSistemasDrones/sistemaDrones");
            if (nodosSistema == null)
            {
                resultado.AgregarError("no se encontro la seccion listaSistemasDrones");
                return;
            }

            foreach (XmlNode nodoSistema in nodosSistema)
            {
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

                XmlNodeList nodosContenido = nodoSistema.SelectNodes("contenido");
                if (nodosContenido == null || nodosContenido.Count == 0)
                {
                    resultado.AgregarError("el sistema " + nombreSistema + " no tiene nodos contenido");
                    continue;
                }

                foreach (XmlNode nodoContenido in nodosContenido)
                {
                    string nombreDron = ObtenerTextoNormalizado(nodoContenido.SelectSingleNode("dron"));
                    if (string.IsNullOrWhiteSpace(nombreDron))
                    {
                        resultado.AgregarError("el sistema " + nombreSistema + " tiene un contenido sin dron");
                        continue;
                    }

                    if (!ExisteNombreDrone(resultado.NombresDrones, nombreDron))
                    {
                        resultado.AgregarError("el sistema " + nombreSistema + " usa dron inexistente: " + nombreDron);
                        continue;
                    }

                    ContenidoSistemaDrones contenido = new ContenidoSistemaDrones(nombreDron);

                    XmlNodeList nodosAltura = nodoContenido.SelectNodes("alturas/altura");
                    if (nodosAltura == null || nodosAltura.Count == 0)
                    {
                        resultado.AgregarError("el sistema " + nombreSistema + " no tiene alturas para dron " + nombreDron);
                        continue;
                    }

                    foreach (XmlNode nodoAltura in nodosAltura)
                    {
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

                        string simbolo = ObtenerTextoNormalizado(nodoAltura);
                        if (string.IsNullOrWhiteSpace(simbolo))
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

                if (sistema.Contenidos.Cantidad != sistema.CantidadDrones)
                {
                    resultado.AgregarError("el sistema " + nombreSistema + " reporta cantidadDrones=" + sistema.CantidadDrones +
                        " pero se cargaron " + sistema.Contenidos.Cantidad + " contenidos validos");
                }

                resultado.SistemasDrones.AgregarAlFinal(sistema);
            }
        }

        private void CargarListaMensajes(XmlNode nodoConfig, ResultadoCargaConfigXml resultado)
        {
            XmlNodeList nodosMensaje = nodoConfig.SelectNodes("listaMensajes/Mensaje");
            if (nodosMensaje == null)
            {
                resultado.AgregarError("no se encontro la seccion listaMensajes");
                return;
            }

            foreach (XmlNode nodoMensaje in nodosMensaje)
            {
                string nombreMensaje = ObtenerAtributo(nodoMensaje, "nombre");
                if (string.IsNullOrWhiteSpace(nombreMensaje))
                {
                    resultado.AgregarError("se encontro un Mensaje sin atributo nombre");
                    continue;
                }

                string nombreSistema = ObtenerTextoNormalizado(nodoMensaje.SelectSingleNode("sistemaDrones"));
                if (string.IsNullOrWhiteSpace(nombreSistema))
                {
                    resultado.AgregarError("el mensaje " + nombreMensaje + " no define sistemaDrones");
                    continue;
                }

                SistemaDronesConfig sistema = BuscarSistema(resultado.SistemasDrones, nombreSistema);
                if (sistema == null)
                {
                    resultado.AgregarError("el mensaje " + nombreMensaje + " referencia un sistema inexistente: " + nombreSistema);
                    continue;
                }

                MensajeConfig mensaje = new MensajeConfig(nombreMensaje, nombreSistema);

                XmlNodeList nodosInstruccion = nodoMensaje.SelectNodes("instrucciones/instruccion");
                if (nodosInstruccion == null || nodosInstruccion.Count == 0)
                {
                    resultado.AgregarError("el mensaje " + nombreMensaje + " no contiene instrucciones");
                    continue;
                }

                foreach (XmlNode nodoInstruccion in nodosInstruccion)
                {
                    string nombreDron = ObtenerAtributo(nodoInstruccion, "dron");
                    if (string.IsNullOrWhiteSpace(nombreDron))
                    {
                        resultado.AgregarError("mensaje " + nombreMensaje + ": instruccion sin atributo dron");
                        continue;
                    }

                    if (!ExisteNombreDrone(resultado.NombresDrones, nombreDron))
                    {
                        resultado.AgregarError("mensaje " + nombreMensaje + ": dron inexistente " + nombreDron);
                        continue;
                    }

                    if (!ExisteDronEnSistema(sistema, nombreDron))
                    {
                        resultado.AgregarError("mensaje " + nombreMensaje + ": el dron " + nombreDron +
                            " no pertenece al sistema " + nombreSistema);
                        continue;
                    }

                    string valorTexto = ObtenerTextoNormalizado(nodoInstruccion);
                    int alturaObjetivo;

                    if (!int.TryParse(valorTexto, out alturaObjetivo))
                    {
                        resultado.AgregarError("mensaje " + nombreMensaje + ": altura invalida para dron " + nombreDron);
                        continue;
                    }

                    if (alturaObjetivo < 1 || alturaObjetivo > sistema.AlturaMaxima)
                    {
                        resultado.AgregarError("mensaje " + nombreMensaje + ": altura fuera de rango para dron " + nombreDron);
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

        private string ObtenerTextoNormalizado(XmlNode nodo)
        {
            if (nodo == null || nodo.InnerText == null)
                return string.Empty;

            return nodo.InnerText.Trim();
        }

        private string ObtenerAtributo(XmlNode nodo, string nombreAtributo)
        {
            if (nodo == null || nodo.Attributes == null)
                return string.Empty;

            XmlAttribute atributo = nodo.Attributes[nombreAtributo];
            if (atributo == null)
                return string.Empty;

            return atributo.Value == null ? string.Empty : atributo.Value.Trim();
        }

        private bool TryParseNodoEntero(XmlNode nodoPadre, string nombreNodoHijo, out int valor)
        {
            valor = 0;
            if (nodoPadre == null)
                return false;

            XmlNode nodoHijo = nodoPadre.SelectSingleNode(nombreNodoHijo);
            if (nodoHijo == null)
                return false;

            string texto = ObtenerTextoNormalizado(nodoHijo);
            return int.TryParse(texto, out valor);
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

        private SistemaDronesConfig BuscarSistema(ListaEnlazada<SistemaDronesConfig> sistemas, string nombreSistema)
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
    }
}
