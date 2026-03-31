using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmisorDrones.Web.Controllers
{
    /// <summary>
    /// Controlador principal que gestiona el dashboard y navegación del sistema
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ServicioDrones _servicioDrones;

        /// <summary>
        /// Constructor con inyección de dependencias
        /// </summary>
        public HomeController(ServicioDrones servicioDrones)
        {
            _servicioDrones = servicioDrones;
        }

        /// <summary>
        /// Acción principal que muestra el dashboard
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.CantidadDrones = _servicioDrones.CantidadDrones;
            ViewBag.TieneCapacidad = _servicioDrones.TieneCapacidad;
            ViewBag.Resumen = _servicioDrones.ObtenerResumen();

            return View();
        }

        /// <summary>
        /// Vista para gestionar drones
        /// </summary>
        public IActionResult GestionarDrones()
        {
            Drone[] drones = _servicioDrones.ObtenerTodosDrones();
            return View(drones);
        }

        /// <summary>
        /// Formulario para agregar nuevo dron
        /// </summary>
        [HttpGet]
        public IActionResult AgregarDron()
        {
            return View();
        }

        /// <summary>
        /// Procesa la adición de un nuevo dron
        /// </summary>
        [HttpPost]
        public IActionResult AgregarDron(int id, int alturaMinima, int alturaMaxima)
        {
            try
            {
                if (id <= 0)
                {
                    ModelState.AddModelError("", "El ID del dron debe ser mayor a 0");
                    return View();
                }

                Drone nuevoDron = new Drone(id, alturaMinima, alturaMaxima);

                if (_servicioDrones.AgregarDron(nuevoDron))
                {
                    TempData["Mensaje"] = $"Dron {id} agregado correctamente";
                    return RedirectToAction(nameof(GestionarDrones));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo agregar el dron. Verifique que no exista con el mismo ID o que haya capacidad");
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        /// <summary>
        /// Ver detalles de un dron específico
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
        /// Eliminar un dron del sistema
        /// </summary>
        [HttpPost]
        public IActionResult EliminarDron(int id)
        {
            if (_servicioDrones.EliminarDron(id))
            {
                TempData["Mensaje"] = $"Dron {id} eliminado correctamente";
            }
            else
            {
                TempData["Error"] = $"No se encontró el dron {id}";
            }

            return RedirectToAction(nameof(GestionarDrones));
        }

        /// <summary>
        /// Acerca de la aplicación
        /// </summary>
        public IActionResult Acerca()
        {
            return View();
        }
    }
}
