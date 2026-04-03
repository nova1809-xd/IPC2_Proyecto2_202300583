using EmisorDrones.CoreBusiness.TDAs;
using EmisorDrones.CoreBusiness.Models;

namespace EmisorDrones.CoreBusiness.Services
{
    /// <summary>
    /// Servicio para gestionar el banco de drones del sistema.
    /// Capacidad máxima: 200 drones.
    /// Usa TDA ListaEnlazada en lugar de List\<T\>.
    /// </summary>
    public class ServicioDrones
    {
        /// <summary>
        /// Capacidad máxima de drones del sistema
        /// </summary>
        private const int CAPACIDAD_MAXIMA = 200;

        /// <summary>
        /// Lista dinámica que almacena todos los drones
        /// </summary>
        private ListaEnlazada<Drone> drones;

        /// <summary>
        /// Constructor que inicializa el servicio
        /// </summary>
        public ServicioDrones()
        {
            drones = new ListaEnlazada<Drone>();
        }

        /// <summary>
        /// Obtiene cantidad de drones registrados
        /// </summary>
        public int CantidadDrones => drones.Cantidad;

        /// <summary>
        /// Verifica si hay capacidad para más drones
        /// </summary>
        public bool TieneCapacidad => drones.Cantidad < CAPACIDAD_MAXIMA;

        /// <summary>
        /// Agrega un nuevo dron al sistema
        /// </summary>
        /// <param name="dron">Dron a agregar</param>
        /// <returns>true si se agregó con éxito, false si no hay capacidad o ID duplicado</returns>
        public bool AgregarDron(Drone dron)
        {
            if (dron == null)
                throw new ArgumentNullException(nameof(dron));

            if (!TieneCapacidad)
                return false;

            // Verificar que no existe dron con mismo ID
            if (ExisteDronConId(dron.Id))
                return false;

            drones.AgregarAlFinal(dron);
            return true;
        }

        /// <summary>
        /// Obtiene un dron por su ID
        /// </summary>
        /// <param name="id">ID del dron a obtener</param>
        /// <returns>Dron encontrado o null</returns>
        public Drone ObtenerDronPorId(int id)
        {
            NodoGenerico<Drone> actual = drones.Cabeza;

            while (actual != null)
            {
                if (actual.Dato.Id == id)
                    return actual.Dato;

                actual = actual.Siguiente;
            }

            return null;
        }

        /// <summary>
        /// Obtiene todos los drones del sistema
        /// </summary>
        /// <returns>lista enlazada con los drones del sistema</returns>
        public ListaEnlazada<Drone> ObtenerTodosDrones()
        {
            return drones;
        }

        /// <summary>
        /// Verifica si existe un dron con un ID específico
        /// </summary>
        /// <param name="id">ID a verificar</param>
        /// <returns>true si existe, false en caso contrario</returns>
        public bool ExisteDronConId(int id)
        {
            return ObtenerDronPorId(id) != null;
        }

        /// <summary>
        /// Obtiene drones que pueden alcanzar una altura específica
        /// </summary>
        /// <param name="altura">Altura a verificar</param>
        /// <returns>lista enlazada de drones que pueden alcanzar la altura</returns>
        public ListaEnlazada<Drone> ObtenerDronesQuePuedenAlcanzarAltura(int altura)
        {
            ListaEnlazada<Drone> candidatos = new ListaEnlazada<Drone>();
            NodoGenerico<Drone> actual = drones.Cabeza;

            while (actual != null)
            {
                if (actual.Dato.PuedeAlcanzarAltura(altura))
                    candidatos.AgregarAlFinal(actual.Dato);

                actual = actual.Siguiente;
            }

            return candidatos;
        }

        /// <summary>
        /// Elimina un dron del sistema por su ID
        /// </summary>
        /// <param name="id">ID del dron a eliminar</param>
        /// <returns>true si se eliminó, false si no existe</returns>
        public bool EliminarDron(int id)
        {
            NodoGenerico<Drone> actual = drones.Cabeza;
            int posicion = 0;

            while (actual != null)
            {
                if (actual.Dato.Id == id)
                {
                    drones.EliminarEnPosicion(posicion);
                    return true;
                }

                actual = actual.Siguiente;
                posicion++;
            }

            return false;
        }

        /// <summary>
        /// Verifica si todos los drones están en rango de alturas válidas
        /// </summary>
        /// <returns>true si sistema es válido</returns>
        public bool ValidarSistema()
        {
            NodoGenerico<Drone> actual = drones.Cabeza;

            while (actual != null)
            {
                Drone d = actual.Dato;

                if (d.AlturaMinima < 1 || d.AlturaMaxima > 100)
                    return false;

                if (d.AlturaMinima > d.AlturaMaxima)
                    return false;

                actual = actual.Siguiente;
            }

            return true;
        }

        /// <summary>
        /// Limpia todos los drones del sistema
        /// </summary>
        public void LimpiarTodos()
        {
            drones.Limpiar();
        }

        /// <summary>
        /// Obtiene un resumen informativo del servicio
        /// </summary>
        public string ObtenerResumen()
        {
            return $@"
=== SERVICIO DE DRONES ===
Drones Registrados: {CantidadDrones}/{CAPACIDAD_MAXIMA}
Capacidad Disponible: {CAPACIDAD_MAXIMA - CantidadDrones}
Sistema Válido: {ValidarSistema()}
";
        }
    }
}
