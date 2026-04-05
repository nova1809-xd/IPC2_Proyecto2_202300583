using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.Services;
using EmisorDrones.CoreBusiness.TDAs;
using Microsoft.AspNetCore.Mvc;

namespace EmisorDrones.Web.Controllers
{
    /// <summary>
    /// controlador principal que gestiona el dashboard y navegación del sistema
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ServicioDrones _servicioDrones;
        private readonly EstadoSistemaService _estadoSistemaService;

        /// <summary>
        /// constructor con inyección de dependencias
        /// </summary>
        public HomeController(ServicioDrones servicioDrones, EstadoSistemaService estadoSistemaService)
        {
            _servicioDrones = servicioDrones;
            _estadoSistemaService = estadoSistemaService;
        }

        /// <summary>
        /// acción principal que muestra el dashboard
        /// </summary>
        public IActionResult Index()
        {
            const int capacidadMaxima = 200;
            int cantidadDrones = 0;
            ListaEnlazada<string> drones = _estadoSistemaService.NombresDrones;
            NodoGenerico<string> nodoActual = drones.Cabeza;

            while (nodoActual != null)
            {
                cantidadDrones++;
                nodoActual = nodoActual.Siguiente;
            }

            bool tieneCapacidad = cantidadDrones < capacidadMaxima;
            int capacidadDisponible = capacidadMaxima - cantidadDrones;

            ViewBag.CantidadDrones = cantidadDrones;
            ViewBag.TieneCapacidad = tieneCapacidad;
            ViewBag.Resumen =
                "--- SERVICIO DE DRONES ---\n" +
                $"Drones Registrados: {cantidadDrones}/{capacidadMaxima}\n" +
                $"Capacidad Disponible: {capacidadDisponible}\n" +
                $"Sistema Válido: {tieneCapacidad}";

            return View();
        }

        /// <summary>
        /// vista para gestionar drones
        /// </summary>
        public IActionResult GestionarDrones()
        {
            SincronizarDronesDesdeEstado();

            ListaEnlazada<Drone> drones = _servicioDrones.ObtenerTodosDrones();
            drones.OrdenarAlfabeticamente(d => d.NombreDron);
            return View(drones);
        }

        /// <summary>
        /// formulario para agregar nuevo dron
        /// </summary>
        [HttpGet]
        public IActionResult AgregarDron()
        {
            return View();
        }

