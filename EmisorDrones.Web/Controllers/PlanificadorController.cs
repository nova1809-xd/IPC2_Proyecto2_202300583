using System.Text;
using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.Services;
using EmisorDrones.CoreBusiness.TDAs;
using Microsoft.AspNetCore.Mvc;

namespace EmisorDrones.Web.Controllers
{
    public class PlanificadorController : Controller
    {
        private readonly MotorOptimizacionService _motorOptimizacionService;
        private readonly XmlReaderService _xmlReaderService;
        private readonly XmlWriterService _xmlWriterService;
        private readonly GraphvizService _graphvizService;
        private readonly EstadoSistemaService _estadoSistemaService;

        public PlanificadorController(
            MotorOptimizacionService motorOptimizacionService,
            XmlReaderService xmlReaderService,
            XmlWriterService xmlWriterService,
            GraphvizService graphvizService,
            EstadoSistemaService estadoSistemaService)
        {
            _motorOptimizacionService = motorOptimizacionService;
            _xmlReaderService = xmlReaderService;
            _xmlWriterService = xmlWriterService;
            _graphvizService = graphvizService;
            _estadoSistemaService = estadoSistemaService;
        }

        [HttpGet]
        public IActionResult CargarXmlEntrada()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CargarXmlEntrada(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo entrada.xml valido.";
                return View();
            }

            if (!archivoXml.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "El archivo seleccionado no es XML.";
                return View();
            }

            string rutaTemporal = Path.GetTempFileName();

            try
            {
                using (FileStream stream = new FileStream(rutaTemporal, FileMode.Create, FileAccess.Write))
                {
                    archivoXml.CopyTo(stream);
                }

                ResultadoCargaConfigXml resultado = _xmlReaderService.CargarEntradaIncremental(rutaTemporal, _estadoSistemaService);

                if (!resultado.Exito)
                {
                    ViewBag.Errores = resultado.Errores;
                    TempData["Error"] = "El XML contiene errores de validacion.";
                    return View();
                }

                TempData["Mensaje"] = "Archivo XML cargado y agregado incrementalmente en memoria.";
                return RedirectToAction(nameof(GestionMensajes));
            }
            finally
            {
                if (System.IO.File.Exists(rutaTemporal))
                    System.IO.File.Delete(rutaTemporal);
            }
        }

        [HttpGet]
        public IActionResult VerSistemaGrafico(string? nombreSistema)
        {
            SistemaDronesConfig? sistema = BuscarSistema(nombreSistema);
            if (sistema == null)
            {
                TempData["Error"] = "No hay sistemas cargados. Primero cargue entrada.xml.";
                return RedirectToAction(nameof(CargarXmlEntrada));
            }

            string dot = _graphvizService.GenerarGrafoSistema(sistema);

            ViewBag.Titulo = "Visualizacion de Sistema de Drones";
            ViewBag.Subtitulo = sistema.NombreSistema;
            ViewBag.Dot = dot;
            ViewBag.Sistemas = _estadoSistemaService.SistemasDrones;
            ViewBag.NombreSistemaSeleccionado = sistema.NombreSistema;

            return View();
        }

