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
        private readonly XmlWriterService _xmlWriterService;
        private readonly GraphvizService _graphvizService;

        public PlanificadorController(
            MotorOptimizacionService motorOptimizacionService,
            XmlWriterService xmlWriterService,
            GraphvizService graphvizService)
        {
            _motorOptimizacionService = motorOptimizacionService;
            _xmlWriterService = xmlWriterService;
            _graphvizService = graphvizService;
        }

        [HttpGet]
        public IActionResult VerSistemaGrafico()
        {
            SistemaDronesConfig sistema = CrearSistemaMock();
            string dot = _graphvizService.GenerarGrafoSistema(sistema);

            ViewBag.Titulo = "Visualizacion de Sistema de Drones";
            ViewBag.Subtitulo = sistema.NombreSistema;
            ViewBag.Dot = dot;

            return View();
        }

        [HttpGet]
        public IActionResult VerInstruccionesGraficas()
        {
            ListaEnlazada<Dron> drones = CrearDronesMock();
            ListaEnlazada<Instruccion> instrucciones = CrearInstruccionesMock();
            ListaEnlazada<InstanteTiempo> timeline = _motorOptimizacionService.GenerarTimeline(instrucciones, drones);

            string nombreMensaje = "IPC2";
            string dot = _graphvizService.GenerarGrafoInstrucciones(timeline, nombreMensaje);

            ViewBag.Titulo = "Visualizacion de Instrucciones Optimas";
            ViewBag.Subtitulo = "Timeline del mensaje " + nombreMensaje;
            ViewBag.Dot = dot;

            return View();
        }

        [HttpGet]
        public IActionResult ProbarIpc2()
        {
            ListaEnlazada<Dron> drones = CrearDronesMock();
            ListaEnlazada<Instruccion> instrucciones = CrearInstruccionesMock();

            ListaEnlazada<InstanteTiempo> timeline = _motorOptimizacionService.GenerarTimeline(instrucciones, drones);

            string json = ConvertirTimelineAJson(timeline);
            return Content(json, "application/json", Encoding.UTF8);
        }

        [HttpGet]
        public IActionResult DescargarSalidaXml()
        {
            ListaEnlazada<Dron> drones = CrearDronesMock();
            ListaEnlazada<Instruccion> instrucciones = CrearInstruccionesMock();

            ListaEnlazada<InstanteTiempo> timeline = _motorOptimizacionService.GenerarTimeline(instrucciones, drones);

            string carpetaSalida = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "salidas");
            if (!Directory.Exists(carpetaSalida))
                Directory.CreateDirectory(carpetaSalida);

            string nombreArchivo = "salida.xml";
            string rutaSalida = Path.Combine(carpetaSalida, nombreArchivo);

            _xmlWriterService.GenerarSalidaXml(
                timeline,
                rutaSalida,
                "IPC2",
                "SistemaIPC2",
                "IPC2");

            return PhysicalFile(rutaSalida, "application/xml", nombreArchivo);
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

        private ListaEnlazada<Dron> CrearDronesMock()
        {
            ListaEnlazada<Dron> drones = new ListaEnlazada<Dron>();
            drones.AgregarAlFinal(new Dron(1, "DronAlpha", 1, 1, 100));
            drones.AgregarAlFinal(new Dron(2, "DronBeta", 1, 1, 100));
            drones.AgregarAlFinal(new Dron(3, "DronGamma", 1, 1, 100));
            return drones;
        }

        private ListaEnlazada<Instruccion> CrearInstruccionesMock()
        {
            ListaEnlazada<Instruccion> instrucciones = new ListaEnlazada<Instruccion>();
            instrucciones.AgregarAlFinal(new Instruccion(TipoInstruccion.EmitirLuz, 0, 1, 4));
            instrucciones.AgregarAlFinal(new Instruccion(TipoInstruccion.EmitirLuz, 0, 2, 2));
            instrucciones.AgregarAlFinal(new Instruccion(TipoInstruccion.EmitirLuz, 0, 3, 5));
            instrucciones.AgregarAlFinal(new Instruccion(TipoInstruccion.EmitirLuz, 0, 1, 3));
            return instrucciones;
        }

        private SistemaDronesConfig CrearSistemaMock()
        {
            SistemaDronesConfig sistema = new SistemaDronesConfig("SistemaIPC2", 5, 3);

            ContenidoSistemaDrones dronAlpha = new ContenidoSistemaDrones("DronAlpha");
            dronAlpha.Alturas.AgregarAlFinal(new AlturaSimbolo(5, "A"));
            dronAlpha.Alturas.AgregarAlFinal(new AlturaSimbolo(4, "B"));
            dronAlpha.Alturas.AgregarAlFinal(new AlturaSimbolo(3, "C"));
            dronAlpha.Alturas.AgregarAlFinal(new AlturaSimbolo(2, "D"));
            dronAlpha.Alturas.AgregarAlFinal(new AlturaSimbolo(1, "E"));

            ContenidoSistemaDrones dronBeta = new ContenidoSistemaDrones("DronBeta");
            dronBeta.Alturas.AgregarAlFinal(new AlturaSimbolo(5, "F"));
            dronBeta.Alturas.AgregarAlFinal(new AlturaSimbolo(4, "G"));
            dronBeta.Alturas.AgregarAlFinal(new AlturaSimbolo(3, "H"));
            dronBeta.Alturas.AgregarAlFinal(new AlturaSimbolo(2, "I"));
            dronBeta.Alturas.AgregarAlFinal(new AlturaSimbolo(1, "J"));

            ContenidoSistemaDrones dronGamma = new ContenidoSistemaDrones("DronGamma");
            dronGamma.Alturas.AgregarAlFinal(new AlturaSimbolo(5, "K"));
            dronGamma.Alturas.AgregarAlFinal(new AlturaSimbolo(4, "L"));
            dronGamma.Alturas.AgregarAlFinal(new AlturaSimbolo(3, "M"));
            dronGamma.Alturas.AgregarAlFinal(new AlturaSimbolo(2, "N"));
            dronGamma.Alturas.AgregarAlFinal(new AlturaSimbolo(1, "O"));

            sistema.Contenidos.AgregarAlFinal(dronAlpha);
            sistema.Contenidos.AgregarAlFinal(dronBeta);
            sistema.Contenidos.AgregarAlFinal(dronGamma);

            return sistema;
        }
    }
}