        /// <summary>
        /// procesa la adición de un nuevo dron
        /// </summary>
        [HttpPost]
        public IActionResult AgregarDron(int id, string nombreDron, int alturaMinima, int alturaMaxima)
        {
            try
            {
                if (id <= 0)
                {
                    ModelState.AddModelError("", "El ID del dron debe ser mayor a 0");
                    return View();
                }

                if (string.IsNullOrWhiteSpace(nombreDron))
                {
                    ModelState.AddModelError("", "El nombre del dron es obligatorio");
                    return View();
                }

                Drone nuevoDron = new Drone(id, nombreDron, alturaMinima, alturaMaxima);

                if (_servicioDrones.AgregarDron(nuevoDron))
                {
                    TempData["Mensaje"] = $"Dron {nombreDron} agregado correctamente";
                    return RedirectToAction(nameof(GestionarDrones));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo agregar el dron. Verifique que el ID y el nombre sean unicos o que haya capacidad");
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        /// <summary>
        /// ver detalles de un dron específico
        /// </summary>
        public IActionResult DetallesDron(int id)
        {
            Drone dron = _servicioDrones.ObtenerDronPorId(id);

            if (dron == null)
            {
                return NotFound();
            }

            return View(dron);
        }

        /// <summary>
        /// eliminar un dron del sistema
        /// </summary>
        [HttpPost]
        public IActionResult EliminarDron(int id)
        {
            Drone dron = _servicioDrones.ObtenerDronPorId(id);

            if (dron != null && _servicioDrones.EliminarDron(id))
            {
                EliminarDronDelEstadoCargado(dron.NombreDron);
                TempData["Mensaje"] = $"Dron {dron.NombreDron} eliminado correctamente";
            }
            else
            {
                TempData["Error"] = $"No se encontró el dron {id}";
            }

            return RedirectToAction(nameof(GestionarDrones));
        }

        /// <summary>
        /// acerca de la aplicación
        /// </summary>
        public IActionResult Acerca()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DescargarDocumentacion()
        {
            string nombreArchivo = "Ensayo_202300583_Proyecto2.pdf";
            string directorioActual = Directory.GetCurrentDirectory();

            string[] rutasPosibles = new[]
            {
                Path.GetFullPath(Path.Combine(directorioActual, nombreArchivo)),
                Path.GetFullPath(Path.Combine(directorioActual, "EmisorDrones.CoreBusiness", nombreArchivo)),
                Path.GetFullPath(Path.Combine(directorioActual, "..", "EmisorDrones.CoreBusiness", nombreArchivo)),
                Path.GetFullPath(Path.Combine(directorioActual, "..", "..", "EmisorDrones.CoreBusiness", nombreArchivo)),
                Path.GetFullPath(Path.Combine(directorioActual, "..", nombreArchivo)),
                Path.GetFullPath(Path.Combine(directorioActual, "..", "..", nombreArchivo))
            };

            string? rutaEnsayo = null;
            for (int i = 0; i < rutasPosibles.Length; i++)
            {
                if (System.IO.File.Exists(rutasPosibles[i]))
                {
                    rutaEnsayo = rutasPosibles[i];
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(rutaEnsayo))
            {
                TempData["Error"] = "No se encontró el archivo de documentación.";
                return RedirectToAction(nameof(Acerca));
            }

            return PhysicalFile(rutaEnsayo, "application/pdf", nombreArchivo);
        }

        [HttpPost]
        public IActionResult InicializarSistema()
        {
            _servicioDrones.LimpiarTodos();
            _estadoSistemaService.LimpiarTodo();

            TempData["Mensaje"] = "Sistema inicializado: listas en memoria limpiadas correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private void SincronizarDronesDesdeEstado()
        {
            int siguienteId = ObtenerSiguienteIdDisponible();
            NodoGenerico<string> nodoNombre = _estadoSistemaService.NombresDrones.Cabeza;

            while (nodoNombre != null)
            {
                string nombreDron = nodoNombre.Dato;

                if (!string.IsNullOrWhiteSpace(nombreDron) && !_servicioDrones.ExisteDronConNombre(nombreDron))
                {
                    Drone dron = new Drone(siguienteId, nombreDron, 1, 100);
                    if (_servicioDrones.AgregarDron(dron))
                    {
                        siguienteId++;
                    }
                }

                nodoNombre = nodoNombre.Siguiente;
            }
        }

        private int ObtenerSiguienteIdDisponible()
        {
            int maxId = 0;
            ListaEnlazada<Drone> drones = _servicioDrones.ObtenerTodosDrones();
            NodoGenerico<Drone> nodoActual = drones.Cabeza;

            while (nodoActual != null)
            {
                if (nodoActual.Dato.Id > maxId)
                {
                    maxId = nodoActual.Dato.Id;
                }

                nodoActual = nodoActual.Siguiente;
            }

            return maxId + 1;
        }

        private void EliminarDronDelEstadoCargado(string nombreDron)
        {
            if (string.IsNullOrWhiteSpace(nombreDron))
                return;

            EliminarNombreDronEnEstado(nombreDron);
            EliminarDronDeSistemasEnEstado(nombreDron);
            EliminarDronDeMensajesEnEstado(nombreDron);
        }

        private void EliminarNombreDronEnEstado(string nombreDron)
        {
            NodoGenerico<string> actual = _estadoSistemaService.NombresDrones.Cabeza;
            int posicion = 0;

            while (actual != null)
            {
                if (string.Equals(actual.Dato, nombreDron, StringComparison.OrdinalIgnoreCase))
                {
                    _estadoSistemaService.NombresDrones.EliminarEnPosicion(posicion);
                    return;
                }

                actual = actual.Siguiente;
                posicion++;
            }
        }

        private void EliminarDronDeSistemasEnEstado(string nombreDron)
        {
            NodoGenerico<SistemaDronesConfig> nodoSistema = _estadoSistemaService.SistemasDrones.Cabeza;

            while (nodoSistema != null)
            {
                SistemaDronesConfig sistema = nodoSistema.Dato;
                NodoGenerico<ContenidoSistemaDrones> nodoContenido = sistema.Contenidos.Cabeza;
                int posicionContenido = 0;

                while (nodoContenido != null)
                {
                    if (string.Equals(nodoContenido.Dato.NombreDron, nombreDron, StringComparison.OrdinalIgnoreCase))
                    {
                        sistema.Contenidos.EliminarEnPosicion(posicionContenido);
                        break;
                    }

                    nodoContenido = nodoContenido.Siguiente;
                    posicionContenido++;
                }

                sistema.CantidadDrones = sistema.Contenidos.Cantidad;
                nodoSistema = nodoSistema.Siguiente;
            }
        }

        private void EliminarDronDeMensajesEnEstado(string nombreDron)
        {
            NodoGenerico<MensajeConfig> nodoMensaje = _estadoSistemaService.Mensajes.Cabeza;
            int posicionMensaje = 0;

            while (nodoMensaje != null)
            {
                MensajeConfig mensaje = nodoMensaje.Dato;
                NodoGenerico<InstruccionMensajeConfig> nodoInstruccion = mensaje.Instrucciones.Cabeza;
                int posicionInstruccion = 0;

                while (nodoInstruccion != null)
                {
                    if (string.Equals(nodoInstruccion.Dato.NombreDron, nombreDron, StringComparison.OrdinalIgnoreCase))
                    {
                        mensaje.Instrucciones.EliminarEnPosicion(posicionInstruccion);
                        nodoInstruccion = mensaje.Instrucciones.Cabeza;
                        posicionInstruccion = 0;
                        continue;
                    }

                    nodoInstruccion = nodoInstruccion.Siguiente;
                    posicionInstruccion++;
                }

                if (mensaje.Instrucciones.EstaVacia)
                {
                    _estadoSistemaService.Mensajes.EliminarEnPosicion(posicionMensaje);
                    nodoMensaje = _estadoSistemaService.Mensajes.Cabeza;
                    posicionMensaje = 0;
                    continue;
                }

                nodoMensaje = nodoMensaje.Siguiente;
                posicionMensaje++;
            }
        }
    }
}
