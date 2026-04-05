using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Services
{
    public class MotorOptimizacionService
    {
        public ListaEnlazada<InstanteTiempo> GenerarTimeline(
            ListaEnlazada<Instruccion> instrucciones,
            ListaEnlazada<Dron> dronesDelSistema)
        {
            ListaEnlazada<InstanteTiempo> timeline = new ListaEnlazada<InstanteTiempo>();

            if (instrucciones == null || dronesDelSistema == null)
                return timeline;

            int segundoActual = 0;

            int i = 0;
            while (i < instrucciones.Cantidad)
            {
                Instruccion instruccion = instrucciones.ObtenerEnPosicion(i);
                Dron dron = BuscarDronPorId(dronesDelSistema, instruccion.DronId);

                if (dron != null)
                {
                    int alturaObjetivo = instruccion.AlturaObjetivo;
                    if (alturaObjetivo < dron.AlturaMinima)
                        alturaObjetivo = dron.AlturaMinima;

                    if (alturaObjetivo > dron.AlturaMaxima)
                        alturaObjetivo = dron.AlturaMaxima;

                    while (dron.AlturaActual < alturaObjetivo)
                    {
                        segundoActual++;
                        InstanteTiempo instanteSubida = CrearInstanteConPadding(segundoActual, dronesDelSistema);
                        dron.SubirUnMetro();
                        ReemplazarMovimiento(instanteSubida, dron.NombreDron, "Subir");
                        timeline.AgregarAlFinal(instanteSubida);
                    }

                    while (dron.AlturaActual > alturaObjetivo)
                    {
                        segundoActual++;
                        InstanteTiempo instanteBajada = CrearInstanteConPadding(segundoActual, dronesDelSistema);
                        dron.BajarUnMetro();
                        ReemplazarMovimiento(instanteBajada, dron.NombreDron, "Bajar");
                        timeline.AgregarAlFinal(instanteBajada);
                    }

                    segundoActual++;
                    InstanteTiempo instanteLuz = CrearInstanteConPadding(segundoActual, dronesDelSistema);
                    ReemplazarMovimiento(instanteLuz, dron.NombreDron, dron.AlturaActual.ToString());
                    timeline.AgregarAlFinal(instanteLuz);
                }

                i++;
            }

            return timeline;
        }

        private InstanteTiempo CrearInstanteConPadding(int segundo, ListaEnlazada<Dron> drones)
        {
            InstanteTiempo instante = new InstanteTiempo(segundo);

            int i = 0;
            while (i < drones.Cantidad)
            {
                Dron dron = drones.ObtenerEnPosicion(i);
                instante.Acciones.AgregarAlFinal(new AccionDron(dron.NombreDron, "Esperar"));
                i++;
            }

            return instante;
        }

        private void ReemplazarMovimiento(InstanteTiempo instante, string nombreDron, string movimiento)
        {
            int i = 0;
            while (i < instante.Acciones.Cantidad)
            {
                AccionDron accion = instante.Acciones.ObtenerEnPosicion(i);

                if (accion.NombreDron == nombreDron)
                {
                    accion.Movimiento = movimiento;
                    return;
                }

                i++;
            }
        }

        private Dron BuscarDronPorId(ListaEnlazada<Dron> drones, int dronId)
        {
            int i = 0;
            while (i < drones.Cantidad)
            {
                Dron dron = drones.ObtenerEnPosicion(i);
                if (dron.Id == dronId)
                    return dron;

                i++;
            }

            return null;
        }
    }
}