        [HttpGet]
        public IActionResult VerInstruccionesGraficas(string? nombreMensaje, string? nombreSistema)
        {
            if (_estadoSistemaService.Mensajes.EstaVacia)
            {
                TempData["Error"] = "No hay mensajes cargados. Primero cargue entrada.xml.";
                return RedirectToAction(nameof(CargarXmlEntrada));
            }

            _estadoSistemaService.Mensajes.OrdenarAlfabeticamente(m => m.NombreMensaje);

            MensajeConfig? mensaje = null;

            if (!string.IsNullOrWhiteSpace(nombreSistema))
            {
                mensaje = BuscarPrimerMensajePorSistema(nombreSistema);

                if (mensaje == null)
                {
                    TempData["Error"] = "El sistema seleccionado no tiene mensajes configurados.";
                    return RedirectToAction(nameof(VerSistemaGrafico), new { nombreSistema });
                }
            }
            else
            {
                mensaje = BuscarMensaje(nombreMensaje);
            }

            if (mensaje == null)
            {
                mensaje = BuscarMensaje(null);
            }

            if (mensaje == null)
            {
                TempData["Error"] = "No hay mensajes cargados. Primero cargue entrada.xml.";
                return RedirectToAction(nameof(CargarXmlEntrada));
            }

            SistemaDronesConfig? sistema = BuscarSistema(mensaje.NombreSistemaDrones);
            if (sistema == null)
            {
                TempData["Error"] = "El mensaje seleccionado referencia un sistema inexistente.";
                return RedirectToAction(nameof(CargarXmlEntrada));
            }

            ListaEnlazada<Dron> drones = CrearDronesDesdeSistema(sistema);
            ListaEnlazada<Instruccion> instrucciones = CrearInstruccionesDesdeMensaje(mensaje, drones);
            ListaEnlazada<InstanteTiempo> timeline = _motorOptimizacionService.GenerarTimeline(instrucciones, drones);

            string dot = _graphvizService.GenerarGrafoInstrucciones(timeline, mensaje.NombreMensaje);

            ViewBag.Titulo = "Visualizacion de Instrucciones Optimas";
            ViewBag.Subtitulo = "Timeline del mensaje " + mensaje.NombreMensaje + " | Sistema " + mensaje.NombreSistemaDrones;
            ViewBag.Dot = dot;
            ViewBag.TiempoOptimo = ObtenerUltimoSegundo(timeline);
            ViewBag.NombreMensajeSeleccionado = mensaje.NombreMensaje;
            ViewBag.Sistemas = _estadoSistemaService.SistemasDrones;
            ViewBag.NombreSistemaSeleccionado = mensaje.NombreSistemaDrones;

            return View();
        }

        [HttpGet]
        public IActionResult GestionMensajes(string? nombreMensaje)
        {
            if (_estadoSistemaService.Mensajes.EstaVacia)
            {
                TempData["Error"] = "No hay mensajes cargados. Primero cargue entrada.xml.";
                return RedirectToAction(nameof(CargarXmlEntrada));
            }

            _estadoSistemaService.Mensajes.OrdenarAlfabeticamente(m => m.NombreMensaje);

            MensajeConfig? mensajeSeleccionado = BuscarMensaje(nombreMensaje);
            if (mensajeSeleccionado == null)
                mensajeSeleccionado = _estadoSistemaService.Mensajes.Cabeza.Dato;

            SistemaDronesConfig? sistema = BuscarSistema(mensajeSeleccionado.NombreSistemaDrones);
            if (sistema == null)
            {
                TempData["Error"] = "El mensaje seleccionado referencia un sistema inexistente.";
                return RedirectToAction(nameof(CargarXmlEntrada));
            }

            ListaEnlazada<Dron> drones = CrearDronesDesdeSistema(sistema);
            ListaEnlazada<Instruccion> instrucciones = CrearInstruccionesDesdeMensaje(mensajeSeleccionado, drones);
            ListaEnlazada<InstanteTiempo> timeline = _motorOptimizacionService.GenerarTimeline(instrucciones, drones);

            string dot = _graphvizService.GenerarGrafoInstrucciones(timeline, mensajeSeleccionado.NombreMensaje);

            string textoReconstruido = ReconstruirTextoMensaje(timeline, sistema);
            if (string.IsNullOrEmpty(textoReconstruido))
                textoReconstruido = mensajeSeleccionado.NombreMensaje; // Fallback si no se puede reconstruir

            ViewBag.NombreMensajeSeleccionado = mensajeSeleccionado.NombreMensaje;
            ViewBag.NombreSistema = mensajeSeleccionado.NombreSistemaDrones;
            ViewBag.TextoMensaje = textoReconstruido;
            ViewBag.TiempoOptimo = ObtenerUltimoSegundo(timeline);
            ViewBag.Dot = dot;

            return View(_estadoSistemaService.Mensajes);
        }

