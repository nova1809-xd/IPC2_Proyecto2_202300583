using EmisorDrones.CoreBusiness.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace EmisorDrones.Web.Controllers
{
    /// <summary>
    /// controla vistas y acciones del sistema de administración
    /// </summary>
    public class AdminController : Controller
    {
        /// <summary>
        /// panel de administración para cambiar release
        /// </summary>
        public IActionResult Dashboard()
        {
            ViewBag.CurrentRelease = FeatureFlags.CurrentRelease;
            ViewBag.AvailableReleases = Enum.GetValues(typeof(FeatureFlags.Release));

            return View();
        }

        /// <summary>
        /// cambiar el release actual
        /// </summary>
        [HttpPost]
        public IActionResult ChangeRelease(int releaseId)
        {
            if (Enum.IsDefined(typeof(FeatureFlags.Release), releaseId))
            {
                FeatureFlags.CurrentRelease = (FeatureFlags.Release)releaseId;
                TempData["Mensaje"] = $"Release cambiado a: {FeatureFlags.CurrentRelease}";
            }

            return RedirectToAction(nameof(Dashboard));
        }

        /// <summary>
        /// vista que muestra todas las funcionalidades disponibles por release
        /// </summary>
        public IActionResult Features()
        {
            var features = new Dictionary<FeatureFlags.Release, List<FeatureFlags.Feature>>();

            foreach (FeatureFlags.Release release in Enum.GetValues(typeof(FeatureFlags.Release)))
            {
                features[release] = Enum.GetValues(typeof(FeatureFlags.Feature))
                    .Cast<FeatureFlags.Feature>()
                    .Where(f => (int)f == (int)release)
                    .ToList();
            }

            ViewBag.Features = features;
            return View();
        }
    }
}
