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

        public PlanificadorController(MotorOptimizacionService motorOptimizacionService)
        {
            _motorOptimizacionService = motorOptimizacionService;
        }

        [HttpGet]
        public IActionResult ProbarIpc2()
        {
            ListaEnlazada<Dron> drones = new ListaEnlazada<Dron>();
            drones.AgregarAlFinal(new Dron(1, "DronAlpha", 1, 1, 100));
            drones.AgregarAlFinal(new Dron(2, "DronBeta", 1, 1, 100));
            drones.AgregarAlFinal(new Dron(3, "DronGamma", 1, 1, 100));

            ListaEnlazada<Instruccion> instrucciones = new ListaEnlazada<Instruccion>();
            instrucciones.AgregarAlFinal(new Instruccion(TipoInstruccion.EmitirLuz, 0, 1, 4));
            instrucciones.AgregarAlFinal(new Instruccion(TipoInstruccion.EmitirLuz, 0, 2, 2));
            instrucciones.AgregarAlFinal(new Instruccion(TipoInstruccion.EmitirLuz, 0, 3, 5));
            instrucciones.AgregarAlFinal(new Instruccion(TipoInstruccion.EmitirLuz, 0, 1, 3));

            ListaEnlazada<InstanteTiempo> timeline = _motorOptimizacionService.GenerarTimeline(instrucciones, drones);

            string json = ConvertirTimelineAJson(timeline);
            return Content(json, "application/json", Encoding.UTF8);
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
    }
}