        [HttpGet]
        public IActionResult ProbarIpc2()
        {
            MensajeConfig? mensaje = BuscarMensaje(null);
            if (mensaje == null)
                return BadRequest("No hay mensajes cargados en memoria.");

            SistemaDronesConfig? sistema = BuscarSistema(mensaje.NombreSistemaDrones);
            if (sistema == null)
                return BadRequest("El mensaje no referencia un sistema valido.");

            ListaEnlazada<Dron> drones = CrearDronesDesdeSistema(sistema);
            ListaEnlazada<Instruccion> instrucciones = CrearInstruccionesDesdeMensaje(mensaje, drones);

            ListaEnlazada<InstanteTiempo> timeline = _motorOptimizacionService.GenerarTimeline(instrucciones, drones);

            string json = ConvertirTimelineAJson(timeline);
            return Content(json, "application/json", Encoding.UTF8);
        }

        [HttpGet]
        public IActionResult DescargarSalidaXml(string? nombreMensaje)
        {
            if (_estadoSistemaService.Mensajes.EstaVacia)
                return BadRequest("No hay mensajes cargados para generar salida XML.");

            ListaEnlazada<SalidaMensajeXml> mensajesSalida = new ListaEnlazada<SalidaMensajeXml>();
            string? errorConstruccion = null;

            if (!string.IsNullOrWhiteSpace(nombreMensaje))
            {
                MensajeConfig? mensaje = BuscarMensaje(nombreMensaje);
                if (mensaje == null)
                    return BadRequest("No se encontró el mensaje solicitado para generar salida XML.");

                if (!TryConstruirSalidaMensaje(mensaje, out SalidaMensajeXml? salidaMensaje, out errorConstruccion))
                    return BadRequest(errorConstruccion ?? "No se pudo construir la salida XML del mensaje solicitado.");

                mensajesSalida.AgregarAlFinal(salidaMensaje);
            }
            else
            {
                NodoGenerico<MensajeConfig> nodoMensaje = _estadoSistemaService.Mensajes.Cabeza;
                while (nodoMensaje != null)
                {
                    MensajeConfig mensajeActual = nodoMensaje.Dato;

                    if (!TryConstruirSalidaMensaje(mensajeActual, out SalidaMensajeXml? salidaMensaje, out errorConstruccion))
                    {
                        return BadRequest(errorConstruccion ?? "No se pudo construir la salida XML para todos los mensajes.");
                    }

                    mensajesSalida.AgregarAlFinal(salidaMensaje);
                    nodoMensaje = nodoMensaje.Siguiente;
                }
            }

            if (mensajesSalida.EstaVacia)
                return BadRequest("No hay mensajes válidos para generar salida XML.");

            string carpetaSalida = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "salidas");
            if (!Directory.Exists(carpetaSalida))
                Directory.CreateDirectory(carpetaSalida);

            string nombreArchivo = "salida.xml";
            string rutaSalida = Path.Combine(carpetaSalida, nombreArchivo);

            _xmlWriterService.GenerarSalidaXml(mensajesSalida, rutaSalida);

            return PhysicalFile(rutaSalida, "application/xml", nombreArchivo);
        }

        private bool TryConstruirSalidaMensaje(
            MensajeConfig mensaje,
            out SalidaMensajeXml salidaMensaje,
            out string? error)
        {
            salidaMensaje = new SalidaMensajeXml(
                string.Empty,
                string.Empty,
                string.Empty,
                new ListaEnlazada<InstanteTiempo>());
            error = null;

            SistemaDronesConfig? sistema = BuscarSistema(mensaje.NombreSistemaDrones);
            if (sistema == null)
            {
                error = "El mensaje " + mensaje.NombreMensaje + " referencia un sistema inválido.";
                return false;
            }

            ListaEnlazada<Dron> drones = CrearDronesDesdeSistema(sistema);
            ListaEnlazada<Instruccion> instrucciones = CrearInstruccionesDesdeMensaje(mensaje, drones);
            ListaEnlazada<InstanteTiempo> timeline = _motorOptimizacionService.GenerarTimeline(instrucciones, drones);

            salidaMensaje = new SalidaMensajeXml(
                mensaje.NombreMensaje,
                mensaje.NombreSistemaDrones,
                mensaje.NombreMensaje,
                timeline);

            return true;
        }

        private string ConvertirTimelineAJson(ListaEnlazada<InstanteTiempo> timeline)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"instantes\":[");

            int i = 0;
            while (i < timeline.Cantidad)
            {
                InstanteTiempo instante = timeline.ObtenerEnPosicion(i);

                sb.Append("{");
                sb.Append("\"segundo\":");
                sb.Append(instante.Segundo);
                sb.Append(",\"acciones\":[");

                int j = 0;
                while (j < instante.Acciones.Cantidad)
                {
                    AccionDron accion = instante.Acciones.ObtenerEnPosicion(j);

                    sb.Append("{");
                    sb.Append("\"nombreDron\":\"");
                    sb.Append(EscapeJson(accion.NombreDron));
                    sb.Append("\",");
                    sb.Append("\"movimiento\":\"");
                    sb.Append(EscapeJson(accion.Movimiento));
                    sb.Append("\"");
                    sb.Append("}");

                    if (j < instante.Acciones.Cantidad - 1)
                        sb.Append(",");

                    j++;
                }

                sb.Append("]");
                sb.Append("}");

                if (i < timeline.Cantidad - 1)
                    sb.Append(",");

                i++;
            }

            sb.Append("]");
            sb.Append("}");

            return sb.ToString();
        }

        private string EscapeJson(string valor)
        {
            if (valor == null)
                return string.Empty;

            return valor.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private SistemaDronesConfig? BuscarSistema(string? nombreSistema)
        {
            NodoGenerico<SistemaDronesConfig> actual = _estadoSistemaService.SistemasDrones.Cabeza;

            if (string.IsNullOrWhiteSpace(nombreSistema))
                return actual != null ? actual.Dato : null;

            while (actual != null)
            {
                if (string.Equals(actual.Dato.NombreSistema, nombreSistema, StringComparison.OrdinalIgnoreCase))
                    return actual.Dato;

                actual = actual.Siguiente;
            }

            return null;
        }

        private MensajeConfig? BuscarMensaje(string? nombreMensaje)
        {
            NodoGenerico<MensajeConfig> actual = _estadoSistemaService.Mensajes.Cabeza;

            if (string.IsNullOrWhiteSpace(nombreMensaje))
                return actual != null ? actual.Dato : null;

            while (actual != null)
            {
                if (string.Equals(actual.Dato.NombreMensaje, nombreMensaje, StringComparison.OrdinalIgnoreCase))
                    return actual.Dato;

                actual = actual.Siguiente;
            }

            return null;
        }

        private MensajeConfig? BuscarPrimerMensajePorSistema(string? nombreSistema)
        {
            if (string.IsNullOrWhiteSpace(nombreSistema))
                return null;

            NodoGenerico<MensajeConfig> actual = _estadoSistemaService.Mensajes.Cabeza;
            while (actual != null)
            {
                if (string.Equals(actual.Dato.NombreSistemaDrones, nombreSistema, StringComparison.OrdinalIgnoreCase))
                    return actual.Dato;

                actual = actual.Siguiente;
            }

            return null;
        }

        private ListaEnlazada<Dron> CrearDronesDesdeSistema(SistemaDronesConfig sistema)
        {
            ListaEnlazada<Dron> drones = new ListaEnlazada<Dron>();
            NodoGenerico<ContenidoSistemaDrones> actualContenido = sistema.Contenidos.Cabeza;
            int id = 1;

            while (actualContenido != null)
            {
                int alturaMinima = 1;
                int alturaMaxima = sistema.AlturaMaxima;

                NodoGenerico<AlturaSimbolo> actualAltura = actualContenido.Dato.Alturas.Cabeza;
                bool primeraAltura = true;
                while (actualAltura != null)
                {
                    int valor = actualAltura.Dato.ValorAltura;
                    if (primeraAltura)
                    {
                        alturaMinima = valor;
                        alturaMaxima = valor;
                        primeraAltura = false;
                    }
                    else
                    {
                        if (valor < alturaMinima)
                            alturaMinima = valor;

                        if (valor > alturaMaxima)
                            alturaMaxima = valor;
                    }

                    actualAltura = actualAltura.Siguiente;
                }

                Dron dron = new Dron(id, actualContenido.Dato.NombreDron, alturaMinima, alturaMinima, alturaMaxima);
                drones.AgregarAlFinal(dron);

                id++;
                actualContenido = actualContenido.Siguiente;
            }

            return drones;
        }

        private ListaEnlazada<Instruccion> CrearInstruccionesDesdeMensaje(MensajeConfig mensaje, ListaEnlazada<Dron> drones)
        {
            ListaEnlazada<Instruccion> instrucciones = new ListaEnlazada<Instruccion>();

            NodoGenerico<InstruccionMensajeConfig> actual = mensaje.Instrucciones.Cabeza;
            while (actual != null)
            {
                int dronId = BuscarIdDronPorNombre(drones, actual.Dato.NombreDron);
                if (dronId > 0)
                {
                    instrucciones.AgregarAlFinal(
                        new Instruccion(TipoInstruccion.EmitirLuz, 0, dronId, actual.Dato.AlturaObjetivo));
                }

                actual = actual.Siguiente;
            }

            return instrucciones;
        }

        private int BuscarIdDronPorNombre(ListaEnlazada<Dron> drones, string nombreDron)
        {
            NodoGenerico<Dron> actual = drones.Cabeza;

            while (actual != null)
            {
                if (string.Equals(actual.Dato.NombreDron, nombreDron, StringComparison.OrdinalIgnoreCase))
                    return actual.Dato.Id;

                actual = actual.Siguiente;
            }

            return -1;
        }

        private int ObtenerUltimoSegundo(ListaEnlazada<InstanteTiempo> timeline)
        {
            int ultimo = 0;
            NodoGenerico<InstanteTiempo> actual = timeline.Cabeza;

            while (actual != null)
            {
                if (actual.Dato.Segundo > ultimo)
                    ultimo = actual.Dato.Segundo;

                actual = actual.Siguiente;
            }

            return ultimo;
        }

        private string ReconstruirTextoMensaje(ListaEnlazada<InstanteTiempo> timeline, SistemaDronesConfig sistema)
        {
            StringBuilder sb = new StringBuilder();

            if (timeline.EstaVacia)
                return string.Empty;

            NodoGenerico<InstanteTiempo> nodoInstante = timeline.Cabeza;
            while (nodoInstante != null)
            {
                InstanteTiempo instante = nodoInstante.Dato;

                if (instante.Acciones != null && !instante.Acciones.EstaVacia)
                {
                    NodoGenerico<AccionDron> nodoAccion = instante.Acciones.Cabeza;
                    while (nodoAccion != null)
                    {
                        AccionDron accion = nodoAccion.Dato;

                        string simbolo = BuscarSimboloEnSistema(sistema, accion.NombreDron, accion.Movimiento);
                        sb.Append(simbolo);

                        nodoAccion = nodoAccion.Siguiente;
                    }
                }

                nodoInstante = nodoInstante.Siguiente;
            }

            return sb.ToString();
        }

        private string BuscarSimboloEnSistema(SistemaDronesConfig sistema, string nombreDron, string altura)
        {
            NodoGenerico<ContenidoSistemaDrones> nodoContenido = sistema.Contenidos.Cabeza;

            while (nodoContenido != null)
            {
                ContenidoSistemaDrones contenido = nodoContenido.Dato;

                if (string.Equals(contenido.NombreDron, nombreDron, StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(altura, out int alturaNumero))
                    {
                        NodoGenerico<AlturaSimbolo> nodoAltura = contenido.Alturas.Cabeza;
                        while (nodoAltura != null)
                        {
                            AlturaSimbolo alturaSimb = nodoAltura.Dato;

                            if (alturaSimb.ValorAltura == alturaNumero)
                                return alturaSimb.Simbolo;

                            nodoAltura = nodoAltura.Siguiente;
                        }
                    }
                    break;
                }

                nodoContenido = nodoContenido.Siguiente;
            }

            return string.Empty;
        }
    }
}
